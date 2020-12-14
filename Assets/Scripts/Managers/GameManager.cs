using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UINavigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    int pointsForHeadShot = 3;

    [SerializeField]
    int pointsForBodyShot = 1;

    [SerializeField]
    int matchDurationInSeconds = 0;

    [SerializeField]
    Image healthBar;

    [SerializeField]
    Text txtTimeLeft;

    [SerializeField]
    Text[] leaderboardNickname;

    [SerializeField]
    Text[] leaderboardScore;

    [SerializeField]
    AudioSource buttonClickSFX;

    [SerializeField]
    AudioSource buzzerSFX;

    [SerializeField]
    AudioSource alarmSFX;

    [SerializeField]
    AudioSource gameTheme;

    [SerializeField]
    GameObject disableCamera;

    [SerializeField]
    PanelFade panelFadeBlackTransition;

    #endregion

    #region Private Variables

    Dictionary<int, PlayerData> playerDictionary = new Dictionary<int, PlayerData>();

    bool gameOn = true;

    #endregion

    internal Player MyPlayer;

    class PlayerData
    {
        public int ActorNumber { get; set; }
        public Player Player { get; set; }
        public Text NicknameControl { get; set; }
        public Text ScoreControl { get; set; }
        private int score;
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
                if (ScoreControl != null) ScoreControl.text = score.ToString("0000");
            }
        }

    }

    private void Awake()
    {
        ClearLeaaderboard();
    }

    void ClearLeaaderboard()
    {
        leaderboardNickname.ToList().ForEach(x => x.text = string.Empty);
        leaderboardScore.ToList().ForEach(x => x.text = string.Empty);
    }

    void Start()
    {
        disableCamera.SetActive(false);
        InvokeRepeating(nameof(UpdateTimer), 0, 1);
    }

    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetButtonDown("Cancel") && Panel.CheckAllPanelsAreClosed())
        {
            buttonClickSFX?.Play();
            Panel.Open<PanelOptions>();
        }
    }

    void UpdateTimer()
    {
        matchDurationInSeconds--;
        txtTimeLeft.text = TimeSpan.FromSeconds(matchDurationInSeconds).ToString(@"mm\:ss");

        if (matchDurationInSeconds <= 10)
        {
            LeanTween.scale(txtTimeLeft.gameObject, Vector3.one * 0.5f, 0.5f).setEase(LeanTweenType.punch);
            alarmSFX.Play();
        }

        if (matchDurationInSeconds == 0)
        {
            TimesUp();
        }
    }

    void TimesUp()
    {
        CleanupForFinish();
        buzzerSFX.Play();
        Panel.Open<PanelTimesUp>();
    }

    internal void Quit()
    {
        CleanupForFinish();
        StartCoroutine(Quit_coroutine());
    }

    IEnumerator Quit_coroutine()
    {
        panelFadeBlackTransition.FadeIn();
        yield return new WaitForSeconds(panelFadeBlackTransition.duration + 0.1f);
        SceneManager.LoadScene("Loading");
    }

    void CleanupForFinish()
    {
        gameOn = false;
        PhotonNetwork.Disconnect();
        if (MyPlayer != null)
        {
            MyPlayer.GetComponentInChildren<Camera>().transform.parent = null;
            MyPlayer.Die(false);
        }
        Destroy(RoomManager.Instance);
        Destroy(GameObject.Find("RoomManager"));
        gameTheme.Stop();
        CancelInvoke(nameof(UpdateTimer));
    }

    internal void RegisterPlayer(int actorNumber, string nickname, Player playerObj)
    {
        if (!playerDictionary.ContainsKey(actorNumber))
        {
            var txtNickname = leaderboardNickname[actorNumber - 1];
            var txtScore = leaderboardScore[actorNumber - 1];
            var pData = new PlayerData()
            {
                ActorNumber = actorNumber,
                NicknameControl = txtNickname,
                ScoreControl = txtScore,
                Player = playerObj,
                Score = 0
            };
            pData.NicknameControl.text = nickname;
            playerDictionary.Add(actorNumber, pData);
            if (playerObj.IsMine)
            {
                leaderboardNickname[actorNumber - 1].fontStyle = FontStyle.Bold;
                leaderboardScore[actorNumber - 1].fontStyle = FontStyle.Bold;
            }
        }
    }

    internal void EnableMyCharacter(bool enabled)
    {
        if (!gameOn)
            return;

        MyPlayer?.EnableMyPlayer(enabled, enabled);
    }

    internal int ReturnPointsPerHitType(HitType hitType)
    {
        if (hitType == HitType.Body)
        {
            return pointsForBodyShot;
        }
        else if (hitType == HitType.Head)
        {
            return pointsForHeadShot;
        }
        else
        {
            return 0;
        }
    }

    internal void ComputeScoreHit(int actorNumber, HitType hitType)
    {
        int hitPoints = ReturnPointsPerHitType(hitType);
        if (playerDictionary.ContainsKey(actorNumber))
        {
            playerDictionary[actorNumber].Score += hitPoints;
            UpdateScoreboardRank();
        }
    }

    void UpdateScoreboardRank()
    {
        IEnumerable<PlayerData> playerList = playerDictionary.Values.OrderByDescending(x => x.Score);
        int i = 0;
        foreach (var entry in playerList)
        {
            entry.NicknameControl.transform.SetSiblingIndex(i);
            entry.ScoreControl.transform.SetSiblingIndex(i);
            i++;
        }
    }

    internal void UpdatePlayerHealthHUD(int healthValue)
    {
        healthBar.fillAmount = (float)healthValue / 10;
    }

    internal void UpdatePlayerHealthHUD(float healthValue)
    {
        healthBar.fillAmount = healthValue;
    }
}
