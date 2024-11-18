// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Migrations.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Update.Internal;
using EntityFrameworkCore.SingleStore.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Metadata.Internal;
using EntityFrameworkCore.SingleStore.Migrations;
using EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;
using EntityFrameworkCore.SingleStore.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SingleStoreServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Registers the given Entity Framework context as a service in the <see cref="IServiceCollection" />
        ///         and configures it to connect to a MySQL compatible database.
        ///     </para>
        ///     <para>
        ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
        ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
        ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
        ///         overridden to configure the EntityFrameworkCore.SingleStore provider and connection string.
        ///     </para>
        ///     <para>
        ///         To configure the <see cref="DbContextOptions{TContext}" /> for the context, either override the
        ///         <see cref="DbContext.OnConfiguring" /> method in your derived context, or supply
        ///         an optional action to configure the <see cref="DbContextOptions" /> for the context.
        ///     </para>
        ///     <para>
        ///         For more information on how to use this method, see the Entity Framework Core documentation at https://aka.ms/efdocs.
        ///         For more information on using dependency injection, see https://go.microsoft.com/fwlink/?LinkId=526890.
        ///     </para>
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be registered. </typeparam>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="serverVersion">
        ///     <para>
        ///         The version of the database server.
        ///     </para>
        ///     <para>
        ///         Create an object for this parameter by calling the static method
        ///         <see cref="ServerVersion.Create(System.Version,ServerType)"/>,
        ///         by calling the static method <see cref="ServerVersion.AutoDetect(string)"/> (which retrieves the server version directly
        ///         from the database server),
        ///         by parsing a version string using the static methods
        ///         <see cref="ServerVersion.Parse(string)"/> or <see cref="ServerVersion.TryParse(string,out ServerVersion)"/>,
        ///         or by directly instantiating an object from the <see cref="SingleStoreServerVersion"/> class.
        ///      </para>
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <param name="optionsAction"> An optional action to configure the <see cref="DbContextOptions" /> for the context. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddSingleStore<TContext>(
            this IServiceCollection serviceCollection,
            string connectionString,
            ServerVersion serverVersion,
            Action<SingleStoreDbContextOptionsBuilder> mySqlOptionsAction = null,
            Action<DbContextOptionsBuilder> optionsAction = null)
            where TContext : DbContext
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            return serviceCollection.AddDbContext<TContext>((_, options) =>
            {
                optionsAction?.Invoke(options);
                options.UseSingleStore(connectionString, mySqlOptionsAction);
            });
        }

        public static IServiceCollection AddEntityFrameworkSingleStore([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, SingleStoreLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<SingleStoreOptionsExtension>>()
                //.TryAdd<IValueGeneratorCache>(p => p.GetService<ISingleStoreValueGeneratorCache>())
                .TryAdd<IRelationalTypeMappingSource, SingleStoreTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, SingleStoreSqlGenerationHelper>()
                .TryAdd<IRelationalAnnotationProvider, SingleStoreAnnotationProvider>()
                .TryAdd<IModelValidator, SingleStoreModelValidator>()
                .TryAdd<IProviderConventionSetBuilder, SingleStoreConventionSetBuilder>()
                //.TryAdd<IRelationalValueBufferFactoryFactory, TypedRelationalValueBufferFactoryFactory>() // What is that?
                .TryAdd<IUpdateSqlGenerator, SingleStoreUpdateSqlGenerator>()
                .TryAdd<IModificationCommandFactory, SingleStoreModificationCommandFactory>()
                .TryAdd<IModificationCommandBatchFactory, SingleStoreModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, SingleStoreValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<ISingleStoreRelationalConnection>())
                .TryAdd<IMigrationsSqlGenerator, SingleStoreMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, SingleStoreDatabaseCreator>()
                .TryAdd<IHistoryRepository, SingleStoreHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, SingleStoreCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, SingleStoreExecutionStrategyFactory>()
                .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, SingleStoreQueryableMethodTranslatingExpressionVisitorFactory>()
                .TryAdd<IRelationalQueryStringFactory, SingleStoreQueryStringFactory>()
                .TryAdd<IMethodCallTranslatorProvider, SingleStoreMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, SingleStoreMemberTranslatorProvider>()
                .TryAdd<IEvaluatableExpressionFilter, SingleStoreEvaluatableExpressionFilter>()
                .TryAdd<IQuerySqlGeneratorFactory, SingleStoreQuerySqlGeneratorFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, SingleStoreSqlTranslatingExpressionVisitorFactory>()
                .TryAdd<IRelationalParameterBasedSqlProcessorFactory, SingleStoreParameterBasedSqlProcessorFactory>()
                .TryAdd<ISqlExpressionFactory, SingleStoreSqlExpressionFactory>()
                .TryAdd<ISingletonOptions, ISingleStoreOptions>(p => p.GetService<ISingleStoreOptions>())
                //.TryAdd<IValueConverterSelector, SingleStoreValueConverterSelector>()
                .TryAdd<IQueryCompilationContextFactory, SingleStoreQueryCompilationContextFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, SingleStoreQueryTranslationPostprocessorFactory>()
                .TryAdd<IMigrationsModelDiffer, SingleStoreMigrationsModelDiffer>()
                .TryAdd<IMigrator, SingleStoreMigrator>()
                .TryAddProviderSpecificServices(m => m
                    //.TryAddSingleton<ISingleStoreValueGeneratorCache, SingleStoreValueGeneratorCache>()
                    .TryAddSingleton<ISingleStoreOptions, SingleStoreOptions>()
                    //.TryAddScoped<ISingleStoreSequenceValueGeneratorFactory, SingleStoreSequenceValueGeneratorFactory>()
                    .TryAddScoped<ISingleStoreUpdateSqlGenerator, SingleStoreUpdateSqlGenerator>()
                    .TryAddScoped<ISingleStoreRelationalConnection, SingleStoreRelationalConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
