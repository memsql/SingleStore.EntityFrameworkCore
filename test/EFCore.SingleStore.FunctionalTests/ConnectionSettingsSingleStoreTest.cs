using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class ConnectionSettingsSingleStoreTest
    {
        [ConditionalTheory]
        [InlineData(SingleStoreGuidFormat.Char36, "'850368d8-93ea-4023-acc7-6fa6e4c3b27f'", null)]
        [InlineData(SingleStoreGuidFormat.Char32, "'850368d893ea4023acc76fa6e4c3b27f'", null)]
        [InlineData(SingleStoreGuidFormat.Binary16, "X'850368D893EA4023ACC76FA6E4C3B27F'", null)]
        [InlineData(SingleStoreGuidFormat.TimeSwapBinary16, "X'402393EA850368D8ACC76FA6E4C3B27F'", null)]
        [InlineData(SingleStoreGuidFormat.LittleEndianBinary16, "X'D8680385EA932340ACC76FA6E4C3B27F'", null)]
        [InlineData(SingleStoreGuidFormat.None, "'850368d8-93ea-4023-acc7-6fa6e4c3b27f'", null)]
        public virtual void Insert_and_read_Guid_value(SingleStoreGuidFormat guidFormat, string sqlEquivalent, string supportedServerVersion)
        {
            if (supportedServerVersion != null &&
                !AppConfig.ServerVersion.Supports.Version(ServerVersion.Parse(supportedServerVersion)))
            {
                return;
            }

            using var context = CreateContext(guidFormat);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.SimpleGuidEntities.Add(new SimpleGuidEntity { GuidValue = new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F") });
            context.SaveChanges();

            var result = context.SimpleGuidEntities
                .Where(e => e.GuidValue == new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F"))
                .ToList();

            var sqlResult = context.SimpleGuidEntities
                .FromSqlRaw("select * from `SimpleGuidEntities` where `GuidValue` = " + sqlEquivalent)
                .ToList();

            Assert.Single(result);
            Assert.Equal(new Guid("850368D8-93EA-4023-ACC7-6FA6E4C3B27F"), result[0].GuidValue);
            Assert.Single(sqlResult);
        }

        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkSingleStore()
            .BuildServiceProvider();

        protected ConnectionSettingsContext CreateContext(SingleStoreGuidFormat guidFormat)
            => new ConnectionSettingsContext(_serviceProvider, "ConnectionSettings", guidFormat);
    }

    public class ConnectionSettingsContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _databaseName;
        private readonly SingleStoreGuidFormat _guidFormat;

        public ConnectionSettingsContext(IServiceProvider serviceProvider, string databaseName, SingleStoreGuidFormat guidFormat)
        {
            _serviceProvider = serviceProvider;
            _databaseName = databaseName;
            _guidFormat = guidFormat;
        }

        public DbSet<SimpleGuidEntity> SimpleGuidEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSingleStore(SingleStoreTestStore.CreateConnectionString(_databaseName, false, _guidFormat))
                .UseInternalServiceProvider(_serviceProvider);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SimpleGuidEntity>();

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<SimpleGuidEntity>()
                .Property(e => e.SimpleGuidEntityId)
                .HasColumnType("bigint");
        }
    }

    public class SimpleGuidEntity
    {
        public int SimpleGuidEntityId { get; set; }
        public Guid GuidValue { get; set; }
    }
}
