using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelGameOver : Panel
{
    [SerializeField]
    AudioSource gameOverMusic = null;

    [SerializeField]
    Text[] leaderboardNicknameRef;

    [SerializeField]
    Text[] leaderboardScoreRef;

    [SerializeField]
    Text[] leaderboardNickname;

    [SerializeField]
    Text[] leaderboardScore;

    [SerializeField]
    PanelFade panelFadeBlackTransition;

    PanelGameOver()
    {
        Opened += PanelGameOver_Opened;
    }

    private void PanelGameOver_Opened()
    {
        gameOverMusic.Play();

        for (int i = 0; i < leaderboardNickname.Length; i++)
        {
            leaderboardNickname[i].text = leaderboardNicknameRef[i].text;
            leaderboardNickname[i].transform.SetSiblingIndex(leaderboardNicknameRef[i].transform.GetSiblingIndex());
            leaderboardScore[i].text = leaderboardScoreRef[i].text;
            leaderboardScore[i].transform.SetSiblingIndex(leaderboardScoreRef[i].transform.GetSiblingIndex());
        }

        GetComponent<CanvasGroup>().alpha = 0;

        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1f, 3);

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    public void btnMainMenu_Click()
    {
        StartCoroutine(LoadStartScene_Coroutine());
    }

    IEnumerator LoadStartScene_Coroutine()
    {
        GetComponent<CanvasGroup>().interactable = false;
        panelFadeBlackTransition.FadeIn();
        yield return new WaitForSeconds(panelFadeBlackTransition.duration + 0.1f);
        SceneManager.LoadScene("Loading");
    }
}
