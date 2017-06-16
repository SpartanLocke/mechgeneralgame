using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Heavily borrowed from photon's sample.

public class JoinRandomRoom : Photon.MonoBehaviour
{
    public Text text;
    
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = true;

    public byte Version = 1;

    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;


    public virtual void Start()
    {
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
    }

    public virtual void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            textLog("Connecting To Server...");

            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings("test");
        }
    }


    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage


    public virtual void OnConnectedToMaster()
    {
        if (ApplicationManager.isPrivateMatch)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            textLog("Connected To Server. Matchmaking In Progress...");
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public virtual void OnJoinedLobby()
    {
        Debug.Log("Connected to lobby");
        PhotonNetwork.LoadLevel("PrivateMatch");
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
       // textLog("No Open Games. Creating a New Game.");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 2 }, null);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom()
    {
        //textLog("");
        if (PhotonNetwork.room.playerCount == 1)
        {
            textLog("Waiting for Another Player...");

        }
        else
        {
            textLog("Match Found! Game Starting Soon");
            ApplicationManager.localPlayerFaction = 1;
            PhotonNetwork.LoadLevel("Main");
        }

    }

    public void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        textLog("Someone else is here! Starting game now");
        ApplicationManager.localPlayerFaction = 0;
        PhotonNetwork.LoadLevel("Main");
    }

    void textLog(string t)
    {
        text.text += t;
        text.text += "\n";
        Debug.Log(t);
    }

}
