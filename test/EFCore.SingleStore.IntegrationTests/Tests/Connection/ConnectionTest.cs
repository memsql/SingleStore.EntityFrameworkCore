using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.IntegrationTests.Models;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.IntegrationTests.Tests.Connection
{
    public class ConnectionTest
    {
        private static readonly SingleStoreConnection Connection = new SingleStoreConnection(AppConfig.Config["Data:ConnectionString"]);

        private static AppDbScope NewDbScope(bool reuseConnection)
        {
            return reuseConnection ? new AppDbScope(Connection) : new AppDbScope();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AffectedRowsFalse(bool reuseConnection)
        {
            var title = "test";
            var blog = new Blog {Title = title};
            using (var scope = NewDbScope(reuseConnection))
            {
                var db = scope.AppDb;
                db.Blogs.Add(blog);
                await db.SaveChangesAsync();
            }
            Assert.True(blog.Id > 0);

            // this will throw a DbUpdateConcurrencyException if UseAffectedRows=true
            var sameBlog = new Blog {Id = blog.Id, Title = title};
            using (var scope = NewDbScope(reuseConnection))
            {
                var db = scope.AppDb;
                db.Blogs.Update(sameBlog);
                await db.SaveChangesAsync();
            }
            Assert.Equal(blog.Id, sameBlog.Id);
        }
    }
}
