using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopScript : MonoBehaviour
{

    public GameObject chosenEffect;
    public float startTime = 0.0f;
    public float loopTimeLimit = 0.0f;
    public float duration = 2.0f;
    private ParticleSystem ps;

    IEnumerator Start()
    {
        ps = chosenEffect.GetComponent<ParticleSystem>();
        changeDurationParticle();
        yield return new WaitForSeconds(startTime);
        PlayEffect();
    }

    void changeDurationParticle()
    {
        ps.Stop(); // Cannot set duration whilst particle system is playing
        var main = ps.main;
        main.duration = duration;
        ps.Play();
    }

    public void PlayEffect()
    {
        StartCoroutine("EffectLoop");
    }


    IEnumerator EffectLoop()
    {
        yield return new WaitForSeconds(loopTimeLimit);
        //GameObject effectPlayer = (GameObject)Instantiate(chosenEffect, transform.position, transform.rotation);
        chosenEffect.SetActive(false);
        chosenEffect.SetActive(true);
        //yield return new WaitForSeconds(loopTimeLimit);

        //Destroy(effectPlayer);
        //chosenEffect.SetActive(false);
        //PlayEffect();
    }

    public void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())                              //si la particule ne joue pas
            {
                PlayEffect();
            }
        }
    }
}
