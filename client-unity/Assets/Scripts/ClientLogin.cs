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

public class ClientLogin : MonoBehaviour
{
    public Text server;
    public Text userName;
    public Text error;

    public string nextScene;

    void Start()
    {
        string serverPort = PlayerPrefs.GetString("serverPort");
        if (serverPort != null)
        {
            server.text = serverPort;
        }
    }
    public void SendUser()
    {
        try
        {
            PlayerPrefs.SetString("serverPort", server.text);
            Debug.Log("Server>" + server.text);
            var channel = new Channel(server.text, ChannelCredentials.Insecure);
            var client = new User.UserClient(channel);
            var reply = client.Create(new CreateUserRequest { Name = userName.text });

            Debug.Log("Your ID is" + reply.Id);
            PlayerPrefs.SetString("userId", reply.Id);

            channel.ShutdownAsync().Wait();

            SceneManager.LoadScene(nextScene);

        } catch(Exception e)
        {
            error.text = server.text = "\n"; 
            error.text += e.ToString();
        }
    }
}
