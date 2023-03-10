// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.
// Author jean@hutonggames.com
// This code is licensed under the MIT Open source License

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


#pragma warning disable 0414

namespace HutongGames.PlayMaker.Pun2
{
    /// <summary>
    /// This component is required on gameObjects and prefabs you are planning to instanciate over the Photon network.
    /// 
    /// It receives the OnPhotonInstantiate message when instanciated and forward it as an Event for Fsm component attached this gameObject and all its childrens.
    /// 
    /// It also verifies the proper setup for fsm components on that gameObject that have networked synched variables:
    /// *It assumes like for the Unity networking that you have a PhotonView observing the fsm.
    /// *I insert at runtime a bridge ( PlayMakerPhotonView ) that goes inbetween the fsm and the photonView. This is required because the fsmComponent doesn't implement photon networking nativly
    /// ( that is not implementing OnPhotonInstantiate() nor OnPhotonSerializeView
    /// It can be set up manually of course, else Iwill do it for the user at runtime when the gameObject is instanciated. 
    /// Note: for gameObject sitting in the hierarchy when starting, the check is also happening but it's directly call within the "PlayMaker photon proxy" mandatory prefab
    /// </summary>
    public class PlayMakerPhotonGameObjectProxy : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {

        const string DebugLabelPrefix = "<color=navy>PlayMaker Photon GameObject proxy: </color>";


        private static Dictionary<int, List<PlayMakerPhotonGameObjectProxy>> LutByActorNumber = new Dictionary<int, List<PlayMakerPhotonGameObjectProxy>>();
        
        /// <summary>
        /// output in the console activities of the various elements.
        /// Is taken form the debug value of the "PlayMaker Photon Proxy"
        /// </summary>
        bool debug = true;
        bool LogMessageInfo = true;

        private PhotonMessageInfo _lastCallbackInfo;
        
        [ContextMenu("Help")]
        public void help()
        {
            Application.OpenURL("https://hutonggames.fogbugz.com/default.asp?W990");
        }


        // get the Playmaker Photon proxy fsm.
        void Awake()
        {


            Debug.Log("Player awake",this);

            // get the photon proxy for Photon Fsm Proxy to send event.
            GameObject go = GameObject.Find("PlayMaker Photon Proxy");

            if (go == null)
            {
                Debug.LogError("Working with photon network require that you add a 'PlayMaker Photon Proxy' component to the gameObject. You can do so from the menu 'PlayMaker Photon/components/Add photon proxy to scene'");
                return;
            }


            //// get the proxy to set the debug flag.
            PlayMakerPhotonProxy _proxy = go.GetComponent<PlayMakerPhotonProxy>();
            if (_proxy!=null)
            {
            	debug = _proxy.debug;
            	LogMessageInfo = _proxy.LogMessageInfo;
            }

            _proxy.SanitizeGameObject(this.gameObject);

        }// Awake

        #region Photon Messages

        /// <summary>
        /// compose this message to dispatch the associated global Fsm Event. 
        /// TOFIX: The problem is, It's called before Fsm are instanciated, so I had to implement a slight delay...
        /// </summary>
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            _lastCallbackInfo = info;
            
            if (debug)
            {
                if (!info.Equals(new PhotonMessageInfo()))
                {
                    if (info.Sender != null)
                    {
                        Debug.Log("PlayMaker Photon proxy:OnPhotonInstantiate " + info.Sender.NickName);
                    }
                    else
                    {
                        Debug.Log("PlayMaker Photon proxy:OnPhotonInstantiate with no sender info");
                    }
                }
                else
                {
                    Debug.Log("PlayMaker Photon proxy:OnPhotonInstantiate with no PhotonMessageInfo");
                }
            }

            // TOFIX: if we found a better solution, I am all up for it... How stable this can be if framerate is low for example?
            Invoke("sendPhotonInstantiationFsmEvent", 0.1f);


        }// OnPhotonInstantiate


        /// <summary>
        /// Sends the photon instantiation fsm event to ALL Fsm of the instantiated gameObject AND its children.
        /// </summary>
        void sendPhotonInstantiationFsmEvent()
        {
            string _fsmEvent = PlayMakerPunLUT.CallbacksEvents[PunCallbacks.OnPhotonInstantiate];

            if (debug)
            {
                Debug.Log("sending "+_fsmEvent+" event to " + this.gameObject.name);
            }
            
            // set the target to be this gameObject.
            FsmOwnerDefault goTarget = new FsmOwnerDefault();
            goTarget.GameObject = new FsmGameObject();
            goTarget.GameObject.Value = this.gameObject;
            goTarget.OwnerOption = OwnerDefaultOption.SpecifyGameObject;

            // send the event to this gameObject and all its children
            FsmEventTarget eventTarget = new FsmEventTarget();
            eventTarget.excludeSelf = false;
            eventTarget.target = FsmEventTarget.EventTarget.GameObject;
            eventTarget.gameObject = goTarget;
            eventTarget.sendToChildren = true;

            
            // create the event.
            FsmEvent fsmEvent = new FsmEvent(_fsmEvent);

            string _data = "<color=darkblue>" + "ActorNumber" + "</color>=<color=<darkblue>" + _lastCallbackInfo.Sender.ActorNumber + "</color> ";
            
            _data += "<color=darkblue>" + "NickName" + "</color>=<color=<darkblue>" + _lastCallbackInfo.Sender.NickName + "</color> ";
         
            
            Debug.Log( "PlayMakerPhotonGameObjectProxy Received Callback <color=fuchsia>" + PunCallbacks.OnPhotonInstantiate + "</color> " +
                      "Broadcasting global Event <color=fuchsia>" + fsmEvent.Name  + "</color>\n"+
                      _data
                , this);
            
            FsmEventData _d = new FsmEventData();
            _d.StringData = _lastCallbackInfo.Sender.NickName;
            _d.IntData = _lastCallbackInfo.Sender.ActorNumber;
            PlayMakerUtils.SendEventToTarget(null,eventTarget, fsmEvent.Name,_d); 

        }// sendPhotonInstantiationFsmEvent

