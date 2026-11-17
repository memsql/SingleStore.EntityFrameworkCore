using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class AdHocComplexTypeQuerySingleStoreTest : AdHocComplexTypeQueryTestBase
{
    public AdHocComplexTypeQuerySingleStoreTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    // The context of this test is private in the base class, so we can't remap its INT key to BIGINT here.
    [ConditionalFact(Skip = "SingleStore: disabled due to distributed-table identity constraints for int keys.")]
    public override async Task Complex_type_equals_parameter_with_nested_types_with_property_of_same_name()
    {
        await base.Complex_type_equals_parameter_with_nested_types_with_property_of_same_name();

        AssertSql(
            """
            @__entity_equality_container_0_Id='1' (Nullable = true)
            @__entity_equality_container_0_Containee1_Id='2' (Nullable = true)
            @__entity_equality_container_0_Containee2_Id='3' (Nullable = true)

            SELECT `e`.`Id`, `e`.`ComplexContainer_Id`, `e`.`ComplexContainer_Containee1_Id`, `e`.`ComplexContainer_Containee2_Id`
            FROM `EntityType` AS `e`
            WHERE ((`e`.`ComplexContainer_Id` = @__entity_equality_container_0_Id) AND (`e`.`ComplexContainer_Containee1_Id` = @__entity_equality_container_0_Containee1_Id)) AND (`e`.`ComplexContainer_Containee2_Id` = @__entity_equality_container_0_Containee2_Id)
            LIMIT 2
            """);
    }

    // The context of this test is private in the base class, so we can't remap its INT key to BIGINT here.
    [ConditionalFact(Skip = "SingleStore: disabled due to distributed-table identity constraints for int keys.")]
    public override async Task Optional_complex_type_with_discriminator()
    {
        await base.Optional_complex_type_with_discriminator();
    }

    // The context of this test is private in the base class, so we can't remap its INT key to BIGINT here.
    [ConditionalFact(Skip = "SingleStore: disabled due to distributed-table identity constraints for int keys.")]
    public override async Task Complex_type_equality_with_non_default_type_mapping()
    {
        await base.Complex_type_equality_with_non_default_type_mapping();
    }

    protected TestSqlLoggerFactory TestSqlLoggerFactory
        => (TestSqlLoggerFactory)ListLoggerFactory;

    protected void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
