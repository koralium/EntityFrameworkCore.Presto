# Presto Entity Framework Core/ADO.Net Provider

This provider is in a very early stage, but should support most queries.

It does not yet support any way to insert data into Presto.

# How to use

You use the provider as any other entity framework core provider:

```
services.AddDbContext<Context>(o =>
{
    o.UsePresto("Data Source=localhost:8080; Catalog=tpch; Schema=tiny; Trino=true;");
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

# ADO.NET

The ADO.NET provider is located in the Data.Presto package.

Example usage:

```
using var connection = new PrestoConnection()
{
  ConnectionString = "Data Source=localhost:8080; Catalog=tpch; Schema=tiny; Trino=true;"
};
using var command = connection.CreateCommand();
command.CommandText = "SELECT * FROM orders";
using var reader = command.ExecuteReader();

while(reader.Read())
{
  // Read columns
}
```

## Supported data types

* Boolean
* Tinyint
* Smallint
* Integer
* BigInt
* Real
* Double
* Decimal
* varchar
* char
* Timestamp / Date
* Array
* Row
* UUID


## Connection string settings

The connection string takes the following parameters:

| Name             | Description                            | Example                                   |
| ---------------- | -------------------------------------- | ----------------------------------------- |
| Data Source      | Address to the presto server           | Data Source=localhost:8080;               |
| User             | Name of the user                       | User=root;                                |
| Catalog          | Default catalog to use.                | Catalog=tpch;                             |
| Schema           | Default schema to use under a catalog. | Schema=tiny;                              |
| ExtraCredentials | Extra credentials to send.             | ExtraCredentials=key1:value1,key2:value2; |
| Trino            | Use trino headers (required for trino) | Trino=true;                               |
| Password         | Password for the user                  | Password=test;                            |
| Ssl		       | Https or http protocol				 	| Ssl=true;									|			

## SSL Traffic

If the SSL connection string option is left out, the ADO.Net provider tries to figure out the protocol by itself.
It first tries https but if that fails it tests http. This is saved as long as the application is running.
But for better first time performance if one is not using https is to set ssl=false in the connection string.

## Write Json

The ADO.NET DbDataReader has support to write a row directly to an Utf8JsonWriter.
This can be helpful if one is setting up an OData proxy in front of Presto for instance.

Example usage:

```
var writer = new Utf8JsonWriter(stream);
var prestoReader = (PrestoDataReader)reader;

writer.WriteStartArray();
while (prestoReader.Read())
{
  prestoReader.ToJson(writer);
}
writer.WriteEndArray();
writer.Flush();
```

# Nuget Package

https://www.nuget.org/packages/Data.Presto/

https://www.nuget.org/packages/EntityFrameworkCore.Presto/