        #endregion

        #region RPC

        /// <summary>
        /// Function typically called from the action "PhotonViewRpcBroadcasFsmEvent" that use RPC to send information about the event to broadcast
        /// </summary>
        /// <param name='target'>
        /// Photon player Target.
        /// </param>
        /// <param name='globalEventName'>
        /// Global Fsm event name to broadcast to the player target
        /// </param>
        public void PhotonRpcBroadcastFsmEvent(Player target, string globalEventName)
        {
            if (LogMessageInfo)
            {
                Debug.Log("RPC to send global Fsm Event:" + globalEventName + " to player:" + target);
            }

            photonView.RPC("rpc", target, globalEventName);
        }
        
        /// <summary>
        /// Function typically called from the action "PhotonViewRpcBroadcasFsmEventToPlayer" that use RPC to send information about the event to broadcast
        /// </summary>
        /// <param name='target'>
        /// Photon player Target.
        /// </param>
        /// <param name='globalEventName'>
        /// Global Fsm event name to broadcast to the player target
        /// </param>
        /// <param name='stringData'>
        /// String data to pass with this event. WARNING: this is not supposed to be (nor efficient) a way to synchronize data. This is simply to comply with
        /// the ability for FsmEvent to include data.
        /// </param>
        public void PhotonRpcFsmBroadcastEventWithString(Player target, string globalEventName, string stringData)
        {
            if (LogMessageInfo)
            {
                Debug.Log("RPC to send string:" + stringData + " with global Fsm Event:" + globalEventName + " to player:" + target.ToString());
            }

            photonView.RPC("rpc_s", target, globalEventName, stringData);
        }
        
        /// <summary>
        /// Function typically called from the action "PhotonViewRpcBroadcasFsmEvent" that use RPC to send information about the event to broadcast
        /// </summary>
        /// <param name='target'>
        /// Photon Target.
        /// </param>
        /// <param name='globalEventName'>
        /// Global Fsm event name to broadcast using the photon target rule.
        /// </param>
        public void PhotonRpcBroadcastFsmEvent(RpcTarget target, string globalEventName)
        {
            if (LogMessageInfo)
            {
                Debug.Log("RPC to send global Fsm Event:" + globalEventName + " to target:" + target,this);
            }

            this.photonView.RPC("rpc", target, globalEventName);// method name used to be too long : "RPC_PhotonRpcBroadcastFsmEvent"
        }

        /// <summary>
        /// Function typically called from the action "PhotonViewRpcBroadcasFsmEvent" that use RPC to send information about the event to broadcast
        /// </summary>
        /// <param name='target'>
        /// Photon Target.
        /// </param>
        /// <param name='globalEventName'>
        /// Global Fsm event name to broadcast using the photon target rule.
        /// </param>	
        /// <param name='stringData'>
        /// String data to pass with this event. WARNING: this is not supposed to be (nor efficient) a way to synchronize data. This is simply to comply with
        /// the ability for FsmEvent to include data.
        /// </param>
        public void PhotonRpcBroadcastFsmEventWithString(RpcTarget target, string globalEventName, string stringData)
        {
            if (LogMessageInfo)
            {
                Debug.Log("RPC to send string:" + stringData + "  with global Fsm Event:" + globalEventName + " to target:" + target);
            }

            photonView.RPC("rpc_s", target, globalEventName, stringData);// method name used to be too long :  "RPC_FsmPhotonRpcBroadcastFsmEventWithString"
        }
        
        
        /// <summary>
        /// RPC CALL to this photonView
        /// </summary>
        /// <param name='globalEventName'>
        /// Global Fsm event name.
        /// </param>
        /// <param name='info'>
        /// Info.
        /// </param>
        [PunRPC]
        void rpc(string globalEventName, PhotonMessageInfo info) // method name used to be too long :  RPC_PhotonRpcBroadcastFsmEvent
        {
            if (LogMessageInfo)
            {
                Debug.Log("RPC Received for owner+"+this.photonView.OwnerActorNr+" for global event '"+ globalEventName+ "' info:"+info,this);
            }
            
           // lastMessagePhotonPlayer = info.Sender;
           
           PlayMakerUtils.SendEventToGameObject(null,this.gameObject,globalEventName,true);
        }

        [PunRPC]
        void rpc_s(string globalEventName, string stringData)
        {
            Fsm.EventData.StringData = stringData;
            
            PlayMakerUtils.SendEventToGameObject(null, this.gameObject, globalEventName,true);
        }
        
        #endregion




    }
}