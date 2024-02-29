using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class PropertyValuesSingleStoreTest : PropertyValuesTestBase<PropertyValuesSingleStoreTest.PropertyValuesSingleStoreFixture>
    {
        public PropertyValuesSingleStoreTest(PropertyValuesSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class PropertyValuesSingleStoreFixture : PropertyValuesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                modelBuilder.Entity<Employee>(
                    b =>
                    {
                        b.Property(e => e.EmployeeId).ValueGeneratedNever();
                        b.Property<int>("Shadow1");
                        b.Property<string>("Shadow2");
                    });

                modelBuilder.Entity<CurrentEmployee>(b => b.Property<int>("Shadow3"));

                modelBuilder.Entity<PastEmployee>(b => b.Property<string>("Shadow4"));

                modelBuilder.Entity<Building>()
                    .HasOne<MailRoom>(nameof(Building.PrincipalMailRoom))
                    .WithMany()
                    .HasForeignKey(b => b.PrincipalMailRoomId);

                modelBuilder.Entity<MailRoom>()
                    .HasOne<Building>(nameof(MailRoom.Building))
                    .WithMany(nameof(Building.MailRooms))
                    .HasForeignKey(m => m.BuildingId);

                modelBuilder.Entity<Office>().HasKey(
                    o => new { o.Number, o.BuildingId });

                modelBuilder.Ignore<UnMappedOffice>();

                modelBuilder.Entity<BuildingDetail>(
                    b =>
                    {
                        b.HasKey(d => d.BuildingId);
                        b.HasOne(d => d.Building).WithOne().HasPrincipalKey<Building>(e => e.BuildingId);
                    });

                modelBuilder.Entity<Building>(
                    b =>
                    {
                        b.Ignore(e => e.NotInModel);
                        b.Property<int>("Shadow1");
                        b.Property<string>("Shadow2");
                    });

            }
        }
    }
}
