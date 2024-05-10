## About

_EntityFrameworkCore.SingleStore_ is the Entity Framework Core (EF Core) provider for [SingleStore](https://www.singlestore.com).

It is build on top of [SingleStoreConnector](https://github.com/memsql/SingleStoreNETConnector).

## How to Use

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Replace with your connection string.
        var connectionString = "server=localhost;user=root;password=1234;database=ef";

        // Replace 'YourDbContext' with the name of your own DbContext derived class.
        services.AddDbContext<YourDbContext>(
            dbContextOptions => dbContextOptions
                .UseSingleStore(connectionString)
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
    }
}
```

## Related Packages

* Other Packages
  * [SingleStoreConnector](https://www.nuget.org/packages/SingleStoreConnector)
  * [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)


