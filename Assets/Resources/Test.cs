using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace HutongGames.PlayMaker.Pun2.Actions
{
public class Test : PunActionBase
{

 public string[] ids;

PunCallbackInfo info;
public override void OnEnter()
{

    

   var Players = PhotonNetwork.CurrentRoom.Players;
   
   Debug.Log(Players.ElementAt(0).Value.NickName);
   Debug.Log(Players.ElementAt(0).Value.CustomProperties["char"]);


}




}
}