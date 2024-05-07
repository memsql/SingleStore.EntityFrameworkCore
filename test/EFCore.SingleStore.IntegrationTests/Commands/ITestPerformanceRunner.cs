using System;
using System.Threading.Tasks;

namespace EntityFrameworkCore.SingleStore.IntegrationTests.Commands
{

    public interface ITestPerformanceRunner
    {
        Task ConnectionTask(Func<AppDb, Task> cb, int ops);
    }

}
