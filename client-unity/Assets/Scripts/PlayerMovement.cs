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
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    
    string id;
    private ClientMain client;

    private Movement movement;

    void Start()
    {
        client = new ClientMain(PlayerPrefs.GetString("serverPort"));
        
        id = PlayerPrefs.GetString("userId");
        gameObject.name = id;
      
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

//        GetUsers();
    }
    
    public void SendPosition()
    {
        client.SendPosition(id, transform.position);
    }

    public void SendConnectPosition()
    {
        client.SendConnectPosition(id, transform.position);
    }


    void InstantiateUsers(RepeatedField<UserPosition> users)
    {
//        Hashtable userObjects = gm.GetComponent<ClientMain>().getUserObjects();
//        foreach(var user in users){
//            Debug.Log("user:" + user.Id + ",size:" + userObjects.Count + "---" + userObjects.ContainsKey(user.Id));
//            Vector3 newPos = new Vector3((float)user.X, (float)user.Y, (float)user.Z);
//            if (user.Id == id || gameObject.name == id)
//            {
//                //Debug.Log("if id == id");
//            }
//            else if (!userObjects.ContainsKey(user.Id))
//             {
//                 GameObject otherPlayer = (GameObject)Instantiate(this.gameObject, newPos, Quaternion.identity) as GameObject;
//                 otherPlayer.name = user.Id;
//                 userObjects.Add(user.Id, otherPlayer);
//                 //Debug.Log("else if contains.id");
//             }
//             else
//             {
//                 GameObject activePlayer = (GameObject)userObjects[user.Id];
//                 if(activePlayer.transform.position != newPos ){
//                     activePlayer.transform.position = newPos;
//                 }
//                 //Debug.Log("else");
//             }
//
//
//        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("Register");
    }
    
    public void OnDestroy()
    {
        try
        {
            client.CloseConnection();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
 
    }
}
