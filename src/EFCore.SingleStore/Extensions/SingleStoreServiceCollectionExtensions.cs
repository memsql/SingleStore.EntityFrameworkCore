// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Migrations.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Update.Internal;
using EntityFrameworkCore.SingleStore.ValueGeneration.Internal;
using JetBrains.Annotations;
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
using EntityFrameworkCore.SingleStore.Metadata.Internal;
using EntityFrameworkCore.SingleStore.Migrations;
using EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SingleStoreServiceCollectionExtensions
    {
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
                .TryAdd<IModificationCommandBatchFactory, SingleStoreModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, SingleStoreValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<ISingleStoreRelationalConnection>())
                .TryAdd<IMigrationsSqlGenerator, SingleStoreMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, SingleStoreDatabaseCreator>()
                .TryAdd<IHistoryRepository, SingleStoreHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, SingleStoreCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, SingleStoreExecutionStrategyFactory>()
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
