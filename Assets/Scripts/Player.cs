using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region STATIC AREA

    static string nickName;

    internal static string NickName
    {
        get => nickName;
        set
        {
            nickName = value.Length > 25 ? value.Substring(0, 25) : value;
        }
    }

    internal static Character SelectedCharacter { get; set; }

    #endregion

    #region Serialized Fields

    [SerializeField]
    CameraShake cameraShakeObj;

    [SerializeField]
    Color emissionColorWhenHit;

    [SerializeField]
    GameObject deathExplosionFX;

    [SerializeField]
    GameObject spawnFX;

    [SerializeField]
    Transform spawnLocations;

    [SerializeField]
    AudioSource reloadHealthBar;

    [SerializeField]
    Transform playerLabel;

    #endregion

    #region Private Variables

    Renderer[] rendererArray;

    Renderer[] customRendererArray;

    Dictionary<string, Color> emissionColorDic = new Dictionary<string, Color>();

    Dictionary<string, Texture> emissionTextureDic = new Dictionary<string, Texture>();

    GameManager gameManagerObj;

    const int maxHealth = 10;

    int health = maxHealth;

    PhotonView view;

    HashSet<string> takenHits = new HashSet<string>();

    #endregion

    internal bool IsMine
    {
        get
        {
            return view.IsMine || !PhotonNetwork.IsConnected;
        }
    }

    void Awake()
    {
        gameManagerObj = FindObjectOfType<GameManager>();
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            var view = GetComponent<PhotonView>();
            name = $"Player{view.Owner.ActorNumber}";

            //Register this player in the GameManager:
            gameManagerObj.RegisterPlayer(view.Owner.ActorNumber, view.Owner.NickName, this);

            if (!view.IsMine)
            {
                SetPlayerLabel(view.Owner.NickName);
            }
        }
       
        SaveMaterialState();
    }

    void SetPlayerLabel(string nickName)
    {
        playerLabel.GetComponent<TextMesh>().text = nickName;
    }

    void SaveMaterialState()
    {
        rendererArray = GetComponentsInChildren<Renderer>();
        customRendererArray = GetComponentsInChildren<Renderer>().Where(x => x.tag != "IgnoreCustomization").ToArray();
        foreach (var renderer in customRendererArray)
        {
            string name = renderer.name;
            if (!emissionColorDic.ContainsKey(name))
            {
                emissionColorDic.Add(name, renderer.material.GetColor("_EmissionColor"));
                emissionTextureDic.Add(name, renderer.material.GetTexture("_EmissionMap"));
            }
        }
    }

    private void Update()
    {
        playerLabel.rotation = Camera.main.transform.rotation; // Causes the text faces the camera.
        
    }

    internal void TakeHit(Projectile projectileObj, HitType hitType)
    {
        TakeHit(projectileObj.ProjectileId, projectileObj.ActorNumber, hitType);
    }

    void TakeHit(string projectileId, int projectileAuthorActorNumber, HitType hitType)
    {
        if (takenHits.Contains(projectileId))
            return;

        takenHits.Add(projectileId);
        StartCoroutine(RemoveHitFromLog(projectileId));

        view.RPC("BroadcastHit", RpcTarget.Others, projectileId, projectileAuthorActorNumber, hitType);

        gameManagerObj?.ComputeScoreHit(projectileAuthorActorNumber, hitType);
        cameraShakeObj?.Shake();
        StartCoroutine(TakeHit_FX());
        ReduceHealth(hitType);
    }

    [PunRPC]
    void BroadcastHit(string projectileId, int projectileAuthorActorNumber, HitType hitType)
    {
        TakeHit(projectileId, projectileAuthorActorNumber, hitType);
    }

    IEnumerator TakeHit_FX()
    {
        foreach (var renderer in customRendererArray)
        {
            renderer.material.SetColor("_EmissionColor", emissionColorWhenHit);
            renderer.material.SetTexture("_EmissionMap", null);
        }

        yield return new WaitForSeconds(0.05f);

        foreach (var renderer in customRendererArray)
        {
            string name = renderer.name;
            var originalColor = emissionColorDic[name];
            renderer.material.SetColor("_EmissionColor", originalColor);
            var originalTexture = emissionTextureDic[name];
            renderer.material.SetTexture("_EmissionMap", originalTexture);
        }
    }

    IEnumerator RemoveHitFromLog(string projectileId)
    {
        yield return new WaitForSeconds(5);
        takenHits.Remove(projectileId);
    }

    void ReduceHealth(HitType hitType)
    {
        int hitPoints = gameManagerObj.ReturnPointsPerHitType(hitType);
        health -= hitPoints;
        if (health <= 0)
        {
            health = 0;
            Die(true);
        }
        if (IsMine)
        {
            gameManagerObj.UpdatePlayerHealthHUD(health);
        }
    }

    internal void Die(bool canRespawn)
    {
        if (IsMine)
        {
            EnableMyPlayer(false, false);
            GetComponent<CharacterGun>().enabled = false;
        }

        foreach (var renderer in rendererArray)
        {
            renderer.enabled = false;
        }

        GetComponentsInChildren<CharacterCollider>().ToList().ForEach(x => x.GetComponent<Collider>().enabled = false);

        Instantiate(deathExplosionFX, transform.position, transform.rotation);

        if (IsMine && canRespawn)
        {
            StartCoroutine(RespawnStep1());
        }
    }

    internal void EnableMyPlayer(bool movement, bool gun)
    {
        var characterMovement = GetComponent<CharacterMovement>();
        if (movement)
        {
            characterMovement.TrapCursor();
        }
        else
        {
            characterMovement.ReleaseCursor();
        }
        characterMovement.enabled = movement;
        GetComponent<CharacterGun>().enabled = gun;
    }

    IEnumerator RespawnStep1()
    {
        yield return new WaitForSeconds(2);

        Transform rndSpawn = spawnLocations.GetChild(Random.Range(0, spawnLocations.childCount));
        GetComponent<CharacterController>().enabled = false;
        transform.position = rndSpawn.position;
        GetComponent<CharacterController>().enabled = true;
        GetComponent<CharacterMovement>().InitialRotationY = rndSpawn.eulerAngles.y;

        yield return new WaitForSeconds(1);

        if (PhotonNetwork.InRoom && view.IsMine)
        {
            view.RPC("RespawnStep2", RpcTarget.All);
        }
        else
        {
            RespawnStep2();
        }
    }

    [PunRPC]
    void RespawnStep2()
    {
        StartCoroutine(RespawnStep3());
    }

    IEnumerator RespawnStep3()
    {
        Instantiate(spawnFX, transform.position, transform.rotation);

        float reloadDelayInSecods = 3;
        LeanTween.value(gameObject, ReloadHealth, 1, 3, reloadDelayInSecods).setEase(LeanTweenType.easeInCubic);

        if (IsMine)
        {
            gameManagerObj.UpdatePlayerHealthHUD(health);
            reloadHealthBar.Play();
            EnableMyPlayer(true, false);
        }

        yield return new WaitForSeconds(reloadDelayInSecods + 0.25f);

        if (IsMine) GetComponent<CharacterGun>().enabled = true;

        health = maxHealth;
        reloadHealthBar.Stop();

        GetComponentsInChildren<CharacterCollider>().ToList().ForEach(x => x.GetComponent<Collider>().enabled = true);
    }

    void ReloadHealth(float value, float ratio)
    {
        reloadHealthBar.pitch = value;

        bool blink = (int)(ratio * 100f) % 2 == 0;
        rendererArray.ToList().ForEach(x => x.enabled = blink);

        if (IsMine)
        {
            gameManagerObj.UpdatePlayerHealthHUD(ratio);
        }
    }
}
