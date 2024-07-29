## About

_EntityFrameworkCore.SingleStore.Json.Microsoft_ adds JSON support for `System.Text.Json` (the Microsoft JSON stack) to [EntityFrameworkCore.SingleStore](https://github.com/memsql/SingleStore.EntityFrameworkCore).

## How to Use

```csharp
optionsBuilder.UseSingleStore(
    connectionString,
    options => options.UseMicrosoftJson())
```

## Related Packages

* [EntityFrameworkCore.SingleStore](https://www.nuget.org/packages/EntityFrameworkCore.SingleStore)
* [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)

## License

_EntityFrameworkCore.SingleStore.Json.Microsoft_ is released as open source under the [MIT license](https://github.com/memsql/SingleStore.EntityFrameworkCore/blob/master/LICENSE).

## Feedback

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/memsql/SingleStore.EntityFrameworkCore).

