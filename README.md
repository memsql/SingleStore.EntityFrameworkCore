# SingleStore.EntityFrameworkCore

`SingleStore.EntityFrameworkCore` is the Entity Framework Core provider for SingleStore. It uses [SingleStoreConnector](https://github.com/memsql/SingleStoreNETConnector) for high-performance database server communication.

## Schedule and Roadmap

Milestone | Status               | Release Date
----------|----------------------|-------------
7.0.0| released             | November 2024
6.0.2| general availability | April 2024
## Getting Started

### 1. Project Configuration

Ensure that your `.csproj` file contains the following reference:

```xml
<PackageReference Include="EntityFrameworkCore.SingleStore" Version="6.0.2" />
```

### 2. Services Configuration

Add `EntityFrameworkCore.SingleStore` to the services configuration in your the `Startup.cs` file of your ASP.NET Core project:

```c#
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

View our [Configuration Options Wiki Page](https://github.com/PomeloFoundation/EntityFrameworkCore.MySql/wiki/Configuration-Options) for a list of common options.

### 3. Sample Application

Check out our [Integration Tests](https://github.com/memsql/SingleStore.EntityFrameworkCore/tree/master/test/EFCore.SingleStore.IntegrationTests) for an example repository that includes an ASP.NET Core MVC Application.

There are also many complete and concise console application samples posted in the issue section (some of them can be found by searching for `Program.cs`).

### 4. Read the EF Core Documentation

Refer to Microsoft's [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/) for detailed instructions and examples on using EF Core.

## Scaffolding / Reverse Engineering

Use the [EF Core tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) to execute scaffolding commands:

```
dotnet ef dbcontext scaffold "Server=localhost;User=root;Password=1234;Database=ef" "SingleStore.EntityFrameworkCore"
```

## License

[MIT](https://github.com/memsql/SingleStore.EntityFrameworkCore/blob/master/LICENSE)
