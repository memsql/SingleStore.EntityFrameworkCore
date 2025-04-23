using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Storage
{
    public class SingleStoreTypeMappingTest
    {
        // TODO: Implement.
        /*
        #region Date/Time

        [Fact]
        public void GenerateSqlLiteral_returns_date_literal()
            => Assert.Equal("DATE '2015-03-12'",
                GetMapping("date").GenerateSqlLiteral(new DateTime(2015, 3, 12)));

        [Fact]
        public void GenerateSqlLiteral_returns_timestamp_literal()
        {
            var mapping = GetMapping("timestamp");
            Assert.Equal("TIMESTAMP '1997-12-17 07:37:16'",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Utc)));
            Assert.Equal("TIMESTAMP '1997-12-17 07:37:16'",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Local)));
            Assert.Equal("TIMESTAMP '1997-12-17 07:37:16'",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Unspecified)));
            Assert.Equal("TIMESTAMP '1997-12-17 07:37:16.345'",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, 345)));
            Assert.Equal("TIMESTAMP '9999-12-31 23:59:59.999999'",
                mapping.GenerateSqlLiteral(DateTime.MaxValue));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_timestamptz_datetime_literal()
        {
            var mapping = GetMapping("timestamptz");
            Assert.Equal("TIMESTAMPTZ '1997-12-17 07:37:16 UTC'",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Utc)));
            Assert.Equal("TIMESTAMPTZ '1997-12-17 07:37:16 UTC'",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Unspecified)));

            var offset = TimeZoneInfo.Local.BaseUtcOffset;
            var offsetStr = (offset < TimeSpan.Zero ? '-' : '+') + offset.ToString(@"hh\:mm");
            Assert.StartsWith($"TIMESTAMPTZ '1997-12-17 07:37:16{offsetStr}",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Local)));

            Assert.Equal("TIMESTAMPTZ '1997-12-17 07:37:16.345678 UTC'",
                mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, 345, DateTimeKind.Utc).AddTicks(6780)));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_timestamptz_datetimeoffset_literal()
        {
            var mapping = GetMapping("timestamptz");
            Assert.Equal("TIMESTAMPTZ '1997-12-17 07:37:16+02:00'",
                mapping.GenerateSqlLiteral(new DateTimeOffset(1997, 12, 17, 7, 37, 16, TimeSpan.FromHours(2))));
            Assert.Equal("TIMESTAMPTZ '1997-12-17 07:37:16.345+02:00'",
                mapping.GenerateSqlLiteral(new DateTimeOffset(1997, 12, 17, 7, 37, 16, 345, TimeSpan.FromHours(2))));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_time_literal()
        {
            var mapping = GetMapping("time");
            Assert.Equal("TIME '04:05:06.123456'",
                mapping.GenerateSqlLiteral(new TimeSpan(0, 4, 5, 6, 123).Add(TimeSpan.FromTicks(4560))));
            Assert.Equal("TIME '04:05:06.000123'",
                mapping.GenerateSqlLiteral(new TimeSpan(0, 4, 5, 6).Add(TimeSpan.FromTicks(1230))));
            Assert.Equal("TIME '04:05:06.123'", mapping.GenerateSqlLiteral(new TimeSpan(0, 4, 5, 6, 123)));
            Assert.Equal("TIME '04:05:06'", mapping.GenerateSqlLiteral(new TimeSpan(4, 5, 6)));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_timetz_literal()
        {
            var mapping = GetMapping("timetz");
            Assert.Equal("TIMETZ '04:05:06.123456+3'",
                mapping.GenerateSqlLiteral(new DateTimeOffset(2015, 3, 12, 4, 5, 6, 123, TimeSpan.FromHours(3))
                    .AddTicks(4560)));
            Assert.Equal("TIMETZ '04:05:06.789+3'", mapping.GenerateSqlLiteral(new DateTimeOffset(2015, 3, 12, 4, 5, 6, 789, TimeSpan.FromHours(3))));
            Assert.Equal("TIMETZ '04:05:06-3'", mapping.GenerateSqlLiteral(new DateTimeOffset(2015, 3, 12, 4, 5, 6, TimeSpan.FromHours(-3))));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_interval_literal()
        {
            var mapping = GetMapping("interval");
            Assert.Equal("INTERVAL '3 04:05:06.007008'", mapping.GenerateSqlLiteral(new TimeSpan(3, 4, 5, 6, 7)
                .Add(TimeSpan.FromTicks(80))));
            Assert.Equal("INTERVAL '3 04:05:06.007'", mapping.GenerateSqlLiteral(new TimeSpan(3, 4, 5, 6, 7)));
            Assert.Equal("INTERVAL '3 04:05:06'", mapping.GenerateSqlLiteral(new TimeSpan(3, 4, 5, 6)));
            Assert.Equal("INTERVAL '04:05:06'", mapping.GenerateSqlLiteral(new TimeSpan(4, 5, 6)));
            Assert.Equal("INTERVAL '-3 04:05:06.007'", mapping.GenerateSqlLiteral(new TimeSpan(-3, -4, -5, -6, -7)));
        }

        #endregion Date/Time

        #region Geometric

        [Fact]
        public void GenerateSqlLiteral_returns_point_literal()
            => Assert.Equal("POINT '(3.5,4.5)'", GetMapping("point").GenerateSqlLiteral(new SingleStorePoint(3.5, 4.5)));

        [Fact]
        public void GenerateCodeLiteral_returns_point_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStorePoint(3.5, 4.5)", CodeLiteral(new SingleStorePoint(3.5, 4.5)));

        [Fact]
        public void GenerateSqlLiteral_returns_line_literal()
            => Assert.Equal("LINE '{3.5,4.5,10}'", GetMapping("line").GenerateSqlLiteral(new SingleStoreLine(3.5, 4.5, 10)));

        [Fact]
        public void GenerateCodeLiteral_returns_line_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreLine(3.5, 4.5, 10.0)", CodeLiteral(new SingleStoreLine(3.5, 4.5, 10)));

        [Fact]
        public void GenerateSqlLiteral_returns_lseg_literal()
            => Assert.Equal("LSEG '[(3.5,4.5),(5.5,6.5)]'", GetMapping("lseg").GenerateSqlLiteral(new SingleStoreLSeg(3.5, 4.5, 5.5, 6.5)));

        [Fact]
        public void GenerateCodeLiteral_returns_lseg_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreLSeg(3.5, 4.5, 5.5, 6.5)", CodeLiteral(new SingleStoreLSeg(3.5, 4.5, 5.5, 6.5)));

        [Fact]
        public void GenerateSqlLiteral_returns_box_literal()
            => Assert.Equal("BOX '((2,1),(4,3))'", GetMapping("box").GenerateSqlLiteral(new SingleStoreBox(1, 2, 3, 4)));

        [Fact]
        public void GenerateCodeLiteral_returns_box_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreBox(1.0, 2.0, 3.0, 4.0)", CodeLiteral(new SingleStoreBox(1, 2, 3, 4)));

        [Fact]
        public void GenerateSqlLiteral_returns_path_closed_literal()
            => Assert.Equal("PATH '((1,2),(3,4))'", GetMapping("path").GenerateSqlLiteral(new SingleStorePath(
                new SingleStorePoint(1, 2),
                new SingleStorePoint(3, 4)
            )));

        [Fact]
        public void GenerateCodeLiteral_returns_closed_path_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStorePath(new SingleStorePoint[] { new SingleStoreTypes.SingleStorePoint(1.0, 2.0), new SingleStoreTypes.SingleStorePoint(3.0, 4.0) }, false)", CodeLiteral(new SingleStorePath(
                new SingleStorePoint(1, 2),
                new SingleStorePoint(3, 4)
            )));

        [Fact]
        public void GenerateSqlLiteral_returns_path_open_literal()
            => Assert.Equal("PATH '[(1,2),(3,4)]'", GetMapping("path").GenerateSqlLiteral(new SingleStorePath(
                new SingleStorePoint(1, 2),
                new SingleStorePoint(3, 4)
            ) { Open = true }));

        [Fact]
        public void GenerateCodeLiteral_returns_open_path_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStorePath(new SingleStorePoint[] { new SingleStoreTypes.SingleStorePoint(1.0, 2.0), new SingleStoreTypes.SingleStorePoint(3.0, 4.0) }, true)", CodeLiteral(new SingleStorePath(
                new SingleStorePoint(1, 2),
                new SingleStorePoint(3, 4)
            ) { Open = true }));

        [Fact]
        public void GenerateSqlLiteral_returns_polygon_literal()
            => Assert.Equal("POLYGON '((1,2),(3,4))'", GetMapping("polygon").GenerateSqlLiteral(new SingleStorePolygon(
                new SingleStorePoint(1, 2),
                new SingleStorePoint(3, 4)
            )));

        [Fact]
        public void GenerateCodeLiteral_returns_polygon_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStorePolygon(new SingleStorePoint[] { new SingleStoreTypes.SingleStorePoint(1.0, 2.0), new SingleStoreTypes.SingleStorePoint(3.0, 4.0) })", CodeLiteral(new SingleStorePolygon(
                new SingleStorePoint(1, 2),
                new SingleStorePoint(3, 4)
            )));

        [Fact]
        public void GenerateSqlLiteral_returns_circle_literal()
            => Assert.Equal("CIRCLE '<(3.5,4.5),5.5>'", GetMapping("circle").GenerateSqlLiteral(new SingleStoreCircle(3.5, 4.5, 5.5)));

        [Fact]
        public void GenerateCodeLiteral_returns_circle_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreCircle(3.5, 4.5, 5.5)", CodeLiteral(new SingleStoreCircle(3.5, 4.5, 5.5)));

        #endregion Geometric

        #region Misc

        [Fact]
        public void GenerateSqlLiteral_returns_bool_literal()
            => Assert.Equal("TRUE", GetMapping("bool").GenerateSqlLiteral(true));

        [Fact]
        public void GenerateSqlLiteral_returns_varbit_literal()
            => Assert.Equal("B'10'", GetMapping("varbit").GenerateSqlLiteral(new BitArray(new[] { true, false })));

        [Fact]
        public void GenerateCodeLiteral_returns_varbit_literal()
            => Assert.Equal("new System.Collections.BitArray(new bool[] { true, false })", CodeLiteral(new BitArray(new[] { true, false })));

        [Fact]
        public void GenerateSqlLiteral_returns_bit_literal()
            => Assert.Equal("B'10'", GetMapping("bit").GenerateSqlLiteral(new BitArray(new[] { true, false })));

        [Fact]
        public void GenerateCodeLiteral_returns_bit_literal()
            => Assert.Equal("new System.Collections.BitArray(new bool[] { true, false })", CodeLiteral(new BitArray(new[] { true, false })));

        [Fact]
        public void GenerateSqlLiteral_returns_array_literal()
            => Assert.Equal("ARRAY[3,4]::integer[]", GetMapping(typeof(int[])).GenerateSqlLiteral(new[] { 3, 4 }));

        [Fact]
        public void GenerateSqlLiteral_returns_array_empty_literal()
            => Assert.Equal("ARRAY[]::smallint[]", GetMapping(typeof(short[])).GenerateSqlLiteral(new short[0]));

        [Fact]
        public void ValueComparer_int_array()
        {
            // This exercises array's comparer when the element doesn't have a comparer, but it implements
            // IEquatable<T>
            var source = new[] { 2, 3, 4 };

            var comparer = GetMapping(typeof(int[])).Comparer;
            var snapshot = (int[])comparer.Snapshot(source);
            Assert.Equal(source, snapshot);
            Assert.NotSame(source, snapshot);
            Assert.True(comparer.Equals(source, snapshot));
            snapshot[1] = 8;
            Assert.False(comparer.Equals(source, snapshot));
        }

        [Fact]
        public void ValueComparer_hstore_array()
        {
            // This exercises array's comparer when the element has its own non-null comparer
            var source = new[]
            {
                new Dictionary<string, string> { { "k1", "v1" } },
                new Dictionary<string, string> { { "k2", "v2" } },
            };

            var comparer = GetMapping(typeof(Dictionary<string, string>[])).Comparer;
            var snapshot = (Dictionary<string, string>[])comparer.Snapshot(source);
            Assert.Equal(source, snapshot);
            Assert.NotSame(source, snapshot);
            Assert.True(comparer.Equals(source, snapshot));
            snapshot[1]["k2"] = "v8";
            Assert.False(comparer.Equals(source, snapshot));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_bytea_literal()
            => Assert.Equal(@"BYTEA E'\\xDEADBEEF'", GetMapping("bytea").GenerateSqlLiteral(new byte[] { 222, 173, 190, 239 }));

        [Fact]
        public void GenerateSqlLiteral_returns_hstore_literal()
            => Assert.Equal(@"HSTORE '""k1""=>""v1"",""k2""=>""v2""'",
                GetMapping("hstore").GenerateSqlLiteral(new Dictionary<string, string>
                {
                    { "k1", "v1" },
                    { "k2", "v2" }
                }));

        [Fact]
        public void ValueComparer_hstore()
        {
            var source = new Dictionary<string, string>
            {
                { "k1", "v1" },
                { "k2", "v2" }
            };

            var comparer = GetMapping("hstore").Comparer;
            var snapshot = (Dictionary<string, string>)comparer.Snapshot(source);
            Assert.Equal(source, snapshot);
            Assert.NotSame(source, snapshot);
            Assert.True(comparer.Equals(source, snapshot));
            snapshot.Remove("k1");
            Assert.False(comparer.Equals(source, snapshot));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_enum_literal()
        {
            var mapping = new SingleStoreEnumTypeMapping(
                "dummy_enum",
                null,
                typeof(DummyEnum),
                new SingleStoreSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
                new SingleStoreSnakeCaseNameTranslator());

            Assert.Equal("'sad'::dummy_enum", mapping.GenerateSqlLiteral(DummyEnum.Sad));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_enum_uppercase_literal()
        {
            var mapping = new SingleStoreEnumTypeMapping(
                "DummyEnum",
                null,
                typeof(DummyEnum),
                new SingleStoreSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
                new SingleStoreSnakeCaseNameTranslator());

            Assert.Equal(@"'sad'::""DummyEnum""", mapping.GenerateSqlLiteral(DummyEnum.Sad));
        }

        private enum DummyEnum
        {
            // ReSharper disable once UnusedMember.Local
            Happy,
            Sad
        };

        [Fact]
        public void GenerateSqlLiteral_returns_tid_literal()
            => Assert.Equal(@"TID '(0,1)'", GetMapping("tid").GenerateSqlLiteral(new SingleStoreTid(0, 1)));

        #endregion Misc

        #region Ranges

        [Fact]
        public void GenerateSqlLiteral_returns_range_empty_literal()
        {
            var value = SingleStoreRange<int>.Empty;
            var literal = GetMapping("int4range").GenerateSqlLiteral(value);
            Assert.Equal("'empty'::int4range", literal);
        }

        [Fact]
        public void GenerateCodeLiteral_returns_range_empty_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreRange<int>(0, false, 0, false)", CodeLiteral(SingleStoreRange<int>.Empty));

        [Fact]
        public void GenerateSqlLiteral_returns_range_inclusive_literal()
        {
            var value = new SingleStoreRange<int>(4, 7);
            var literal = GetMapping("int4range").GenerateSqlLiteral(value);
            Assert.Equal("'[4,7]'::int4range", literal);
        }

        [Fact]
        public void GenerateCodeLiteral_returns_range_inclusive_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreRange<int>(4, true, 7, true)", CodeLiteral(new SingleStoreRange<int>(4, 7)));

        [Fact]
        public void GenerateSqlLiteral_returns_range_inclusive_exclusive_literal()
        {
            var value = new SingleStoreRange<int>(4, false, 7, true);
            var literal = GetMapping("int4range").GenerateSqlLiteral(value);
            Assert.Equal("'(4,7]'::int4range", literal);
        }

        [Fact]
        public void GenerateCodeLiteral_returns_range_inclusive_exclusive_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreRange<int>(4, false, 7, true)", CodeLiteral(new SingleStoreRange<int>(4, false, 7, true)));

        [Fact]
        public void GenerateSqlLiteral_returns_range_infinite_literal()
        {
            var value = new SingleStoreRange<int>(0, false, true, 7, true, false);
            var literal = GetMapping("int4range").GenerateSqlLiteral(value);
            Assert.Equal("'(,7]'::int4range", literal);
        }

        [Fact]
        public void GenerateCodeLiteral_returns_range_infinite_literal()
            => Assert.Equal("new SingleStoreTypes.SingleStoreRange<int>(0, false, true, 7, true, false)", CodeLiteral(new SingleStoreRange<int>(0, false, true, 7, true, false)));

        #endregion Ranges
*/
        [Fact]
        public void Bool_with_SingleStoreBooleanType_None_maps_to_null()
        {
            // TODO: EF Core 5
            // If SingleStoreBooleanType.None is used, the intention was to not map System.Boolean at all, but EF Core
            // successfully substitutes System.Boolean with System.Int32 and then maps it to INT.
            // So for EF Core 5, we should remove the SingleStoreBooleanType.None option altogether.
            var options = GetOptions(b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.None)));
            var mapping = GetMapping(typeof(bool), options);

            Assert.Equal("int", mapping.StoreType);
        }

        [Fact]
        public void Bool_with_SingleStoreBooleanType_Default_maps_to_tinyint1()
        {
            var options = GetOptions(b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.Default)));
            var mapping = GetMapping(typeof(bool), options);

            Assert.Equal("tinyint(1)", mapping.StoreType);
        }

        [Fact]
        public void Bool_with_SingleStoreBooleanType_TinyInt1_maps_to_tinyint1()
        {
            var options = GetOptions(b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.TinyInt1)));
            var mapping = GetMapping(typeof(bool), options);

            Assert.Equal("tinyint(1)", mapping.StoreType);
        }

        [Fact]
        public void Bool_with_SingleStoreBooleanType_Bit1_maps_to_bit1()
        {
            var options = GetOptions(b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.Bit1)));
            var mapping = GetMapping(typeof(bool), options);

            Assert.Equal("bit(1)", mapping.StoreType);
        }

        [Fact]
        public void Bit1_with_SingleStoreBooleanType_Default_maps_to_UInt64()
        {
            var options = GetOptions(b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(SingleStoreBooleanType.Default)));
            var mapping = GetMapping("bit(1)", options);

            Assert.Equal(typeof(ulong), mapping.ClrType);
        }

        #region Support

        private static SingleStoreTypeMappingSource GetMapper(SingleStoreOptions options = null)
            => new SingleStoreTypeMappingSource(
                new TypeMappingSourceDependencies(
                    new ValueConverterSelector(new ValueConverterSelectorDependencies()),
                    new JsonValueReaderWriterSource(new JsonValueReaderWriterSourceDependencies()),
                    Array.Empty<ITypeMappingSourcePlugin>()),
                new RelationalTypeMappingSourceDependencies(
                    Array.Empty<IRelationalTypeMappingSourcePlugin>()),
                options ?? new SingleStoreOptions());

        private static RelationalTypeMapping GetMapping(string storeType, SingleStoreOptions options = null)
            => GetMapper(options).FindMapping(storeType);

        private static RelationalTypeMapping GetMapping(Type clrType, SingleStoreOptions options = null)
            => (RelationalTypeMapping)GetMapper(options).FindMapping(clrType);

        private static CSharpHelper GetCsHelper(SingleStoreOptions options = null)
            => new CSharpHelper(GetMapper(options));

        private static string CodeLiteral(object value, SingleStoreOptions options = null)
            => GetCsHelper(options).UnknownLiteral(value);

        private static SingleStoreOptions GetOptions(Action<SingleStoreDbContextOptionsBuilder> builder)
        {
            var mySqlOptions = new SingleStoreOptions();
            mySqlOptions.Initialize(
                new DbContextOptionsBuilder()
                    .UseSingleStore("Server=foo", builder)
                    .Options);

            return mySqlOptions;
        }

        #endregion Support
    }
}
