// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
	#pragma warning disable 649

   
	public class Launcher : MonoBehaviourPunCallbacks
    {

		#region Private Serializable Fields

		[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		[SerializeField]
		private GameObject controlPanel;

		[Tooltip("The Ui Text to inform the user about the connection progress")]
		[SerializeField]
		private Text feedbackText;

		[Tooltip("The maximum number of players per room")]
		[SerializeField]
		private byte maxPlayersPerRoom = 4;

		[Tooltip("The UI Loader Anime")]
		[SerializeField]
		private LoaderAnime loaderAnime;

		#endregion

		#region Private Fields
		/// <summary>
		/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
		/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
		/// Typically this is used for the OnConnectedToMaster() callback.
		/// </summary>
		bool isConnecting;

		/// <summary>
		/// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
		/// </summary>
		string gameVersion = "1";

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
		{
			if (loaderAnime==null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.",this);
			}

			PhotonNetwork.AutomaticallySyncScene = true;

		}

		#endregion


		#region Public Methods

		public void Connect()
		{
			feedbackText.text = "";

			isConnecting = true;

			controlPanel.SetActive(false);

			if (loaderAnime!=null)
			{
				loaderAnime.StartLoaderAnimation();
			}

			if (PhotonNetwork.IsConnected)
			{
				LogFeedback("Joining Room...");
				PhotonNetwork.JoinRandomRoom();
			}else{

				LogFeedback("Connecting...");
				
				PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
			}
		}

		
		void LogFeedback(string message)
		{
			if (feedbackText == null) {
				return;
			}

			feedbackText.text += System.Environment.NewLine+message;
		}

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks
        
        public override void OnConnectedToMaster()
		{
           .
			if (isConnecting)
			{
				LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
				Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
		
				PhotonNetwork.JoinRandomRoom();
			}
		}

		
		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
			Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

			PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
		}


		
		public override void OnDisconnected(DisconnectCause cause)
		{
			LogFeedback("<Color=Red>OnDisconnected</Color> "+cause);
			Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

			loaderAnime.StopLoaderAnimation();

			isConnecting = false;
			controlPanel.SetActive(true);

		}

		
		public override void OnJoinedRoom()
		{
			LogFeedback("<Color=Green>OnJoinedRoom</Color> with "+PhotonNetwork.CurrentRoom.PlayerCount+" Player(s)");
			Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
		
			if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
			{
				Debug.Log("We load the 'Room for 1' ");

				PhotonNetwork.LoadLevel("Room 1");

			}
		}

		#endregion
		
	}
}