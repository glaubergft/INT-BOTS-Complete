using Photon.Pun;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private Transform spawnLocations;

    #endregion

    #region Private Variables

    private PhotonView view;

    #endregion

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (view.IsMine)
        {
            string prefabName = Path.Combine("MultiplayerPrefabs", "Player");

            var spawn = spawnLocations.GetChild(view.Owner.ActorNumber - 1);
            if (spawn != null)
            {
                var playerGameObject = PhotonNetwork.Instantiate(prefabName, spawn.position, Quaternion.identity);
                playerGameObject.GetComponent<CharacterMovement>().InitialRotationY = spawn.eulerAngles.y;
                playerGameObject.GetComponent<CharacterCustomizer>().UpdateCharacter(Player.SelectedCharacter);
                FindObjectOfType<GameManager>().MyPlayer = playerGameObject.GetComponent<Player>();
            }
            else
            {
                PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
            }
        }
    }
}
