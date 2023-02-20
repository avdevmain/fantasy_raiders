// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.
// Author jean@hutonggames.com
// Amended by philipherlitz@gmail.com of Googly Eyes Games
// This code is licensed under the MIT Open source License

using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace HutongGames.PlayMaker.Pun2
{
    public class PlayMakerPhotonProxyRoomList : MonoBehaviourPunCallbacks
    {

        public static PlayMakerPhotonProxyRoomList Instance;

        public PlayMakerArrayListProxy proxy;
        int index;
        //  Dictionary<string, RoomInfo> RoomList = new Dictionary<string, RoomInfo>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }

            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfo room in roomList)
            {
                if (proxy.arrayList.Contains(room.Name))
                {
                    index = proxy.arrayList.IndexOf(room.Name);

                    if (room.RemovedFromList)
                    {
                        // we delete the entry
                        // RoomList.Remove(room.Name);
                        proxy.arrayList.RemoveAt(index);
                        // Debug.LogWarning("Removed from list.");
                    }
                    else
                    {
                        // we update the entry
                        //RoomList[room.Name] = room;
                        string roomData = (room.Name + "," + room.PlayerCount + "," + room.MaxPlayers + "," + room.IsOpen.ToString());
                        proxy.Set(index, roomData, "String");
                        //  Debug.LogWarning("Set at index");
                    }
                }
                else
                {
                    if (!room.RemovedFromList)
                    {
                        // we create the entry
                        //RoomList[room.Name] = room;
                        string roomData = (room.Name + "," + room.PlayerCount + "," + room.MaxPlayers + "," + room.IsOpen.ToString());
                        proxy.Add(roomData, "String");
                        //   Debug.LogWarning("Add to list.");
                    }
                }
            }


        }
    }
}