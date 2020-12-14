using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;

public class PanelOptions : Panel
{
    [SerializeField]
    GameManager gameManager = null;

    PanelOptions()
    {
        Opened += PanelQuit_Opened;
        Closed += PanelOptions_Closed;
    }

    private void PanelQuit_Opened()
    {
        gameManager.EnableMyCharacter(false);
    }

    private void PanelOptions_Closed()
    {
        print("PanelOptions_Closed");
        gameManager.EnableMyCharacter(true);
    }
}
