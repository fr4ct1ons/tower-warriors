using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PersistentAudioPlayer - Keeps an audio track playing when chaning scenes, unless the source has a different clip assigned. Then, it will destroy the previous instance.
/// Allows for multiple instances of this object.
/// </summary>
[RequireComponent(typeof(AudioSource))]    
public class PersistentAudioPlayer : MonoBehaviour
{
    /// <summary>
    /// Array with the instances of the PersistentAudioPlayers
    /// </summary>
    static private PersistentAudioPlayer[] instances = new PersistentAudioPlayer[10];
    
    [Tooltip("Index of the persistent instance to be stored.")]
    [SerializeField] private int playerIndex = 0;
    
    [Tooltip("AudioSource to be controlled.")]
    [SerializeField] private AudioSource source;
    
    [Tooltip("Set to true if it will always follow the main camera. Currently, does not get the camera on awake.")] 
    [SerializeField] private bool followMainCamera;

    [SerializeField] private Transform mainCam;

    public AudioSource Source
    {
        get => source;
        set => source = value;
    }
    /// <summary>
    /// Marks the object as persistent and destroys any previous existing instance in the instances array.[
    /// If the playerIndex is invalid (i.e is out of the array's bounds) then this instance disables itself.
    /// </summary>
    private void Awake()
    {
        if (playerIndex < 0 || playerIndex >= instances.Length)
        {
            Debug.LogError("This index is not valid. Object will be disabled.", gameObject);
            source.Pause();
            gameObject.SetActive(false);
        }

        if (!source)
        {
            source = GetComponent<AudioSource>();
        }
        if (instances[playerIndex])
        {
            if (instances[playerIndex].Source.clip == source.clip)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(instances[playerIndex].gameObject);
                instances[playerIndex] = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            instances[playerIndex] = this;
            DontDestroyOnLoad(gameObject);
        }

        if (followMainCamera)
        {
            mainCam = Camera.main.transform;
        }
    }

    /// <summary>
    /// Makes this object's transform.position equal to the main camera's transform.
    /// </summary>
    private void Update()
    {
        if (followMainCamera)
        {
            if(!mainCam)
                mainCam = Camera.main.transform;
            
            transform.position = mainCam.position;
        }
    }
}
