using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathMeshRendererSounds: MonoBehaviour {
    public AudioClip splash = null;
    public AudioClip emerging = null;
    public Action callbackCollision = null;

    void Start()
    {
        splash = (AudioClip) Resources.Load("meshSplash");
        emerging = (AudioClip) Resources.Load("meshEmerging");
        GetComponent<AudioSource>().PlayOneShot(emerging);        
    }

    public void CallbackCollision(Action callback)
    {
        callbackCollision = callback;
    }

	void OnCollisionEnter()
     {
         GetComponent<AudioSource>().PlayOneShot(splash);

         if (callbackCollision != null)
            callbackCollision();
     }
}
