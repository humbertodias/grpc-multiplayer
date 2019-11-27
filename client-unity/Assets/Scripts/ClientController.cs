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
    public Text userNameText;
    public Text error;

    private Channel channel;

    void Start()
    {
        channel = new Channel("127.0.0.1:57601", ChannelCredentials.Insecure);
    }

    public void SendUser()
    {
        try
        {
            var client = new User.UserClient(channel);
            var reply = client.Create(new CreateUserRequest { Name = userNameText.text });

            Debug.Log("Your ID is" + reply.Id);
            PlayerPrefs.SetString("userId", reply.Id);

            channel.ShutdownAsync().Wait();
            SceneManager.LoadScene("Main");

        } catch(Exception e)
        {
            error.text = e.ToString();
        }
    }
}
