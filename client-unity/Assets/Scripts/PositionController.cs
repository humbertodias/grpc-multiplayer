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

public class PositionController : MonoBehaviour
{

    private Channel channel;
    private Multiplay.MultiplayClient client;
    private AsyncDuplexStreamingCall<ConnectPositionRequest, ConnectPositionResponse> call;
    //private AsyncStreamingCall<SetPositionRequest, SetPositionResponse> call2;

    void Start()
    {
        channel = new Channel(PlayerPrefs.GetString("serverPort"), ChannelCredentials.Insecure);
        client = new Multiplay.MultiplayClient(channel);
        call = client.ConnectPosition();
    }

    private async Task QuitConnection()
    {
        await call.RequestStream.CompleteAsync();
        //await SetUsersPosition();
        channel.ShutdownAsync().Wait();
    }
/*
    private async Task SendPosition(String id, Vector3 tmp)
    {
        var req = new SetPositionRequest { Id = id, X = tmp.x, Y = tmp.y, Z = tmp.z };
        //Debug.Log("SendPosition " + tmp);
        await call2.RequestStream.WriteAsync(req);
    }
*/
}