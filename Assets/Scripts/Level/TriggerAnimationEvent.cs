using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimationEvent : MonoBehaviour
{
    private Animator anim;                                                  //animator du joueur
    [SerializeField] private FmodEventEmitter emitterScript;          //l'emmiter pour tween

    // Use this for initialization
    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        if (!emitterScript)
            Debug.LogError("nop");
    }

    /// <summary>
    /// vérifie si on est pas en train de courrir, si non, une fois sur 10, lancer l'annimation twin !
    /// </summary>
    public void HandleTwinning()
    {
        if (anim.GetBool("isWalking"))      //si on marche, annuler
            return;
        if (Random.value < 0.15f)
        {
            anim.SetBool("isTwinning", true);                //lance l'animation de marche
            if (SoundManager.SS)
                SoundManager.SS.playSound(emitterScript);
        }
            
    }

    public void StopTwinning()
    {
        anim.SetBool("isTwinning", false);                //lance l'animation de marche
    }
}
