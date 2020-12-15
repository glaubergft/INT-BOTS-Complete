using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    private CharacterRepository characterRepository;

    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        characterRepository = FindObjectOfType<CharacterRepository>();
    }
    
    public void UpdateCharacter(Character selectedCharacter)
    {
        if (PhotonNetwork.InRoom && view.IsMine)
        {
            view.RPC("ExecuteUpdateCharacter", RpcTarget.All, selectedCharacter);
        }
        else
        {
            ExecuteUpdateCharacter(selectedCharacter);
        }
    }

    [PunRPC]
    public void ExecuteUpdateCharacter(Character selectedCharacter)
    {
        var material = characterRepository.AvailableCharacters.Where(x2 => x2.Character == selectedCharacter).First().Material;
        Renderer[] rendererArray = GetComponentsInChildren<Renderer>();
        foreach (var renderer in rendererArray)
        {
            if (!renderer.CompareTag("IgnoreCustomization"))
            {
                renderer.sharedMaterial = material;
            }
        }
    }
}
