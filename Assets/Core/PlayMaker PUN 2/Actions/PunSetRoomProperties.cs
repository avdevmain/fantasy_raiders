
using Photon.Pun;
using Photon.Realtime;

namespace HutongGames.PlayMaker.Pun2.Actions{
public class PunSetRoomProperties : FsmStateAction
{

		[Tooltip("Defines if the room can be joined. If not open, the room is excluded from random matchmaking. \n" +
			"This does not affect listing in a lobby but joining the room will fail if not open.")]
		public FsmBool open;

        [Tooltip("Send this event if the room properties were set.")]
		public FsmEvent successEvent;
		
		[Tooltip("Send this event if the room properties access failed")]
		public FsmEvent failureEvent;

        public override void Reset()
		{
            open = null;
            successEvent = null;
			failureEvent = null;
        }
		public override void OnEnter()
		{
			bool ok = setRoomProperties();
			
			
			if (ok)
			{
				Fsm.Event(successEvent);
			}else{
				Fsm.Event(failureEvent);
			}
			
			Finish();
		}

        bool setRoomProperties()
		{
			Room _room = PhotonNetwork.CurrentRoom;

            _room.IsOpen = open.Value;

            return true;
        }

}
}