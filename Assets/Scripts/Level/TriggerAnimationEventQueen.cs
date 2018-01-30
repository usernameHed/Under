using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimationEventQueen : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void StopGetEggs()
    {
        anim.SetBool("getEggs", false);
    }

    public void StopWin()
    {
        anim.SetBool("win", false);
    }
}
