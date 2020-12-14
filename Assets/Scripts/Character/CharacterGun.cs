using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGun : MonoBehaviourPun
{
    #region Serialized Fields

    [SerializeField]
    Transform characterCamera;

    [SerializeField]
    Transform launcher;

    [SerializeField]
    GameObject projectile;

    [SerializeField]
    float fireRate = 0.25f;

    #endregion

    #region Private Variables

    float lastFire = 0;

    PhotonView view;

    #endregion

    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && lastFire + fireRate <= Time.time)
        {
            lastFire = Time.time;
            Shoot();
        }
    }

    internal void Shoot()
    {
        string projectileId = Guid.NewGuid().ToString();
        if (PhotonNetwork.InRoom && view.IsMine)
        {
            view.RPC("ExecuteShoot", RpcTarget.All, projectileId, view.Owner.ActorNumber);
        }
        else if (!PhotonNetwork.IsConnected)
        {
            ExecuteShoot(projectileId, 0);
        }

    }

    [PunRPC]
    private void ExecuteShoot(string projectileId, int actorNumber)
    {
        GameObject instance = Instantiate(projectile, launcher.position, launcher.rotation);
        instance.transform.position = launcher.position;
        instance.transform.forward = characterCamera.forward;
        instance.GetComponent<Projectile>().ProjectileId = projectileId;
        instance.GetComponent<Projectile>().ActorNumber = actorNumber;
    }
}
