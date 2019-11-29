using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Grpc.Core;
using Google.Protobuf;
using System.Threading;
using System.Threading.Tasks;
using Anharu;
using System;

public class GameManager : MonoBehaviour
{

    private Channel channel;

    private Hashtable userObjects;

    
    void Awake()
    {
        channel = new Channel("127.0.0.1:57601", ChannelCredentials.Insecure);
        userObjects = new Hashtable();
    }

    public Channel GetChannel()
    {
        return channel;
    }

    public Hashtable getUserObjects()
    {
        return userObjects;
    }
    
    public void CloseConnection()
    {
        Debug.Log("Before close - " + channel.State);
        channel.ShutdownAsync().Wait();
        Debug.Log("After close - " + channel.State);
    }    
    
}