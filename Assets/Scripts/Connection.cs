using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Connection : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
      PhotonNetwork.ConnectUsingSettings();  
    }

    public override void OnConnectedToMaster()
    {
      Debug.Log("Connected to master");
      PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
      Debug.Log("Joined lobby");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
