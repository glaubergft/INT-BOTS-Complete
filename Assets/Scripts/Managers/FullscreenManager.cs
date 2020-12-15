using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<Toggle>().isOn = SavedOption();
    }

    public void ToggleChanged(bool value)
    {
        UpdateOption(GetComponent<Toggle>().isOn);
#if UNITY_EDITOR
        UnityEditor.EditorWindow.focusedWindow.maximized = value;
#else
        Screen.fullScreen = value;
        if (value)
        {
            var maxResolution = Screen.resolutions[Screen.resolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
#endif
    }

    private void UpdateOption(bool value)
    {
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    public static bool SavedOption()
    {
        return PlayerPrefs.GetInt("Fullscreen", 0) == 1;
    }
}
