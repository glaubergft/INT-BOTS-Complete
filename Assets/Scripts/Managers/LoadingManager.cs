using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class LoadingManager : MonoBehaviour
{
    private AsyncOperation async; //Hold the parameter from that scene
    
    [SerializeField]
    private Text txtPercent; //Textfield to display the progress

    [SerializeField]
    private bool waitForUserInput = false;

    private bool ready = false;

    [SerializeField]
    private float delay = 0; //How much time should I wait before I enable the next scene

    [SerializeField]
    private int sceneToLoad = -1; //If -1 = net scene, else load that scene number

    private void Start()
    {
        Time.timeScale = 1;
        Input.ResetInputAxes(); //Reset the input for 1 frame
        System.GC.Collect();
        Scene currentScene = SceneManager.GetActiveScene();
        if (sceneToLoad == -1)
            async = SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);
        else
            async = SceneManager.LoadSceneAsync(sceneToLoad);
        async.allowSceneActivation = false; //Wait for my OK to start the next scene.

        if (!waitForUserInput)
        {
            //ready = true;
            StartCoroutine(Activate(delay));
        }
    }

    IEnumerator Activate(float delay)
    {
        yield return new WaitForSeconds(delay * Time.timeScale);
        ready = true; //We call this function when we are ready to load the next scene.
    }

    private void Update()
    {

        if (waitForUserInput && Input.anyKey)
        {
            ready = true;
        }

        if (txtPercent)
        {
            txtPercent.text = $"{((async.progress + 0.1f) * 100).ToString("f2")} %";
        }

        if (async.progress >= 0.89f && SplashScreen.isFinished && ready)
        {
            async.allowSceneActivation = true;
        }
    }
}
