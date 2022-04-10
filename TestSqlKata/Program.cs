using Dapper;
using Microsoft.Extensions.Configuration;

using SqlKata.Execution;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Text.Json;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var connectionstring = config["ConnectionString"];


var connection = new SqlConnection(config["ConnectionString"]);
var compiler = new SqlKata.Compilers.SqlServerCompiler();
var db = new QueryFactory(connection, compiler);

var a = await db.Query("Audit").Take(5).OrderByDesc("Id").GetAsync<Audit>();

var b = await db.Query("Audit").Take(7).OrderByDesc("Id").GetAsync<Audit>();

//var query = new Query("Audit").Select("Id"). OrderByDesc("Id").Take(5);
//SqlResult result = compiler.Compile(query);
//string sql = result.Sql;


//using (var con = new SqlConnection(connectionstring))
//{
//    con.Open();

//    // Dapper 
//    Console.WriteLine("Fetch data");

//    var audits = await con.QueryAsync<Audit>("SELECT TOP 20 * FROM Audit ORDER BY Id DESC;");

//    foreach (var audit in audits)
//    {
//        using (var memorystream = new MemoryStream())
//        {
//            // Data-object (byte array) to transmit
//            await JsonSerializer.SerializeAsync(memorystream, audit);
//            var compressedbytes = memorystream.GZipBytes(CompressionMode.Compress);
            
//            // Print
//            var percent = compressedbytes.Length * 100 / memorystream.Length;
//            Console.WriteLine($"{audit.Updatetime} {memorystream.Length,10:#} {compressedbytes.Length,10:#} {percent,4:#}%");

//            // Unpack received data
//            var decompressedbytes = compressedbytes.GZipBytes(CompressionMode.Decompress);
//            var x = JsonSerializer.Deserialize<Audit>(decompressedbytes);
//        }
//    }
//}

Console.WriteLine("Finished..");
