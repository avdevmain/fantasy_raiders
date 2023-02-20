using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("PunRoomList")]
	[Tooltip("Gets Room Name, Player Count, Max Players, and isOpen at an index in the PlayMakerPhotonRoomListProxy.")]
	public class PunRoomListGetRoomData : FsmStateAction
	{
		[Tooltip("Refernece the PlaymakerPhotonRoomListProxy in your scene.")]
		public FsmGameObject RoomListProxy;

		[UIHint(UIHint.FsmInt)]
		[Tooltip("The index of the PlayMakerPhotonRoomListProxy to get room data from.")]
		public FsmInt index;

		[UIHint(UIHint.Variable)]
		[Tooltip("The room name.")]
		public FsmString roomName;

		[UIHint(UIHint.Variable)]
		[Tooltip("The room name.")]
		public FsmInt playerCount;

		[UIHint(UIHint.Variable)]
		[Tooltip("The room name.")]
		public FsmInt maxPlayers;

		[UIHint(UIHint.Variable)]
		[Tooltip("The room name.")]
		public FsmBool isOpen;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("The event to trigger if the action is successful.")]
        public FsmEvent successEvent;

        [UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("The Failure message.")]
        public FsmString failureString;

        private string arrayListIndexString;
		private PlayMakerArrayListProxy proxy;

		public override void Reset()
		{

			RoomListProxy = null;
			index = null;
			roomName = null;
			playerCount = null;
			maxPlayers = null;
			isOpen = false;
			successEvent = null;
			failureEvent = null;
			failureString = null;

		}

		// Code that runs on entering the state.
		public override void OnEnter()
		{
			object element = null;
			proxy = (RoomListProxy.Value).GetComponent<PlayMakerArrayListProxy>();

			try
			{
				element = proxy.arrayList[index.Value];
			}catch(System.Exception e)
            {
				failureString.Value = e.Message;
				if (failureString.Value == "Index was out of range. Must be non-negative and less than the size of the collection.\r\nParameter name: index")
				{
					failureString.Value = null;
					Fsm.Event(successEvent);
				}
				Fsm.Event(failureEvent);
				return;
            }
			arrayListIndexString = element.ToString();

			string[] splitArray = arrayListIndexString.Split(char.Parse(",")); //Here we are passing the splitted string to array by that char

			roomName.Value = splitArray[0]; 
			playerCount.Value = int.Parse(splitArray[1]);
			maxPlayers.Value = int.Parse(splitArray[2]);
			isOpen.Value = splitArray[3] == "True";

			Finish();
		}


	}

}
