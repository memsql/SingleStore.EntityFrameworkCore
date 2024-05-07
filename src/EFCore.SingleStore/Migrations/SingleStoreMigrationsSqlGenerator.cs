// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Metadata.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Migrations
{
    // CHECK: Can we increase the usage of the new model over the old one, or are we done here?
    /// <summary>
    ///     SingleStore-specific implementation of <see cref="MigrationsSqlGenerator" />.
    /// </summary>
    public class SingleStoreMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        private static readonly Regex _typeRegex = new Regex(@"(?<Name>[a-z0-9]+)\s*?(?:\(\s*(?<Length>\d+)?\s*\))?",
            RegexOptions.IgnoreCase);

        private static readonly HashSet<string> _spatialStoreTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "geometry",
            "point",
            "curve",
            "linestring",
            "line",
            "linearring",
            "surface",
            "polygon",
            "geometrycollection",
            "multipoint",
            "multicurve",
            "multilinestring",
            "multisurface",
            "multipolygon",
        };

        private readonly IRelationalAnnotationProvider _annotationProvider;
        private readonly ISingleStoreOptions _options;
        private readonly RelationalTypeMapping _stringTypeMapping;

        public SingleStoreMigrationsSqlGenerator(
            [NotNull] MigrationsSqlGeneratorDependencies dependencies,
            [NotNull] IRelationalAnnotationProvider annotationProvider,
            [NotNull] ISingleStoreOptions options)
            : base(dependencies)
        {
            _annotationProvider = annotationProvider;
            _options = options;
            _stringTypeMapping = dependencies.TypeMappingSource.GetMapping(typeof(string));
        }

        /// <summary>
        ///     Generates commands from a list of operations.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="model">The target model which may be <see langword="null" /> if the operations exist without a model.</param>
        /// <param name="options">The options to use when generating commands.</param>
        /// <returns>The list of commands to be executed or scripted.</returns>
        public override IReadOnlyList<MigrationCommand> Generate(
            IReadOnlyList<MigrationOperation> operations,
            IModel model,
            MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
        {
            Check.NotNull(operations, nameof(operations));

            Options = options;

            var builder = new MigrationCommandListBuilder(Dependencies);
            try
            {
                UpdateMigrationOperations(operations);
                foreach (var operation in operations)
                {
                    if (!IsFullText(operation))
                    {
                        Generate(operation, model, builder);
                    }
                }
            }
            finally
            {
                Options = MigrationsSqlGenerationOptions.Default;
            }

            return builder.GetCommandList();
        }

        private void UpdateMigrationOperations(IReadOnlyList<MigrationOperation> operations)
        {
            foreach (var operation in operations)
            {
                if (operation is CreateIndexOperation createIndexOperation)
                {
                    if (IsFullText(operation))
                    {
                        var createTableOperation = (CreateTableOperation)operations.Single(o =>
                            (o is CreateTableOperation createTableOperation && createTableOperation.Name == createIndexOperation.Table));

                        try
                        {
                            createTableOperation.AddAnnotation(SingleStoreAnnotationNames.FullTextIndex, createIndexOperation.Columns);
                        }
                        catch (InvalidOperationException)
                        {
                            throw new InvalidOperationException("Feature 'more than one FULLTEXT KEY' is not supported by SingleStore.");
                        }
                    }
                }
            }
        }

        private static bool IsFullText(MigrationOperation operation)
        {
            return operation is CreateIndexOperation &&
                   operation[SingleStoreAnnotationNames.FullTextIndex] as bool? == true;
        }

        /// <summary>
        ///     <para>
        ///         Builds commands for the given <see cref="MigrationOperation" /> by making calls on the given
        ///         <see cref="MigrationCommandListBuilder" />.
        ///     </para>
        ///     <para>
        ///         This method uses a double-dispatch mechanism to call one of the 'Generate' methods that are
        ///         specific to a certain subtype of <see cref="MigrationOperation" />. Typically database providers
        ///         will override these specific methods rather than this method. However, providers can override
        ///         this methods to handle provider-specific operations.
        ///     </para>
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            CheckSchema(operation);

            switch (operation)
            {
                case SingleStoreCreateDatabaseOperation createDatabaseOperation:
                    Generate(createDatabaseOperation, model, builder);
                    break;
                case SingleStoreDropDatabaseOperation dropDatabaseOperation:
                    Generate(dropDatabaseOperation, model, builder);
                    break;
                default:
                    base.Generate(operation, model, builder);
                    break;
            }
        }

        protected virtual void CheckSchema(MigrationOperation operation)
        {
            if (_options.SchemaNameTranslator != null)
            {
                return;
            }

            var schema = operation.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(p => p.Name.IndexOf(nameof(AddForeignKeyOperation.Schema), StringComparison.Ordinal) >= 0)
                .Select(p => p.GetValue(operation) as string)
                .FirstOrDefault(schemaValue => schemaValue != null);

            if (schema != null)
            {
                var name = operation.GetType()
                    .GetProperty(nameof(AddForeignKeyOperation.Name), BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                    ?.GetValue(operation) as string;

                throw new InvalidOperationException($"A schema \"{schema}\" has been set for an object of type \"{operation.GetType().Name}\"{(string.IsNullOrEmpty(name) ? string.Empty : $" with the name of \"{name}\"")}. MySQL does not support the EF Core concept of schemas. Any schema property of any \"MigrationOperation\" must be null. This behavior can be changed by setting the `SchemaBehavior` option in the `UseSingleStore` call.");
            }
        }

        protected override void Generate(
            [NotNull] CreateTableOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .AppendLine(" (");

            using (builder.Indent())
            {
                CreateTableColumns(operation, model, builder);
                CreateTableConstraints(operation, model, builder);
                CreateFullTextIndex(operation, builder);
                builder.AppendLine();
            }

            builder.Append(")");

            var tableOptions = new List<(string, string)>();

            if (operation[SingleStoreAnnotationNames.CharSet] is string charSet)
            {
                tableOptions.Add(("CHARACTER SET", charSet));
            }

            if (operation[RelationalAnnotationNames.Collation] is string collation)
            {
                tableOptions.Add(("COLLATE", collation));
            }

            if (operation.Comment != null)
            {
                tableOptions.Add(("COMMENT", SingleStoreStringTypeMapping.EscapeSqlLiteralWithLineBreaks(operation.Comment, !_options.NoBackslashEscapes, false)));
            }

            tableOptions.AddRange(
                SingleStoreEntityTypeExtensions.DeserializeTableOptions(operation[SingleStoreAnnotationNames.StoreOptions] as string)
                    .Select(kvp => (kvp.Key, kvp.Value)));

            foreach (var (key, value) in tableOptions)
            {
                builder
                    .Append(" ")
                    .Append(key)
                    .Append("=")
                    .Append(value);
            }

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        private void CreateFullTextIndex(CreateTableOperation operation, MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var fullTextIndex = operation[SingleStoreAnnotationNames.FullTextIndex] as string[];
            if (fullTextIndex != null)
            {
                builder.AppendLine(",");
                builder
                    .Append("FULLTEXT(")
                    .Append(ColumnList(fullTextIndex))
                    .Append(")");
            }
        }

        protected override void CreateTableConstraints(
            CreateTableOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            CreateTablePrimaryKeyConstraint(operation, model, builder);
            // We're disabling this functionality because SingleStore doesn't support foreign keys and check constraints.
            // EF Core creates unique index for each foreign key that responds for OneToOne relationship between entities.
            // We plan on adding this functionality back when SingleStore is supporting foreign keys (~ end of 2023).
            /*CreateTableUniqueConstraints(operation, model, builder);
            CreateTableCheckConstraints(operation, model, builder);
            CreateTableForeignKeys(operation, model, builder);*/
        }
        protected override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation.Comment != operation.OldTable.Comment)
            {
                builder.Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema));

                // An existing comment will be removed, when set to an empty string.
                GenerateComment(operation.Comment ?? string.Empty, builder);

                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }

            var oldTableOptions = SingleStoreEntityTypeExtensions.DeserializeTableOptions(operation.OldTable[SingleStoreAnnotationNames.StoreOptions] as string);
            var newTableOptions = SingleStoreEntityTypeExtensions.DeserializeTableOptions(operation[SingleStoreAnnotationNames.StoreOptions] as string);
            var addedOrChangedTableOptions = newTableOptions.Except(oldTableOptions).ToArray();

            if (addedOrChangedTableOptions.Length > 0)
            {
                builder
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema));

                foreach (var (key, value) in addedOrChangedTableOptions)
                {
                    builder
                        .Append(" ")
                        .Append(key)
                        .Append("=")
                        .Append(value);
                }

                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="AlterColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            AlterColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" MODIFY COLUMN ");

            ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.Name,
                operation,
                model,
                builder);

            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            builder.EndCommand();
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameIndexOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (string.IsNullOrEmpty(operation.Table))
            {
                throw new InvalidOperationException(SingleStoreStrings.IndexTableRequired);
            }

            if (operation.NewName != null)
            {
                if (_options.ServerVersion.Supports.RenameIndex)
                {
                    builder.Append("ALTER TABLE ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                        .Append(" RENAME INDEX ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                        .Append(" TO ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                        .AppendLine(";");

                    EndStatement(builder);
                }
                else
                {
                    var index = model?
                        .GetRelationalModel()
                        .FindTable(operation.Table, operation.Schema)
                        ?.Indexes
                        .FirstOrDefault(i => i.Name == operation.NewName);

                    if (index == null)
                    {
                        throw new InvalidOperationException(
                            $"Could not find the model index: {Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema)}.{Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName)}. Upgrade to Mysql 5.7+ or split the 'RenameIndex' call into 'DropIndex' and 'CreateIndex'");
                    }

                    Generate(new DropIndexOperation
                    {
                        Schema = operation.Schema,
                        Table = operation.Table,
                        Name = operation.Name
                    }, model, builder);

                    var createIndexOperation = CreateIndexOperation.CreateFrom(index);
                    createIndexOperation.Name = operation.NewName;

                    Generate(createIndexOperation, model, builder);
                }
            }
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RestartSequenceOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RestartSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            if (!_options.ServerVersion.Supports.Sequences)
            {
                throw new InvalidOperationException(
                    $"Cannot restart sequence '{operation.Name}' because sequences are not supported in server version {_options.ServerVersion}.");
            }
            builder
                .Append("ALTER SEQUENCE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .Append(" RESTART WITH ")
                .Append(IntegerConstant(operation.StartValue))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameTableOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameTableOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .Append(" RENAME TO ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName, operation.NewSchema))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            EndStatement(builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="CreateIndexOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected override void Generate(
            CreateIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (!_options.ServerVersion.Supports.SpatialIndexes &&
                operation[SingleStoreAnnotationNames.SpatialIndex] is true)
            {
                Dependencies.MigrationsLogger.Logger.LogWarning(
                    $"Spatial indexes are not supported on {_options.ServerVersion}. The CREATE INDEX operation will be ignored.");
                return;
            }

            builder.Append("CREATE ");

            // We're disabling this functionality because SingleStore doesn't support foreign keys.
            // EF Core creates unique index for each foreign key that responds for OneToOne relationship between entities.
            // We plan on adding this functionality back when SingleStore is supporting foreign keys (~ end of 2023)
            /*if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }*/

            IndexTraits(operation, model, builder);

            builder
                .Append("INDEX ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(Truncate(operation.Name, 64)))
                .Append(" ON ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" (")
                .Append(ColumnListWithIndexPrefixLength(operation, operation.Columns))
                .Append(")");

            IndexOptions(operation, model, builder);

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(EnsureSchemaOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(DropSchemaOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        ///     Builds commands for the given <see cref="CreateSequenceOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            [NotNull] CreateSequenceOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));
            if (!_options.ServerVersion.Supports.Sequences)
            {
                throw new InvalidOperationException(
                    $"Cannot create sequence '{operation.Name}' because sequences are not supported in server version {_options.ServerVersion}.");
            }

            // "CREATE SEQUENCE"  supported only in MariaDb from 10.3.
            // However, "CREATE SEQUENCE name AS type" expression is currently not supported.
            // The base MigrationsSqlGenerator.Generate method generates that expression.
            // Also, when creating a sequence current version of MariaDb doesn't tolerate "NO MINVALUE"
            // when specifying "STARTS WITH" so, StartValue mus be set accordingly.
            // https://github.com/aspnet/EntityFrameworkCore/blob/master/src/EFCore.Relational/Migrations/MigrationsSqlGenerator.cs#L535-L543
            var oldValue = operation.ClrType;
            operation.ClrType = typeof(long);
            if (operation.StartValue <= 0 )
            {
                operation.MinValue = operation.StartValue;
            }
            base.Generate(operation, model, builder);
            operation.ClrType = oldValue;
        }

        /// <summary>
        ///     Builds commands for the given <see cref="SingleStoreCreateDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected virtual void Generate(
            [NotNull] SingleStoreCreateDatabaseOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE DATABASE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            if (operation.CharSet != null)
            {
                builder
                    .Append(" CHARACTER SET ")
                    .Append(operation.CharSet);
            }

            if (operation.Collation != null)
            {
                builder
                    .Append(" COLLATE ")
                    .Append(operation.Collation);
            }

            builder
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        /// <summary>
        ///     Builds commands for the given <see cref="SingleStoreDropDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected virtual void Generate(
            [NotNull] SingleStoreDropDatabaseOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("DROP DATABASE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(Dependencies.SqlGenerationHelper.StatementTerminator)
                .AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);
            EndStatement(builder);
        }

        protected override void Generate(
            [NotNull] DropIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (string.IsNullOrEmpty(operation.Table))
            {
                throw new InvalidOperationException(SingleStoreStrings.IndexTableRequired);
            }

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP INDEX ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            if (terminate)
            {
                builder
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
            }
        }

        protected override void Generate(
            DropUniqueConstraintOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
            => Generate(
                new SingleStoreDropUniqueConstraintAndRecreateForeignKeysOperation
                {
                    IsDestructiveChange = operation.IsDestructiveChange,
                    Name = operation.Name,
                    Schema = operation.Schema,
                    Table = operation.Table,
                    RecreateForeignKeys = false,
                },
                model,
                builder);

        protected virtual void Generate(
            SingleStoreDropUniqueConstraintAndRecreateForeignKeysOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            void DropUniqueKey()
            {
                builder.Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                    .Append(" DROP KEY ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

                EndStatement(builder);
            }

            DropUniqueKey();

        }

        protected static ReferentialAction ToReferentialAction(DeleteBehavior deleteBehavior)
        {
            switch (deleteBehavior)
            {
                case DeleteBehavior.SetNull:
                    return ReferentialAction.SetNull;
                case DeleteBehavior.Cascade:
                    return ReferentialAction.Cascade;
                case DeleteBehavior.NoAction:
                case DeleteBehavior.ClientNoAction:
                    return ReferentialAction.NoAction;
                default:
                    return ReferentialAction.Restrict;
            }
        }

        // CHECK: Can we improve this implementation?
        /// <summary>
        ///     Builds commands for the given <see cref="RenameColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table))
                .Append(" CHANGE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        /// <summary>
        ///     Generates a SQL fragment for a column definition in an <see cref="AddColumnOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ColumnDefinition(AddColumnOperation operation, IModel model,
            MigrationCommandListBuilder builder)
            => ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.Name,
                operation,
                model,
                builder);

        /// <summary>
        ///     Generates a SQL fragment for a column definition for the given column metadata.
        /// </summary>
        /// <param name="schema"> The schema that contains the table, or <see langword="null"/> to use the default schema. </param>
        /// <param name="table"> The table that contains the column. </param>
        /// <param name="name"> The column name. </param>
        /// <param name="operation"> The column metadata. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ColumnDefinition(
            [CanBeNull] string schema,
            [NotNull] string table,
            [NotNull] string name,
            [NotNull] ColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var matchType = GetColumnType(schema, table, name, operation, model);
            var matchLen = "";
            var match = _typeRegex.Match(matchType ?? "-");
            if (match.Success)
            {
                matchType = match.Groups["Name"].Value.ToLower();
                if (match.Groups["Length"].Success)
                {
                    matchLen = match.Groups["Length"].Value;
                }
            }

            var valueGenerationStrategy = SingleStoreValueGenerationStrategyCompatibility.GetValueGenerationStrategy(operation.GetAnnotations().OfType<IAnnotation>().ToArray());

            var autoIncrement = false;
            if (valueGenerationStrategy == SingleStoreValueGenerationStrategy.IdentityColumn &&
                string.IsNullOrWhiteSpace(operation.DefaultValueSql) && operation.DefaultValue == null)
            {
                switch (matchType)
                {
                    case "bigint":
                        autoIncrement = true;
                        break;
                    case "datetime":
                        if (!_options.ServerVersion.Supports.DateTimeCurrentTimestamp)
                        {
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                $"on Add or Update in server version {_options.ServerVersion}. Try explicitly setting the column type to TIMESTAMP.");
                        }

                        goto case "timestamp";
                    case "timestamp":
                        operation.DefaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
                        break;
                }
            }

            string onUpdateSql = null;
            if (operation.IsRowVersion || valueGenerationStrategy == SingleStoreValueGenerationStrategy.ComputedColumn)
            {
                switch (matchType)
                {
                    case "datetime":
                        if (!_options.ServerVersion.Supports.DateTimeCurrentTimestamp)
                        {
                            throw new InvalidOperationException(
                                $"Error in {table}.{name}: DATETIME does not support values generated " +
                                $"on Add or Update in server version {_options.ServerVersion}. Try explicitly setting the column type to TIMESTAMP.");
                        }

                        goto case "timestamp";
                    case "timestamp":
                        if (string.IsNullOrWhiteSpace(operation.DefaultValueSql) && operation.DefaultValue == null)
                        {
                            operation.DefaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
                        }

                        onUpdateSql = $"CURRENT_TIMESTAMP({matchLen})";
                        break;
                }
            }

            if (operation.ComputedColumnSql == null)
            {
                ColumnDefinitionWithCharSet(schema, table, name, operation, model, builder);

                if (autoIncrement)
                {
                    builder.Append(" AUTO_INCREMENT");
                }

                GenerateComment(operation.Comment, builder);

                // AUTO_INCREMENT has priority over reference definitions.
                if (onUpdateSql != null && !autoIncrement)
                {
                    builder
                        .Append(" ON UPDATE ")
                        .Append(onUpdateSql);
                }
            }
            else
            {
                builder
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                    .Append(" AS ")
                    .Append($"({operation.ComputedColumnSql})")
                    .Append(" PERSISTED")
                    .Append(" ")
                    .Append(GetColumnType(schema, table, name, operation, model));

                if (operation.IsNullable && _options.ServerVersion.Supports.NullableGeneratedColumns)
                {
                    builder.Append(" NULL");
                }

                GenerateComment(operation.Comment, builder);
            }
        }

        private void GenerateComment(string comment, MigrationCommandListBuilder builder)
        {
            if (comment == null)
            {
                return;
            }

            builder.Append(" COMMENT ")
                .Append(SingleStoreStringTypeMapping.EscapeSqlLiteralWithLineBreaks(comment, !_options.NoBackslashEscapes, false));
        }

        private void ColumnDefinitionWithCharSet(string schema, string table, string name, ColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation.ComputedColumnSql != null)
            {
                ComputedColumnDefinition(schema, table, name, operation, model, builder);
                return;
            }

            var columnType = GetColumnType(schema, table, name, operation, model);

            builder
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                .Append(" ")
                .Append(columnType);

            builder.Append(operation.IsNullable ? " NULL" : " NOT NULL");

            DefaultValue(operation.DefaultValue, operation.DefaultValueSql, columnType, builder);

            var srid = operation[SingleStoreAnnotationNames.SpatialReferenceSystemId];
            if (srid is int &&
                IsSpatialStoreType(columnType))
            {
                builder.Append($" /*!80003 SRID {srid} */");
            }
        }

        protected override string GetColumnType(string schema, string table, string name, ColumnOperation operation, IModel model)
            => GetColumnTypeWithCharSetAndCollation(
                operation,
                operation.ColumnType ?? base.GetColumnType(schema, table, name, operation, model));

        private static string GetColumnTypeWithCharSetAndCollation(ColumnOperation operation, string columnType)
        {
            if (columnType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return columnType;
            }

            var charSet = operation[SingleStoreAnnotationNames.CharSet];
            if (charSet != null)
            {
                const string characterSetClausePattern = @"(CHARACTER SET|CHARSET)\s+\w+";
                var characterSetClause = $@"CHARACTER SET {charSet}";

                columnType = Regex.IsMatch(columnType, characterSetClausePattern, RegexOptions.IgnoreCase)
                    ? Regex.Replace(columnType, characterSetClausePattern, characterSetClause)
                    : columnType.TrimEnd() + " " + characterSetClause;
            }

            // At this point, all legacy `SingleStore:Collation` annotations should have been replaced by `Relational:Collation` ones.
#pragma warning disable 618
            Debug.Assert(operation.FindAnnotation(SingleStoreAnnotationNames.Collation) == null);
#pragma warning restore 618

            // Also at this point, all explicitly added `Relational:Collation` annotations (through delegation) should have been set to the
            // `Collation` property and removed.
            Debug.Assert(operation.FindAnnotation(RelationalAnnotationNames.Collation) == null);

            // If we set the collation through delegation, we use the `Relational:Collation` annotation, so the collation will not be in the
            // `Collation` property.
            var collation = operation.Collation;
            if (collation != null)
            {
                const string collationClausePattern = @"COLLATE \w+";
                var collationClause = $@"COLLATE {collation}";

                columnType = Regex.IsMatch(columnType, collationClausePattern, RegexOptions.IgnoreCase)
                    ? Regex.Replace(columnType, collationClausePattern, collationClause)
                    : columnType.TrimEnd() + " " + collationClause;
            }

            return columnType;
        }

        protected override void DefaultValue(
            object defaultValue,
            string defaultValueSql,
            string columnType,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (defaultValueSql is not null)
            {
                if (IsDefaultValueSqlSupported(defaultValueSql, columnType))
                {
                    builder
                        .Append(" DEFAULT ")
                        .Append(defaultValueSql);
                }
                else
                {
                    Dependencies.MigrationsLogger.DefaultValueNotSupportedWarning(defaultValueSql, _options.ServerVersion, columnType);
                }
            }
            else if (defaultValue is not null)
            {
                var isDefaultValueSupported = IsDefaultValueSupported(columnType);
                var supportsDefaultExpressionSyntax = _options.ServerVersion.Supports.DefaultExpression ||
                                                      _options.ServerVersion.Supports.AlternativeDefaultExpression;

                var typeMapping = Dependencies.TypeMappingSource.GetMappingForValue(defaultValue);

                if (typeMapping is IDefaultValueCompatibilityAware defaultValueCompatibilityAware)
                {
                    typeMapping = defaultValueCompatibilityAware.Clone(true);
                }

                var sqlLiteralDefaultValue = typeMapping.GenerateSqlLiteral(defaultValue);

                if (isDefaultValueSupported ||
                    supportsDefaultExpressionSyntax)
                {
                    var useDefaultExpressionSyntax = !isDefaultValueSupported;

                    builder.Append(" DEFAULT ");

                    if (useDefaultExpressionSyntax)
                    {
                        builder.Append("(");
                    }

                    builder.Append(sqlLiteralDefaultValue);

                    if (useDefaultExpressionSyntax)
                    {
                        builder.Append(")");
                    }
                }
                else
                {
                    Dependencies.MigrationsLogger.DefaultValueNotSupportedWarning(
                        sqlLiteralDefaultValue,
                        _options.ServerVersion,
                        columnType);
                }
            }
        }

        private bool IsDefaultValueSqlSupported(string defaultValueSql, string columnType)
        {
            if (IsDefaultValueSupported(columnType))
            {
                return true;
            }

            var trimmedDefaultValueSql = defaultValueSql.Trim();

            if (_options.ServerVersion.Supports.DefaultExpression)
            {
                if (trimmedDefaultValueSql.StartsWith("(") && trimmedDefaultValueSql.EndsWith(")"))
                {
                    return true;
                }
            }
            else if (_options.ServerVersion.Supports.AlternativeDefaultExpression)
            {
                if ((trimmedDefaultValueSql.EndsWith("()") && !trimmedDefaultValueSql.StartsWith("(")) ||
                    (trimmedDefaultValueSql.StartsWith("(") && trimmedDefaultValueSql.EndsWith(")")))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Generates a SQL fragment for the primary key constraint of a <see cref="CreateTableOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void CreateTablePrimaryKeyConstraint(
            [NotNull] CreateTableOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var primaryKey = operation.PrimaryKey;
            if (primaryKey != null)
            {
                builder.AppendLine(",");

                // MySQL InnoDB has the requirement, that an AUTO_INCREMENT column has to be the first
                // column participating in an index.

                var sortedColumnNames = primaryKey.Columns.Length > 1
                    ? primaryKey.Columns
                        .Select(columnName => operation.Columns.First(co => co.Name == columnName))
                        .OrderBy(co => co[SingleStoreAnnotationNames.ValueGenerationStrategy] is SingleStoreValueGenerationStrategy generationStrategy
                                       && generationStrategy == SingleStoreValueGenerationStrategy.IdentityColumn
                            ? 0
                            : 1)
                        .Select(co => co.Name)
                        .ToArray()
                    : primaryKey.Columns;

                var sortedPrimaryKey = new AddPrimaryKeyOperation()
                {
                    Schema = primaryKey.Schema,
                    Table = primaryKey.Table,
                    Name = primaryKey.Name,
                    Columns = sortedColumnNames,
                    IsDestructiveChange = primaryKey.IsDestructiveChange,
                };

                foreach (var annotation in primaryKey.GetAnnotations())
                {
                    sortedPrimaryKey[annotation.Name] = annotation.Value;
                }

                PrimaryKeyConstraint(
                    sortedPrimaryKey,
                    model,
                    builder);
            }
        }

        protected override void Generate(
            AddForeignKeyOperation operation,
            [CanBeNull] IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            // Feature 'FOREIGN KEY' is not supported by SingleStore.
        }

        protected override void ForeignKeyConstraint(
            AddForeignKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            // Feature 'FOREIGN KEY' is not supported by SingleStore.
        }

        protected override void PrimaryKeyConstraint(
            [NotNull] AddPrimaryKeyOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("PRIMARY KEY ");

            IndexTraits(operation, model, builder);

            builder.Append("(")
                .Append(ColumnListWithIndexPrefixLength(operation, operation.Columns))
                .Append(")");
        }

        protected override void UniqueConstraint(
            [NotNull] AddUniqueConstraintOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            // We're disabling this functionality because SingleStore doesn't support foreign keys.
            // EF Core creates unique index for each foreign key that responds for OneToOne relationship between entities.
            // We plan on adding this functionality back when SingleStore is supporting foreign keys (~ end of 2023)
            /*Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("UNIQUE ");

            IndexTraits(operation, model, builder);

            builder.Append("(")
                .Append(ColumnListWithIndexPrefixLength(operation, operation.Columns))
                .Append(")");*/
        }

        protected override void Generate(AddPrimaryKeyOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" ADD ");
            PrimaryKeyConstraint(operation, model, builder);
            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            if (operation.Columns.Length == 1)
            {
                builder.Append(
                    $"CALL POMELO_AFTER_ADD_PRIMARY_KEY({_stringTypeMapping.GenerateSqlLiteral(operation.Schema)}, {_stringTypeMapping.GenerateSqlLiteral(operation.Table)}, {_stringTypeMapping.GenerateSqlLiteral(operation.Columns.First())});");

                builder.AppendLine();
            }

            EndStatement(builder);
        }

        protected override void Generate(
            DropPrimaryKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
            => Generate(
                new SingleStoreDropPrimaryKeyAndRecreateForeignKeysOperation
                {
                    IsDestructiveChange = operation.IsDestructiveChange,
                    Name = operation.Name,
                    Schema = operation.Schema,
                    Table = operation.Table,
                    RecreateForeignKeys = false,
                },
                model,
                builder,
                terminate);

        protected virtual void Generate(
            SingleStoreDropPrimaryKeyAndRecreateForeignKeysOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            void DropPrimaryKey()
            {
                builder.Append($"CALL POMELO_BEFORE_DROP_PRIMARY_KEY({_stringTypeMapping.GenerateSqlLiteral(operation.Schema)}, {_stringTypeMapping.GenerateSqlLiteral(operation.Table)});")
                    .AppendLine()
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                    .Append(" DROP PRIMARY KEY");

                if (terminate)
                {
                    builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                    EndStatement(builder);
                }
            }

            DropPrimaryKey();
        }

        /// <summary>
        ///     Generates a SQL fragment for traits of an index from a <see cref="CreateIndexOperation" />,
        ///     <see cref="AddPrimaryKeyOperation" />, or <see cref="AddUniqueConstraintOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <see langword="null"/> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void IndexTraits(MigrationOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var spatial = operation[SingleStoreAnnotationNames.SpatialIndex] as bool?;
            if (spatial == true)
            {
                builder.Append("SPATIAL ");
            }
        }

        protected override void IndexOptions(CreateIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            // The base implementation supports index filters in form of a WHERE clause.
            // This is not supported by MySQL, so we don't call it here.

            var fullText = operation[SingleStoreAnnotationNames.FullTextIndex] as bool?;
            if (fullText == true)
            {
                var fullTextParser = operation[SingleStoreAnnotationNames.FullTextParser] as string;
                if (!string.IsNullOrEmpty(fullTextParser))
                {
                    // Official MySQL support exists since 5.1, but since MariaDB does not support full-text parsers and does not recognize
                    // the "/*!xxxxx" syntax for versions below 50700, we use 50700 here, even though the statement would work in lower
                    // versions as well. Since we don't support MySQL 5.6 officially anymore, this is fine.
                    builder.Append(" /*!50700 WITH PARSER ")
                        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(fullTextParser))
                        .Append(" */");
                }
            }
        }

        private string ColumnListWithIndexPrefixLength(MigrationOperation operation, string[] columns)
            => operation[SingleStoreAnnotationNames.IndexPrefixLength] is int[] prefixValues
                ? ColumnList(
                    columns,
                    (c, i) => prefixValues.Length > i && prefixValues[i] > 0
                        ? $"({prefixValues[i]})"
                        : null)
                : ColumnList(columns);

        protected virtual string ColumnList([NotNull] string[] columns, Func<string, int, string> columnPostfix)
            => string.Join(", ", columns.Select((c, i) => Dependencies.SqlGenerationHelper.DelimitIdentifier(c) + columnPostfix?.Invoke(c, i)));

        private string IntegerConstant(long value)
            => string.Format(CultureInfo.InvariantCulture, "{0}", value);

        private static string Truncate(string source, int maxLength)
        {
            if (source == null
                || source.Length <= maxLength)
            {
                return source;
            }

            return source.Substring(0, maxLength);
        }

        private static bool IsSpatialStoreType(string storeType)
            => _spatialStoreTypes.Contains(storeType);

        private static bool IsDefaultValueSupported(string columnType)
            => !columnType.Contains("blob", StringComparison.OrdinalIgnoreCase) &&
               !columnType.Contains("text", StringComparison.OrdinalIgnoreCase) &&
               !columnType.Contains("json", StringComparison.OrdinalIgnoreCase) &&
               !IsSpatialStoreType(columnType);
    }
}
