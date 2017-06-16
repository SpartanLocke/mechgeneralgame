using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrivateMatchmaking : Photon.MonoBehaviour {
    // Use this for initialization
    public Text roomNumber;
	void Start () {
	
	}

    // Update is called once per frame



    public void CreateRoom()
    {
        int numGames = PhotonNetwork.countOfPlayers - 1;
        int roomName = Random.Range(10, 90) + numGames * 100;
        PhotonNetwork.CreateRoom(roomName.ToString(), new RoomOptions() { maxPlayers = 2, isVisible = false }, PhotonNetwork.lobby);
    }
    
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnPhotonCreateGameFailed()
    {
        Debug.Log("failed to create game");
    }

    public void OnPhotonJoinRoomFailed()
    {
        Debug.Log("join room failed. probably room doesn't exist, or is already full.");
        roomNumber.text = "join room failed. Room likely doesn't exist.";
        //handle front-end here.
    }

    public void OnCreatedRoom()
    {
        roomNumber.text = PhotonNetwork.room.name;

    }


    public void OnJoinedRoom()
    {
        if (PhotonNetwork.room.playerCount == 1)
        {
            roomNumber.text = "Room Created! Your Room Number: \n" + PhotonNetwork.room.name;
        }
        else
        {
            ApplicationManager.localPlayerFaction = 1;
            PhotonNetwork.LoadLevel("Main");
        }

    }

    public void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        ApplicationManager.localPlayerFaction = 0;
        PhotonNetwork.LoadLevel("Main");
    }

}
