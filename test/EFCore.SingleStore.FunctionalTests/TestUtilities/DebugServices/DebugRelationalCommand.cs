using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.DebugServices;

public class DebugRelationalCommand : RelationalCommand
{
    public DebugRelationalCommand(
        [NotNull] RelationalCommandBuilderDependencies dependencies,
        [NotNull] string commandText,
        [NotNull] IReadOnlyList<IRelationalParameter> parameters)
        : base(dependencies, commandText, parameters)
    {
    }

    protected override RelationalDataReader CreateRelationalDataReader()
        => new DebugRelationalDataReader();
}
