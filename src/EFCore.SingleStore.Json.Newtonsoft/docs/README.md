## About

_EntityFrameworkCore.SingleStore.Json.Newtonsoft_ adds JSON support for `Newtonsoft.Json` (the Newtonsoft JSON/Json.NET stack) to [EntityFrameworkCore.SingleStore](https://github.com/memsql/SingleStore.EntityFrameworkCore).

## How to Use

```csharp
optionsBuilder.UseSingleStore(
    connectionString,
    options => options.UseNewtonsoftJson())
```

## Related Packages

* [EntityFrameworkCore.SingleStore](https://www.nuget.org/packages/EntityFrameworkCore.SingleStore)
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## License

_EntityFrameworkCore.SingleStore.Json.Newtonsoft_ is released as open source under the [MIT license](https://github.com/memsql/SingleStore.EntityFrameworkCore/blob/master/LICENSE).

## Feedback

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/memsql/SingleStore.EntityFrameworkCore).

