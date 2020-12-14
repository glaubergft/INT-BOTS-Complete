﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    float speed = 100;

    [SerializeField]
    GameObject impactFx;

    #endregion

    #region Private Variables

        string projectileId = string.Empty;

        int actorNumber = -1;

    #endregion

    public string ProjectileId 
    { 
        get => projectileId;
        set 
        { 
            projectileId = projectileId == string.Empty ? value : projectileId; //The Id can only be set once
        }
    }

    internal int ActorNumber
    { 
        get => actorNumber;
        set
        {
            actorNumber = actorNumber == -1 ? value : actorNumber; //The ActorNumber can only be set once
        }
    }

    

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ManageImpact();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerCollider"))
        {
            other.gameObject.GetComponent<CharacterCollider>().CommunicateHit(this);
        }
        ManageImpact();
    }

    void ManageImpact()
    {
        Instantiate(impactFx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
