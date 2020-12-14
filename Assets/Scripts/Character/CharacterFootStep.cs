using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
public class CharacterFootStep : MonoBehaviour
{
    [SerializeField]
    AudioClip audioClip;

    [SerializeField]
    AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    void OnTriggerEnter(Collider other)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
