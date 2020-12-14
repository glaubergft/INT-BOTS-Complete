using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;

public class PanelQuit : Panel
{
    [SerializeField]
    GameManager gameManager = null;

    PanelQuit()
    {
        Opened += PanelQuit_Opened;
        Closed += PanelQuit_Closed;
    }

    void PanelQuit_Opened()
    {
        gameManager.EnableMyCharacter(false);
    }

    void PanelQuit_Closed()
    {
        gameManager.EnableMyCharacter(true);
    }

    public void btnYes_Click()
    {
        gameManager.Quit();
    }
}
