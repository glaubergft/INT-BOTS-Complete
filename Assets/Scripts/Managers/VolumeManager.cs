using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Slider))]
public class VolumeManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer; 

    [SerializeField]
    private string parameter;

    [SerializeField]
    private AudioSource testAudioClip;

    private Slider slide = null; 

    private void Awake()
    {
        slide = GetComponent<Slider>();
    }

    public void SetVolume(float value)
    {
        SetVolume(audioMixer, parameter, value);
    }

    public void SetVolume(AudioMixer audioMixer, string parameter, float value)
    {
        audioMixer.SetFloat(parameter, value);

        if (testAudioClip != null && !testAudioClip.isPlaying)
        {
            testAudioClip.Play();
        }
    }

}
