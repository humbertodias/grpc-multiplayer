using System;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Core;
using Anharu;

namespace client
{
    class MultiplayerClient
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("localhost:57601", ChannelCredentials.Insecure);
            var client = new Multiplay.MultiplayClient(channel);

            // var s1 = SendSetPosition(client);
            // Thread.Sleep(500);
            // var s2 = SendSetPosition(client);
            //Task.WaitAll(s1,s2);
            // resp(s1);
            // resp(s2);



            // var resp2 = SendConnectPosition(client);
            // Console.WriteLine("Resp2:" + resp2);

            var resp3 = GetUsers(client);
            Console.WriteLine("Resp3:" + resp3);

            Console.WriteLine("Before:" + channel);
            channel.ShutdownAsync().Wait();
            Console.WriteLine("After:" + channel);

        }

        // BiDiretional
        static async Task SendConnectPosition(Multiplay.MultiplayClient client)
        {
            using (var call = client.ConnectPosition())
            {
                Console.WriteLine("Starting background task to receive messages");
                var readTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        Console.WriteLine("Greeting: " + call.ResponseStream.Current.Users);
                        // "Greeting: Hello World" is written multiple times
                    }
                });

                Console.WriteLine("Starting to send messages");
                Console.WriteLine("Type a message to echo then press enter.");
                for (var i = 0; i < 50; i++)
                {
                    Console.WriteLine("Req: " + i);
                   // Thread.Sleep(599);
                    await call.RequestStream.WriteAsync(new ConnectPositionRequest { Id = "id" + i, X = 1f, Y = 2f, Z = 3f });
                }

                Console.WriteLine("Disconnecting");
                await call.RequestStream.CompleteAsync();
                await readTask;
            }            
        }


        // Client Stream
        static async Task SendSetPosition(Multiplay.MultiplayClient client)
        {
            using (var call = client.SetPosition())
            {
                Console.WriteLine("Req: ");
                await call.RequestStream.WriteAsync(new SetPositionRequest { Id = "TEST", X = 1f, Y = 2f, Z = 3f });
                
                await call.RequestStream.CompleteAsync();
                
                var response = await call;
                Console.WriteLine("After Response : " + response.Id + "-" + response.Status);
            }
        }

        // Server Stream
        static async Task GetUsers(Multiplay.MultiplayClient client)
        {
            var req = new GetUsersRequest { RoomId = "XXXX" };
            using (var call = client.GetUsers(req))
            {
                
                while (await call.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Greeting: " + call.ResponseStream.Current.Users);
                }
            }
        }


    }
}
