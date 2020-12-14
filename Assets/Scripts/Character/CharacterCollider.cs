using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    Head,
    Body
}

public class CharacterCollider : MonoBehaviour
{
    #region Private Variables

    Player playerObj;

    #endregion

    public HitType type;

    private void Awake()
    {
        playerObj = gameObject.FindParentComponent<Player>();
    }

    public void CommunicateHit(Projectile projectileObj)
    {
        playerObj.TakeHit(projectileObj, type);
    }
}
