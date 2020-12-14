using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PanelConnect : Panel
{
    public Text lblStatus = null;

    PanelConnect()
    {
        Closed += PanelConnect_Closed;
    }

    private void PanelConnect_Closed()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
