# SingleStore.EntityFrameworkCore

`SingleStore.EntityFrameworkCore` is the Entity Framework Core provider for SingleStore. It uses [SingleStoreConnector](https://github.com/memsql/SingleStoreNETConnector) for high-performance database server communication.

## Schedule and Roadmap

Milestone | Status               | Release Date
----------|----------------------|-------------
8.0.3| released             | February 2026
8.0.0| released             | July 2025
7.0.1| released             | March 2025
7.0.0| released             | November 2024
6.0.2| general availability | April 2024
## Getting Started

### 1. Project Configuration

Ensure that your `.csproj` file contains the following reference:

```xml
<PackageReference Include="EntityFrameworkCore.SingleStore" Version="8.0.0" />
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

View our [Configuration Options Wiki Page](https://github.com/memsql/SingleStore.EntityFrameworkCore/wiki/Configuration-Options) for a list of common options.

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

## Time zone handling
SingleStore does not support changing [`@@session.time_zone`](https://docs.singlestore.com/db/v9.0/user-and-cluster-administration/maintain-your-cluster/setting-the-time-zone/time-zone-engine-variables/) at runtime (it is a MySQL-compatibility variable and remains `SYSTEM`).

If you need `DateTimeOffset.LocalDateTime` translation, configure a session time zone offset in the provider:
```c#
optionsBuilder.UseSingleStore(cs, o => o.SessionTimeZone("-08:00"));
```
If not configured, the provider defaults to `+00:00` (UTC).

## License

[MIT](https://github.com/memsql/SingleStore.EntityFrameworkCore/blob/master/LICENSE)
