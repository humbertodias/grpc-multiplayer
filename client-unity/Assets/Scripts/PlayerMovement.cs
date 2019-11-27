using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Grpc.Core;
using Google.Protobuf;
using System.Threading;
using System.Threading.Tasks;
using Anharu;


public class PlayerMoviment : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;

    
    private Channel channel;
    private AsyncDuplexStreamingCall<ConnectPositionRequest, ConnectPositionResponse> call;
    
    private Multiplay.MultiplayClient client;
    string id;

    private Hashtable userObjects;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        channel = new Channel("127.0.0.1:57601", ChannelCredentials.Insecure);
        client = new Multiplay.MultiplayClient(channel);
        call = client.ConnectPosition();

        id = PlayerPrefs.GetString("userId");
        this.userObjects = new Hashtable();

    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);


        // Send
        GetPosition();
        SendPosition();
        if (Input.GetKey(KeyCode.Q))
        {
            QuitConnection();
        }


    }

   private async Task GetPosition()
    {
        
        if (await call.ResponseStream.MoveNext())
        {
            ConnectPositionResponse positions = call.ResponseStream.Current;
            foreach(var user in positions.Users){

                Vector3 newPos = new Vector3((float)user.X, (float)user.Y, (float)user.Z);
                if (user.Id == id)
                {

                }
                // else if (!this.userObjects.ContainsKey(user.Id))
                // {
                //     GameObject otherPlayer = (GameObject)Instantiate(this.gameObject, newPos, Quaternion.identity) as GameObject;
                //     otherPlayer.name = user.Id;
                //     this.userObjects.Add(user.Id, "OK");
                // }
                // else
                // {
                //     GameObject activePlayer = (GameObject)this.userObjects[user.Id];
                //     if(activePlayer.transform.position != newPos ){
                //         activePlayer.transform.position = newPos;
                //     }
                // }


            }

        }
        Debug.Log("SIZE:" + this.userObjects.Count);
    }

    private async Task SendPosition()
    {
        Vector3 tmp = transform.position;
        var x = tmp.x;
        var y = tmp.y;
        var z = tmp.z;
        var req = new ConnectPositionRequest { Id = id, X = x, Y = y, Z = z };
        await call.RequestStream.WriteAsync(req);
    }

    private async Task QuitConnection()
    {
        await call.RequestStream.CompleteAsync();
        //await SetUsersPosition();
        channel.ShutdownAsync().Wait();
    }

 

    public void OnDestroy()
    {
        Debug.Log("OnDestroy");
        QuitConnection();
    }
}
