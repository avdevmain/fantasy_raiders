// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.
// Author jean@hutonggames.com
// This code is licensed under the MIT Open source License

using UnityEngine;
using Photon.Pun;

namespace HutongGames.PlayMaker.Pun2.Actions
{
	[ActionCategory("Photon")]
	[Tooltip("Remote Event Calls (using Photon RPC under the hood) let you broadcast a Fsm Event to photon targets ( all players, other players, master).")]
	[HelpUrl("")]
	public class PhotonViewRpcBroadcastFsmEvent : PunActionBase
	{

		[Tooltip("The rpc targets.")]
		[ObjectType(typeof(RpcTarget))]
        public FsmEnum  rpcTargets;
		
		[Tooltip("you can set it to BroadCastAll or pick a gameobject with a PlayMakerPhotonGameObjectProxy component to target just that gameobject photonView.")]
		public FsmEventTarget eventTarget;
		
		[RequiredField]
		[Tooltip("The event you want to send.")]
		public FsmEvent remoteEvent;
		
		[Tooltip("Optional string data ( will be injected in the Event data. Use 'get Event Info' action to retrieve it)")]
		public FsmString stringData;

		
		RpcTarget _rpcTargets;
		
	
		public override void Reset()
		{

			eventTarget = new FsmEventTarget();
			eventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			
			remoteEvent = null;
			rpcTargets = null;
			stringData = null;
		}

		public override void OnEnter()
		{
			ExecuteAction();
			
			Finish();
		}

		void ExecuteAction()
		{

			if (remoteEvent != null && remoteEvent.IsGlobal == false)
			{ 
				return;
			}
			
			if (PlayMakerPhotonProxy.Instance==null)
			{
				Debug.LogError("PlayMakerPhotonProxy is missing in the scene");
				return;
			}
			
			_rpcTargets = (RpcTarget)rpcTargets.Value;
			
			if (eventTarget.target == FsmEventTarget.EventTarget.BroadcastAll)
			{
				
				if (! stringData.IsNone && stringData.Value != ""){
					PlayMakerPhotonProxy.Instance.PhotonRpcBroadcastFsmEventWithString(_rpcTargets, remoteEvent.Name,stringData.Value);
				}else{
					PlayMakerPhotonProxy.Instance.PhotonRpcBroadcastFsmEvent(_rpcTargets, remoteEvent.Name);
				}
			}else if (eventTarget.target == FsmEventTarget.EventTarget.GameObject)
			{

				GameObject _go = Fsm.GetOwnerDefaultTarget(eventTarget.gameObject);

				PlayMakerPhotonGameObjectProxy _pvProxy = _go != null ? _go.GetComponent<PlayMakerPhotonGameObjectProxy>() : null;

				if (_pvProxy != null)
				{
					if (!stringData.IsNone && stringData.Value != "")
					{
						_pvProxy.PhotonRpcBroadcastFsmEventWithString(_rpcTargets, remoteEvent.Name,stringData.Value);
					}
					else
					{
						_pvProxy.PhotonRpcBroadcastFsmEvent(_rpcTargets,remoteEvent.Name);
					}
				}
			}
			
		}
		
		
		
		public override string ErrorCheck()
		{
			if (eventTarget.target != FsmEventTarget.EventTarget.BroadcastAll && eventTarget.target != FsmEventTarget.EventTarget.GameObject )
			{
				return "eventTarget can be either BroadcastAll or GameObject ";	
			}

			if (eventTarget.excludeSelf.Value)
			{
				return "exclude Self has no effect in this case, send an rpc with RPCTarget set to Others";		
			}
			
			if (remoteEvent == null)
			{
				return "Remote Event not set";
			}
	
            return string.Empty;
		}
		
		
		
	}
}