using Dapper;
using SqlKata;
using SqlKata.Execution;
using System.Data.SqlClient;

//var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
//var connectionstring = config["ConnectionString"];

// https://github.com/sqlkata/querybuilder/issues/213 Lack of Connection Dispose in documentation
var connectionstring = "Server=127.0.0.1;Database=NorthWind;Trusted_Connection=True;";
SqlConnection sqlconnection = new(connectionstring);
SqlKata.Compilers.SqlServerCompiler compiler = new();

IEnumerable<Customers> records;
Query query;

using (QueryFactory db = new(sqlconnection, compiler))
{
    db.Logger = compiled => { Console.WriteLine(compiled.ToString()); };

    records = await db
        .Query(nameof(Customers))
        .Take(5)
        .OrderByDesc(nameof(Customers.CustomerID))
        //.Select(ColumnNames<Customers>("CustomerID", nameof(Customers.ContactName)))
        .Select("CustomerID", "ContactName")
        .GetAsync<Customers>();
    
    foreach (var record in records) Console.WriteLine(record.ContactName);

    query = new Query(nameof(Customers)).OrderByDesc(nameof(Customers.CustomerID));
    query.Take(3);
    records = db.Get<Customers>(query);

    foreach (var record in records) Console.WriteLine(record.ContactName);
}

query = new Query(nameof(Customers)).Take(4).OrderByDesc(nameof(Customers.CustomerID));
var compiledquery = compiler.Compile(query);
Console.WriteLine(compiledquery.Sql);

using (SqlConnection dappercon = new(connectionstring))
{
    dappercon.Open();
    records = await dappercon.QueryAsync<Customers>(compiledquery.Sql, compiledquery.NamedBindings);
}

using (SqlConnection sqlcon = new(connectionstring))
{
    SqlCommand cmd = new SqlCommand(compiledquery.Sql, sqlcon);
    var sqlparameters = compiledquery.NamedBindings.Select(p => new SqlParameter(p.Key, p.Value)).ToArray();
    cmd.Parameters.AddRange(sqlparameters);
    sqlcon.Open();

    var reader = cmd.ExecuteReader();
    while (reader.Read())

        Console.WriteLine(reader[5]);
    sqlcon.Close();
};

Console.WriteLine("Finished..");

// -------------------

string[] ColumnNames<T>(params string[] nameof)
{
    return nameof.Select(s => $"{typeof(T).Name}.{s}").ToArray();
}

//    foreach (var record in records) Console.WriteLine(record.ContactName);
//    //    foreach (var audit in audits)
//    //    {
//    //        using (var memorystream = new MemoryStream())
//    //        {
//    //            // Data-object (byte array) to transmit
//    //            await JsonSerializer.SerializeAsync(memorystream, audit);
//    //            var compressedbytes = memorystream.GZipBytes(CompressionMode.Compress);

//    //            // Print
//    //            var percent = compressedbytes.Length * 100 / memorystream.Length;
//    //            Console.WriteLine($"{audit.Updatetime} {memorystream.Length,10:#} {compressedbytes.Length,10:#} {percent,4:#}%");

//    //            // Unpack received data
//    //            var decompressedbytes = compressedbytes.GZipBytes(CompressionMode.Decompress);
//    //            var x = JsonSerializer.Deserialize<Audit>(decompressedbytes);
//    //        }
//    //    }