using System;
using Microsoft.EntityFrameworkCore;
using SingleStoreConnector;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.Behaviors;

// See https://bugs.mysql.com/bug.php?id=103961.
public class HavingBehavior : RawSqlTestWithFixture<HavingBehavior.HavingBehaviorFixture>
{
    public HavingBehavior(HavingBehaviorFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
    }

    [Fact]
    public void Having_without_aggregate_with_function_is_allowed_by_default()
    {
        using var command = Connection.CreateCommand();
        command.CommandText = """
SELECT EXTRACT(year FROM `i`.`BestServedBefore`) AS `Year`, COUNT(*) AS `Count`
FROM `IceCreams` AS `i`
GROUP BY EXTRACT(year FROM `i`.`BestServedBefore`)
HAVING EXTRACT(year FROM `i`.`BestServedBefore`) < 2030
ORDER BY COUNT(*) DESC
""";

        using var reader = command.ExecuteReader();

        Assert.True(reader.Read());
        Assert.Equal(2025, Convert.ToInt32(reader["Year"]));
        Assert.Equal(2L, Convert.ToInt64(reader["Count"]));
        Assert.False(reader.Read());
    }

    [Fact]
    public void Having_without_aggregate_with_function_using_projection_reference()
    {
        using var command = Connection.CreateCommand();
        command.CommandText = """
SELECT EXTRACT(year FROM `i`.`BestServedBefore`) AS `Year`, COUNT(*) AS `Count`
FROM `IceCreams` AS `i`
GROUP BY EXTRACT(year FROM `i`.`BestServedBefore`)
HAVING `Year` < 2030
ORDER BY COUNT(*) DESC
""";

        using var dataReader = command.ExecuteReader();

        Assert.True(dataReader.Read());
        Assert.Equal(2025, Convert.ToInt32(dataReader["Year"]));
        Assert.Equal(2L, Convert.ToInt64(dataReader["Count"]));
        Assert.False(dataReader.Read());
    }

    [Fact]
    public void Having_without_aggregate_with_column_is_allowed_by_default()
    {
        using var command = Connection.CreateCommand();
        command.CommandText = """
SELECT EXTRACT(year FROM `i`.`BestServedBefore`) AS `Year`, COUNT(*) AS `Count`
FROM `IceCreams` AS `i`
GROUP BY EXTRACT(year FROM `i`.`BestServedBefore`)
HAVING `i`.`BestServedBefore` < '2030-01-01'
ORDER BY COUNT(*) DESC
""";

        using var reader = command.ExecuteReader();

        Assert.True(reader.Read());
        Assert.Equal(2025, Convert.ToInt32(reader["Year"]));
        Assert.Equal(2L, Convert.ToInt64(reader["Count"]));
        Assert.False(reader.Read());
    }

    [Fact]
    public void Having_without_aggregate_with_column_using_projection_reference()
    {
        using var command = Connection.CreateCommand();
        command.CommandText = """
SELECT EXTRACT(year FROM `i`.`BestServedBefore`) AS `Year`, COUNT(*) AS `Count`, `i`.`BestServedBefore` < '2030-01-01' AS `having`
FROM `IceCreams` AS `i`
GROUP BY EXTRACT(year FROM `i`.`BestServedBefore`), `i`.`BestServedBefore` < '2030-01-01'
HAVING `having`
ORDER BY COUNT(*) DESC
""";

        using var dataReader = command.ExecuteReader();

        Assert.True(dataReader.Read());
        Assert.Equal(2025, Convert.ToInt32(dataReader["Year"]));
        Assert.Equal(2L, Convert.ToInt64(dataReader["Count"]));
        Assert.False(dataReader.Read());
    }

    [Fact]
    public void Having_without_aggregate_with_constant()
    {
        using var command = Connection.CreateCommand();
        command.CommandText = """
SELECT EXTRACT(year FROM `i`.`BestServedBefore`) AS `Year`, COUNT(*) AS `Count`
FROM `IceCreams` AS `i`
GROUP BY EXTRACT(year FROM `i`.`BestServedBefore`)
HAVING TRUE
ORDER BY COUNT(*) DESC
""";

        using var dataReader = command.ExecuteReader();

        Assert.True(dataReader.Read());
        Assert.Equal(2025, Convert.ToInt32(dataReader["Year"]));
        Assert.Equal(2L, Convert.ToInt64(dataReader["Count"]));
        Assert.True(dataReader.Read());
        Assert.Equal(2036, Convert.ToInt32(dataReader["Year"]));
        Assert.Equal(1L, Convert.ToInt64(dataReader["Count"]));
        Assert.False(dataReader.Read());
    }

    public static class Model
    {
        public class IceCream
        {
            public int IceCreamId { get; set; }
            public string Name { get; set; }
            public DateTime BestServedBefore { get; set; }
        }

        public class HavingBehaviorContext : ContextBase
        {
            public DbSet<IceCream> IceCreams { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<IceCream>(
                    entity =>
                    {
                        entity.HasData(
                            new IceCream { IceCreamId = 1, Name = "Vanilla", BestServedBefore = new DateTime(2025, 6, 1)},
                            new IceCream { IceCreamId = 2, Name = "Chocolate", BestServedBefore = new DateTime(2025, 6, 1)},
                            new IceCream { IceCreamId = 3, Name = "Matcha", BestServedBefore = new DateTime(2036, 1, 1)});
                    });
            }
        }
    }

    public class HavingBehaviorFixture : SingleStoreTestFixtureBase<Model.HavingBehaviorContext>;
}
