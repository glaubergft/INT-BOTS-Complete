using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoQuality : MonoBehaviour
{
    public Text fpsText;
    public float deltaTime;
    public int threshold = 15;
    float fps;
    private bool downgraded = false;
    private void Start()
    {
        InvokeRepeating("CheckFps",1,1);
    }

    private void CheckFps()
    {
        fpsText.text = $"{Mathf.Ceil(fps).ToString()} fps";

        if (fps <= threshold && !downgraded)
        {
            QualitySettings.SetQualityLevel(0, true);
            downgraded = true;
        }
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        
    }
}
