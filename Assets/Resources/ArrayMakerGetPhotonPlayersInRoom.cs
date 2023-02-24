// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Photon")]
	[Tooltip("Store the list of Photon players in an ArrayList")]
	public class ArrayMakerGetPhotonPlayersInRoom : CollectionsActions
	{
		
		[ActionSection("Array referencing")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The ArrayList reference that will host the players names")]
		public FsmString NameReference;
		
		[Tooltip("The ArrayList reference that will host the players id")]
		public FsmString IdReference;

        [Tooltip("The ArrayList reference that will host the players characters")]
		public FsmString CharReference;
		
		[ActionSection("Options")]
		[Tooltip("If true, list only other players.")]
		public FsmBool otherPlayerOnly;
		
	
		private Photon.Realtime.Player[] players;
		private string[] playerNames;
		
		
		
		public override void Reset()
		{
			gameObject = null;
			otherPlayerOnly = true;
			NameReference = "Photon Player Name";
			IdReference = "Photon Player ID";
            CharReference = "Player Char";
		}
		
		public override void OnEnter()
		{
			if (otherPlayerOnly.Value){
				players = PhotonNetwork.PlayerListOthers;
			}else{
				players = PhotonNetwork.PlayerList;
			}
							//
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			
			// -------- Player -----------//
			// get each arrayList reference
			PlayMakerArrayListProxy nameList = GetArrayListProxyPointer(go,NameReference.Value,false);
					
			PlayMakerArrayListProxy idList = GetArrayListProxyPointer(go,IdReference.Value,false);

			PlayMakerArrayListProxy charList = GetArrayListProxyPointer(go,CharReference.Value,false);

			nameList.arrayList.Clear();
			idList.arrayList.Clear();
			charList.arrayList.Clear();
			
			foreach (Photon.Realtime.Player player in players)
            {
				nameList.arrayList.Add(player.NickName);
				idList.arrayList.Add(player.UserId);
                charList.arrayList.Add((string)player.CustomProperties["char"]);
			}
	
		}
	}
}
