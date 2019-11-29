using System;
using Grpc.Core;
using Anharu;

namespace client
{
    class UserClient
    {
        static void MainX(string[] args)
        {
            Channel channel = new Channel("localhost:57601", ChannelCredentials.Insecure);
            var client = new User.UserClient(channel);
            var req = new CreateUserRequest { Name= "Test" };
            var resp = client.Create(req);
            Console.WriteLine(resp.Id);
        }
    }
}
