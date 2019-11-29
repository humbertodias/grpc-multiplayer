using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Grpc.Core;
using Google.Protobuf.Collections;
using Google.Protobuf;
using System.Threading;
using System.Threading.Tasks;
using Anharu;
using System;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    public GameObject gm;

    
    private Multiplay.MultiplayClient client;
    string id;

    private Movement movement;

    void Start()
    {
        //characterController = GetComponent<CharacterController>();
        client = new Multiplay.MultiplayClient(gm.GetComponent<GameManager>().GetChannel());

        id = PlayerPrefs.GetString("userId");
        gameObject.name = id;
        gm.GetComponent<GameManager>().getUserObjects().Add(id, this.gameObject);

        movement = GetComponent<Movement>();
        movement.OnBeforeFlip += beforeFlip;
        movement.OnAfterFlip += afterFlip;
    }

    private void beforeFlip(){
       // Debug.Log("beforeFlip");
    }
    private void afterFlip(){
        // Debug.Log("afterFlip");
        SendPosition();
    }

    void Update()
    {
        /*
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
*/

        GetUsers();
    }
    
    public async Task SendPosition()
    {
        Vector3 tmp = transform.position;
        var x = tmp.x;
        var y = tmp.y;
        var z = tmp.z;
        using (var call = client.SetPosition())
        {
            Console.WriteLine("Req: ");
            await call.RequestStream.WriteAsync(new SetPositionRequest { Id = id, X = x, Y = y, Z = z });
                
            await call.RequestStream.CompleteAsync();
                
            var response = await call;
            Console.WriteLine("After Response : " + response.Id + "-" + response.Status);
        }
    }

    public async Task SendConnectPosition()
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

            Vector3 tmp = transform.position;
            var x = tmp.x;
            var y = tmp.y;
            var z = tmp.z;

            Console.WriteLine("Starting to send messages");
            for (var i = 0; i < chunks; i++)
            {
                Console.WriteLine("Req: " + i);
                await call.RequestStream.WriteAsync(new ConnectPositionRequest { Id = id + i, X = x, Y = y, Z = z });
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
                InstantiateUsers(call.ResponseStream.Current.Users);
            }
        }
    }


    void InstantiateUsers(RepeatedField<UserPosition> users)
    {
        Hashtable userObjects = gm.GetComponent<GameManager>().getUserObjects();
        foreach(var user in users){
            Debug.Log("user:" + user.Id + ",size:" + userObjects.Count + "---" + userObjects.ContainsKey(user.Id));
            Vector3 newPos = new Vector3((float)user.X, (float)user.Y, (float)user.Z);
            if (user.Id == id || gameObject.name == id)
            {
                //Debug.Log("if id == id");
            }
            else if (!userObjects.ContainsKey(user.Id))
             {
                 GameObject otherPlayer = (GameObject)Instantiate(this.gameObject, newPos, Quaternion.identity) as GameObject;
                 otherPlayer.name = user.Id;
                 userObjects.Add(user.Id, otherPlayer);
                 //Debug.Log("else if contains.id");
             }
             else
             {
                 GameObject activePlayer = (GameObject)userObjects[user.Id];
                 if(activePlayer.transform.position != newPos ){
                     activePlayer.transform.position = newPos;
                 }
                 //Debug.Log("else");
             }


        }
    }
    
    public void OnDestroy()
    {
        try
        {
            Debug.Log("OnDestroy");
            gm.GetComponent<GameManager>().CloseConnection();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
 
    }
}
