using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using SingleStoreConnector;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class SqlExecutorSingleStoreTest : SqlExecutorTestBase<NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public SqlExecutorSingleStoreTest(NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public override void Query_with_parameters()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}", city, contactTitle);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_dbParameter_with_name()
        {
            var city = CreateDbParameter("@city", "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = @city", city);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_positional_dbParameter_with_name()
        {
            var city = CreateDbParameter("@city", "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0}", city);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_positional_dbParameter_without_name()
        {
            var city = CreateDbParameter(name: null, value: "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0}", city);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_dbParameters_mixed()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            var cityParameter = CreateDbParameter("@city", city);
            var contactTitleParameter = CreateDbParameter("@contactTitle", contactTitle);

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = @contactTitle", city, contactTitleParameter);

                Assert.Equal(-1, actual);

                actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = @city AND `ContactTitle` = {1}", cityParameter, contactTitle);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_parameters_interpolated()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlInterpolated(
                        $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

                Assert.Equal(-1, actual);
            }
        }

        [ConditionalFact]
        public override async Task Query_with_parameters_async()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Database
                    .ExecuteSqlRawAsync(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}", city, contactTitle);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override async Task Query_with_parameters_interpolated_async()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Database
                    .ExecuteSqlInterpolatedAsync(
                        $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

                Assert.Equal(-1, actual);
            }
        }

        public override void Query_with_DbParameters_interpolated()
        {
            var city = CreateDbParameter("city", "London");
            var contactTitle = CreateDbParameter("contactTitle", "Sales Representative");

            using var context = CreateContext();
            var actual = context.Database
                .ExecuteSqlInterpolated(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(-1, actual);
        }

        [ConditionalFact]
        public override void Executes_stored_procedure()
        {
            using var context = CreateContext();
            Assert.Equal(0, context.Database.ExecuteSqlRaw(TenMostExpensiveProductsSproc));
        }

        [ConditionalFact]
        public override void Executes_stored_procedure_with_parameter()
        {
            using var context = CreateContext();
            var parameter = CreateDbParameter("@CustomerID", "ALFKI");

            Assert.Equal(0, context.Database.ExecuteSqlRaw(CustomerOrderHistorySproc, parameter));
        }

        [ConditionalFact]
        public override void Executes_stored_procedure_with_generated_parameter()
        {
            using var context = CreateContext();
            Assert.Equal(0, context.Database.ExecuteSqlRaw(CustomerOrderHistoryWithGeneratedParameterSproc, "ALFKI"));
        }

        [ConditionalFact]
        public override async Task Executes_stored_procedure_async()
        {
            using var context = CreateContext();
            Assert.Equal(0, await context.Database.ExecuteSqlRawAsync(TenMostExpensiveProductsSproc));
        }

        [ConditionalFact]
        public override async Task Executes_stored_procedure_with_parameter_async()
        {
            using var context = CreateContext();
            var parameter = CreateDbParameter("@CustomerID", "ALFKI");

            Assert.Equal(0, await context.Database.ExecuteSqlRawAsync(CustomerOrderHistorySproc, parameter));
        }

        [ConditionalFact]
        public override async Task Executes_stored_procedure_with_generated_parameter_async()
        {
            using var context = CreateContext();
            Assert.Equal(0, await context.Database.ExecuteSqlRawAsync(CustomerOrderHistoryWithGeneratedParameterSproc, "ALFKI"));
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new SingleStoreParameter
            {
                ParameterName = name,
                Value = value
            };

        protected override string TenMostExpensiveProductsSproc => @"CALL `Ten Most Expensive Products`()";

        protected override string CustomerOrderHistorySproc => @"CALL `CustOrderHist`(@CustomerID)";

        protected override string CustomerOrderHistoryWithGeneratedParameterSproc => @"CALL `CustOrderHist`({0})";
    }
}
