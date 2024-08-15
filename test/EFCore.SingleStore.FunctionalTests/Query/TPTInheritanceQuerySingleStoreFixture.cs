﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class TPTInheritanceQuerySingleStoreFixture : TPTInheritanceQueryFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Country>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Drink>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Animal>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Coke>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Lilt>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Tea>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Bird>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Eagle>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Kiwi>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        }
    }
}