using System;
using Grpc.Core;
using Anharu;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace client
{
    // https://mongodb.github.io/mongo-csharp-driver/2.3/apidocs/html/M_MongoDB_Driver_MongoClient_DropDatabase.htm
    class MongoClientTest
    {
        static void MainX(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("test");
            Task task = create(db, "movie");


            Console.WriteLine(client);
            Console.WriteLine(db);
            Console.WriteLine(task);

            Task.WaitAll(task);

            // insert(collection);
            // list(collection);

        }

        static async Task create(IMongoDatabase db, string collection){
           await db.CreateCollectionAsync(collection);
           Console.WriteLine("JUJU");
        }

//         static void list(MongoCollection collection){
// //            var collection = db.GetCollection<BsonDocument>(collection);
//             var cursor = collection.Find(new BsonDocument()).ToCursor();
//             foreach (var document in cursor.ToEnumerable())
//             {
//                 Console.WriteLine(document);   
//             }
//         }


//         static async void insert(MongoCollection collection){

//             var document = new BsonDocument
//             {
//                 { "name", "MongoDB" },
//                 { "type", "Database" },
//                 { "count", 1 },
//                 { "info", new BsonDocument
//                     {
//                         { "x", 203 },
//                         { "y", 102 }
//                     }}
//             };

//             await collection.InsertOneAsync(document);

//         }
    }
}


