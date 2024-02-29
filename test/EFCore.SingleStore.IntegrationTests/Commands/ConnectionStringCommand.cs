using System;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.IntegrationTests.Commands
{
    public class ConnectionStringCommand : IConnectionStringCommand
    {
        public void Run()
        {
            Console.Write(AppConfig.ConnectionString);
        }
    }
}
