using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class AdHocManyToManyQuerySingleStoreTest : AdHocManyToManyQueryRelationalTestBase
{
    [ConditionalFact]
    public override async Task SelectMany_with_collection_selector_having_subquery()
    {
        var contextFactory = await InitializeAsync<MyContext7973>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<MyContext7973.User>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<MyContext7973.Organisation>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<MyContext7973.OrganisationUser>()
                .Property(e => e.OrganisationId)
                .HasColumnType("bigint");
            modelBuilder.Entity<MyContext7973.OrganisationUser>()
                .Property(e => e.UserId)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var users = (from user in context.Users
                     from organisation in context.Organisations.Where(o => o.OrganisationUsers.Any()).DefaultIfEmpty()
                     select new { UserId = user.Id, OrgId = organisation.Id }).ToList();

        Assert.Equal(2, users.Count);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Many_to_many_load_works_when_join_entity_has_custom_key(bool async)
    {
        var contextFactory = await InitializeAsync<Context20277>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<ManyM_DB>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<ManyN_DB>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<ManyMN_DB>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<ManyMN_DB>()
                .Property(e => e.ManyM_Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<ManyMN_DB>()
                .Property(e => e.ManyN_Id)
                .HasColumnType("bigint");
        });

        int id;
        using (var context = contextFactory.CreateContext())
        {
            var m = new ManyM_DB();
            var n = new ManyN_DB();
            context.AddRange(m, n);
            m.ManyN_DB = new List<ManyN_DB> { n };

            context.SaveChanges();

            id = m.Id;
        }

        ClearLog();

        using (var context = contextFactory.CreateContext())
        {
            var m = context.Find<ManyM_DB>(id);

            if (async)
            {
                await context.Entry(m).Collection(x => x.ManyN_DB).LoadAsync();
            }
            else
            {
                context.Entry(m).Collection(x => x.ManyN_DB).Load();
            }

            Assert.Equal(3, context.ChangeTracker.Entries().Count());
            Assert.Equal(1, m.ManyN_DB.Count);
            Assert.Equal(1, m.ManyN_DB.Single().ManyM_DB.Count);
            Assert.Equal(1, m.ManyNM_DB.Count);
            Assert.Equal(1, m.ManyN_DB.Single().ManyNM_DB.Count);

            id = m.ManyN_DB.Single().Id;
        }

        using (var context = contextFactory.CreateContext())
        {
            var n = context.Find<ManyN_DB>(id);

            if (async)
            {
                await context.Entry(n).Collection(x => x.ManyM_DB).LoadAsync();
            }
            else
            {
                context.Entry(n).Collection(x => x.ManyM_DB).Load();
            }

            Assert.Equal(3, context.ChangeTracker.Entries().Count());
            Assert.Equal(1, n.ManyM_DB.Count);
            Assert.Equal(1, n.ManyM_DB.Single().ManyN_DB.Count);
            Assert.Equal(1, n.ManyNM_DB.Count);
            Assert.Equal(1, n.ManyM_DB.Single().ManyNM_DB.Count);
        }
    }

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
