using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerCharacter;

    [SerializeField]
    PanelFade panelFadeWhiteTransition;

    [SerializeField]
    PanelFade panelFadeBlackTransition;

    [SerializeField]
    private AudioSource currentMusic;

    [SerializeField]
    private AudioSource gameStart;

    private bool loadingGame = false;

    private void Start()
    {
        PickRandomCharacter();
    }

    private void PickRandomCharacter()
    {
        var characterRepository = FindObjectOfType<CharacterRepository>();

        if (Player.SelectedCharacter == Character.None)
        {
            int rnd = Random.Range(0, characterRepository.AvailableCharacters.Length);
            Player.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
            Player.SelectedCharacter = characterRepository.AvailableCharacters[rnd].Character;
        }
        playerCharacter.GetComponent<CharacterCustomizer>().UpdateCharacter(Player.SelectedCharacter);
    }

    public void FindPlayers()
    {
        if (!PhotonNetwork.IsConnected)
        {
            RoomManager.Instance.Connect();
            return;
        }
        else if (!PhotonNetwork.InLobby)
        {
            RoomManager.Instance.JoinLobby();
            return;
        }

        Player.NickName = Panel.Find<PanelStart>().txtNickname.text;
        
        //*** MULTIPLAYER CODE
        
        //STEP 1: Setup basic information about your player: 
        PhotonNetwork.NickName = Player.NickName;
        PhotonNetwork.LocalPlayer.CustomProperties["SelectedCharacter"] = Player.SelectedCharacter;

        //STEP 2: Define the settings used when looking for a matching Room:
        OpJoinRandomRoomParams joinRoomParams = new OpJoinRandomRoomParams();
        joinRoomParams.ExpectedMaxPlayers = RoomManager.MaxPlayers;

        //STEP 3: Define the settings used when creating a Room:
        EnterRoomParams createRoomParams = new EnterRoomParams();
        createRoomParams.RoomOptions = new RoomOptions();
        createRoomParams.RoomOptions.MaxPlayers = RoomManager.MaxPlayers;

        //STEP 4: Find players. Photon will automatically join or create a room as needed:
        PhotonNetwork.NetworkingClient.OpJoinRandomOrCreateRoom(joinRoomParams, createRoomParams);
    }

    public void LoadGameScene()
    {
        if (loadingGame) { return; }

        PhotonNetwork.CurrentRoom.IsOpen = false;
        loadingGame = true;
        StartCoroutine(LoadGameScene_Coroutine());
    }

    private IEnumerator LoadGameScene_Coroutine()
    {
        currentMusic.Stop();
        gameStart.Play();
        panelFadeWhiteTransition.FadeIn();
        yield return new WaitForSeconds(panelFadeWhiteTransition.duration);
        PhotonNetwork.LoadLevel("Game");
    }
}
