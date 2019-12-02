using System;
using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;
using Anharu;

public class ClientController : MonoBehaviour
{
    public Text userName;
    public Text server;
    public Text error;

    public void SendUser()
    {
        try
        {
            Debug.Log("Server>" + server.text);
            var channel = new Channel(server.text, ChannelCredentials.Insecure);
            var client = new User.UserClient(channel);
            var reply = client.Create(new CreateUserRequest { Name = userName.text });

            Debug.Log("Your ID is" + reply.Id);
            PlayerPrefs.SetString("userId", reply.Id);

            channel.ShutdownAsync().Wait();

            PlayerPrefs.SetString("serverPort", server.text);
            SceneManager.LoadScene("Main");

        } catch(Exception e)
        {
            error.text = e.ToString();
        }
    }
}
