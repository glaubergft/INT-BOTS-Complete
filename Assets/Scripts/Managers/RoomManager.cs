using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UINavigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    internal static RoomManager Instance = null;

    internal static byte MaxPlayers = 2;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public override void OnDisable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            string prefabName = Path.Combine("MultiplayerPrefabs", "PlayerManager");
            PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
        }
    }

    private void Start()
    {
        Connect();
    }

    internal void Connect()
    {
        Display("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //JoinLobby();
    }



    public void JoinLobby()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnJoinedLobby()
    {
        if (Panel.Find<PanelConnect>().IsOpened && PhotonNetwork.NetworkClientState == ClientState.JoinedLobby)
        {
            Display("Joined Lobby");
            FindObjectOfType<StartManager>().FindPlayers();
        }
    }

    public override void OnJoinedRoom()
    {
        Display("Waiting for Players...");
        if (!Panel.Find<PanelConnect>().IsOpened)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            CheckGameStart();
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //External player connected
        if (!Panel.Find<PanelConnect>().IsOpened)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            CheckGameStart();
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Display("Room Creation Failed: " + message);
    }

    public override void OnLeftRoom()
    {
        Display("Left Room");
    }

    private void Display(string info)
    {
        Debug.Log(info + " - PhotonNetwork.NetworkClientState:" + PhotonNetwork.NetworkClientState.ToString());
        if (Panel.Find<PanelConnect>() != null)
        {
            Panel.Find<PanelConnect>().lblStatus.text = info;
        }
    }

    private void CheckGameStart()
    {
        if (PhotonNetwork.PlayerList.Length == MaxPlayers) //MaxPlayers
        {
            FindObjectOfType<StartManager>().LoadGameScene();
        }
    }
}
