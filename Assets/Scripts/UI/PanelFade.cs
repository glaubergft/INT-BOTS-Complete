using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class PanelFade : MonoBehaviour
{
    public enum FadeState
    {
        None,
        FadeIn,
        FadeOut
    }

    //[SerializeField]
    //private Image backgroundImage;

    //[SerializeField]
    //private Color color;

    public float duration;

    [SerializeField]
    private FadeState startState;

    [SerializeField]
    private AudioSource audioAction;

    [SerializeField]
    private bool interactable;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //backgroundImage.color = color;
        if (startState == FadeState.FadeIn)
        {
            FadeIn();
        }
        else if (startState == FadeState.FadeOut)
        {
            FadeOut();
        }
    }

    public void FadeIn()
    {
        Initialize();
        canvasGroup.alpha = 0;
        LeanTween.alphaCanvas(canvasGroup, 1f, duration);
    }
    
    public void FadeOut()
    {
        Initialize();
        canvasGroup.alpha = 1;
        LeanTween.alphaCanvas(canvasGroup, 0f, duration);
        StartCoroutine(FadeOut_coroutine());
    }

    IEnumerator FadeOut_coroutine()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

    private void Initialize()
    {
        gameObject.SetActive(true);
        if (audioAction != null)
        {
            audioAction.Play();
        }
        canvasGroup.interactable = interactable;
    }
}
