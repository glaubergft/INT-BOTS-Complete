using Photon.Pun;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    Transform spawnLocations;

    #endregion

    #region Private Variables

    PhotonView view;

    #endregion

    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (view.IsMine)
        {
            string prefabName = Path.Combine("MultiplayerPrefabs", "Player");

            //string spawnName = $"Spawn{ActorNumber()}";
            //var spawn = GameObject.Find(spawnName)?.transform;
            var spawn = spawnLocations.GetChild(view.Owner.ActorNumber - 1);
            if (spawn != null)
            {
                var playerGameObject = PhotonNetwork.Instantiate(prefabName, spawn.position, Quaternion.identity);
                playerGameObject.GetComponent<CharacterMovement>().InitialRotationY = spawn.eulerAngles.y;
                playerGameObject.GetComponent<CharacterCustomizer>().UpdateCharacter(Player.SelectedCharacter);
                FindObjectOfType<GameManager>().MyPlayer = playerGameObject.GetComponent<Player>();
                //FindObjectOfType<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                //FindObjectOfType<Canvas>().worldCamera = GetComponentInChildren<Camera>();
            }
            else
            {
                PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
            }
        }
    }
}
