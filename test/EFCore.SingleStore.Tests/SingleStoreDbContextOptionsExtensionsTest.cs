using System;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore
{
    public class SingleStoreDbContextOptionsBuilderExtensionsTest
    {
        [Fact]
        public void Multiple_UseSingleStore_calls_each_get_fully_applied()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "Server=first;",
                options =>
                    options.DefaultDataTypeMappings(
                        mappings =>
                            mappings.WithClrBoolean(SingleStoreBooleanType.Bit1)));

            builder.UseSingleStore(
                "Server=second;",
                options =>
                    options.DefaultDataTypeMappings(
                        mappings =>
                            mappings.WithClrBoolean(SingleStoreBooleanType.TinyInt1)));

            var mySqlOptionsExtension = builder.Options.GetExtension<SingleStoreOptionsExtension>();
            Assert.StartsWith("Server=second;", mySqlOptionsExtension.ConnectionString, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(SingleStoreBooleanType.TinyInt1, mySqlOptionsExtension.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_true()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore("TreatTinyAsBoolean=True");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(SingleStoreBooleanType.TinyInt1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_false()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore("TreatTinyAsBoolean=False");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(SingleStoreBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_unspecified()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore("Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(SingleStoreBooleanType.Default, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void Explicit_DefaultDataTypeMappings_take_precedence_over_TreatTinyAsBoolean_true()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "TreatTinyAsBoolean=True",
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.Bit1)));

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(SingleStoreBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void Explicit_DefaultDataTypeMappings_take_precedence_over_TreatTinyAsBoolean_false()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "TreatTinyAsBoolean=False",
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.TinyInt1)));

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(SingleStoreBooleanType.TinyInt1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_SingleStoreServerVersion_Version()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 0), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.SingleStore, mySqlOptions.ServerVersion.Type);
            Assert.Equal("singlestore", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_SingleStoreServerVersion_string_version_only()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 0), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.SingleStore, mySqlOptions.ServerVersion.Type);
            Assert.Equal("singlestore", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_SingleStoreServerVersion_string_version_full()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 0), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.SingleStore, mySqlOptions.ServerVersion.Type);
            Assert.Equal("singlestore", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_SingleStoreServerVersion_ServerVersion()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 0), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.SingleStore, mySqlOptions.ServerVersion.Type);
            Assert.Equal("singlestore", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_SingleStoreServerVersion_incorrect_ServerVersion_throws()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    var builder = new DbContextOptionsBuilder();

                    builder.UseSingleStore(
                        "Server=foo");
                });
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_SingleStoreServerVersion_LatestSupportedServerVersion()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                "Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(SingleStoreServerVersion.LatestSupportedServerVersion.Version, mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.SingleStore, mySqlOptions.ServerVersion.Type);
            Assert.Equal("singlestore", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_ServerVersion_FromString()
        {
            var builder = new DbContextOptionsBuilder();
            var serverVersion = ServerVersion.Parse("7.8.0-singlestore");

            builder.UseSingleStore(
                "Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(7, 8, 0), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.SingleStore, mySqlOptions.ServerVersion.Type);
            Assert.Equal("singlestore", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [ConditionalFact(Skip="ServerVersion parameter isn't needed anymore to call UseSingleStore()")]
        public void UseSingleStore_with_ServerVersion_AutoDetect()
        {
            var builder = new DbContextOptionsBuilder();
            var serverVersion = ServerVersion.AutoDetect(AppConfig.ConnectionString);

            builder.UseSingleStore(
                "Server=foo");

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(serverVersion.Version, mySqlOptions.ServerVersion.Version);
            Assert.Equal(serverVersion.Type, mySqlOptions.ServerVersion.Type);
            Assert.Equal(serverVersion.TypeIdentifier, mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [ConditionalFact(Skip="Connection string/connection is a mandatory parameter for UseSingleStore() call")]
        public void UseSingleStore_without_connection_string()
        {
            var builder = new DbContextOptionsBuilder();
            var serverVersion = ServerVersion.AutoDetect(AppConfig.ConnectionString);

            builder.UseSingleStore();

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);
        }

        [ConditionalFact(Skip="Connection string/connection is a mandatory parameter for UseSingleStore() call")]
        public void UseSingleStore_without_connection_explicit_DefaultDataTypeMappings_is_applied()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSingleStore(
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.Bit1)));

            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(SingleStoreBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }
    }
}
