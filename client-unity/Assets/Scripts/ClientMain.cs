using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Grpc.Core;
using Google.Protobuf;
using System.Threading;
using System.Threading.Tasks;
using Anharu;
using System;

public class ClientMain
{
    private Channel channel;
    private Multiplay.MultiplayClient client;

   public ClientMain(string hostPort)
    {
        channel = new Channel(hostPort, ChannelCredentials.Insecure);
        client = new Multiplay.MultiplayClient(channel);
    }

    public Channel GetChannel()
    {
        return channel;
    }

    public async Task SendPosition(string id, Vector3 tmp)
    {
        using (var call = client.SetPosition())
        {
            Console.WriteLine("Req: ");
            await call.RequestStream.WriteAsync(new SetPositionRequest { Id = id, X = tmp.x, Y = tmp.y, Z = tmp.z });
                
            await call.RequestStream.CompleteAsync();
                
            var response = await call;
            Console.WriteLine("After Response : " + response.Id + "-" + response.Status);
        }
    }

    public async Task SendConnectPosition(string id, Vector3 tmp)
    {
        using (var call = client.ConnectPosition())
        {
            int chunks = 0;
            Console.WriteLine("Starting background task to receive messages");
            var readTask = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext())
                {
                    Console.WriteLine("SendConnectPosition: " + call.ResponseStream.Current.Users);
                    chunks++;
                }
            });


            Console.WriteLine("Starting to send messages");
            for (var i = 0; i < chunks; i++)
            {
                Console.WriteLine("Req: " + i);
                await call.RequestStream.WriteAsync(new ConnectPositionRequest { Id = id + i, X = tmp.x, Y = tmp.y, Z = tmp.z });
            }

            Console.WriteLine("Disconnecting");
            await call.RequestStream.CompleteAsync();
            await readTask;
        }            
    }

    public async Task GetUsers()
    {
        var req = new GetUsersRequest { RoomId = "XXXX" };
        using (var call = client.GetUsers(req))
        {
            while (await call.ResponseStream.MoveNext())
            {
                Console.WriteLine("GetUsers: " + call.ResponseStream.Current.Users);
                //InstantiateUsers(call.ResponseStream.Current.Users);
            }
        }
    }    
    

    public void CloseConnection()
    {
        channel.ShutdownAsync().Wait();
    }    
    
}