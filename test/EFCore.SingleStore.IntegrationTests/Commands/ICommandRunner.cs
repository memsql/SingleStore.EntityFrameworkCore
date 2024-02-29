namespace EntityFrameworkCore.SingleStore.IntegrationTests.Commands
{

    public interface ICommandRunner
    {
        int Run(string[] args);
    }

}
