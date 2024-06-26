﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class TPTGearsOfWarQuerySingleStoreFixture : TPTGearsOfWarQueryRelationalFixture, IQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);

            new SingleStoreDbContextOptionsBuilder(optionsBuilder)
                .EnableIndexOptimizedBooleanColumns(true);

            return optionsBuilder;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<Weapon>().HasIndex(e => e.IsAutomatic);
        }

        public new ISetSource GetExpectedData()
        {
            var data = (GearsOfWarData)base.GetExpectedData();

            foreach (var mission in data.Missions)
            {
                mission.Timeline = GetExpectedValue(mission.Timeline);
            }

            return data;
        }

        public static DateTimeOffset GetExpectedValue(DateTimeOffset value)
        {
            const int mySqlMaxMillisecondDecimalPlaces = 6;
            var decimalPlacesFactor = (decimal)Math.Pow(10, 7 - mySqlMaxMillisecondDecimalPlaces);

            // Change DateTimeOffset values, because MySQL does not preserve offsets and has a maximum of 6 decimal places, in contrast to
            // .NET which has 7.
            return new DateTimeOffset(
                (long)(Math.Truncate(value.UtcTicks / decimalPlacesFactor) * decimalPlacesFactor),
                TimeSpan.Zero);
        }
    }
}
