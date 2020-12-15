using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PanelTimesUp : Panel
{
    [SerializeField]
    float waitSecondsForGameOver;

    PanelTimesUp()
    {
        Opened += PanelTimesUp_Opened;
    }

    private void PanelTimesUp_Opened()
    {
        LeanTween.scale(gameObject, Vector3.one * 0.5f, waitSecondsForGameOver - 0.5f).setEase(LeanTweenType.punch);
        StartCoroutine(PanelTimesUp_Opened_Coroutine());
    }

    IEnumerator PanelTimesUp_Opened_Coroutine()
    {
        yield return new WaitForSeconds(waitSecondsForGameOver);
        Open<PanelGameOver>();
    }
}
