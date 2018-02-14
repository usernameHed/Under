using UnityEngine;
using System.Collections;

/// <summary>
/// Ce script doit être attaché à un système de particule
/// Il permet de s'auto-détruire quand l'effet de particule a fini
/// </summary>

public class AutoDestroy : MonoBehaviour
{
    public bool justInactive = true;
    private ParticleSystem ps;
    public bool destroyWithNoPS = false;
    public float timeBeforeDestroy = 3f;

    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
        if (destroyWithNoPS)
            Destroy(gameObject, timeBeforeDestroy);
    }

    public void Update()
    {
        if (ps && !destroyWithNoPS)
        {
            if (!ps.IsAlive())                              //si la particule ne joue pas, détruire
            {
                if (justInactive)
                    gameObject.SetActive(false);
                else
                    Destroy(gameObject);
            }
        }
    }
}