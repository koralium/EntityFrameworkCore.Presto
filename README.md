# Presto Entity Framework Core Provider

This provider is in a very early stage, but should support most queries.

It does not yet support any way to insert data into Presto.

# How to use

You use the provider as any other entity framework core provider:

```
services.AddDbContext<Context>(o =>
{
    o.UsePresto("Data Source=localhost:8080; Catalog=tpch; Schema=tiny;");
});
```

## Model Presto Row data type

To use Presto's Row data type, you can do the following in model creating:

```
modelBuilder.Entity<Customer>(customer =>
{
    customer.ToTable("customer")
        .HasKey(x => x.Custkey);

    customer.Property(x => x.Object).IsRowType();
});
```

This will mark the property "Object" to be included in the table, and not be collected through a reference table.

## Model Presto Array data type

To use Presto's Array data type, it is very similar to the row data type:

```
modelBuilder.Entity<Customer>(customer =>
{
    customer.ToTable("customer")
        .HasKey(x => x.Custkey);

    customer.Property(x => x.Array).IsArrayType();
});
```

# Nuget Package

https://www.nuget.org/packages/EntityFrameworkCore.Presto/
