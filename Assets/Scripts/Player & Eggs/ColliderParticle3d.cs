using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class ColliderParticle3d : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public GameObject refObject;                                            //reference de l'objet
    public enum TypeOfObject                                                //type d'objet
    {
        Eggs = 1,
        Player = 2,
        Other = 3,
        ParticlePortal = 4,
    }
    public TypeOfObject TP = TypeOfObject.Eggs;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable

    #endregion

    #region  initialisation

    #endregion

    #region core script

    #endregion

    #region unity fonction and ending
    /// <summary>
    /// colision des particules
    /// </summary>
    private void OnParticleCollision(GameObject other)
    {
        if (!refObject)
            return;
        Debug.Log(other.name);
        if (other.tag == "EvilParticle")                                              //particule destructible
        {
            //Debug.Log("ici");
            switch (TP)
            {
                case TypeOfObject.Eggs:
                    refObject.GetComponent<EggsController>().destroyThis();
                    break;
                case TypeOfObject.Player:
                    refObject.GetComponent<PlayerController>().destroyThis();
                    break;
            }
        }
        else if (other.tag == "FreezParticle")
        {
            switch (TP)
            {
                case TypeOfObject.Eggs:
                    EggsController EC2 = refObject.GetComponent<EggsController>();          //défini la référence de l'eggs controller de other
                    if (EC2 && !refObject.GetComponent<EggsController>().isFirstInfected
                        && !refObject.GetComponent<EggsController>().isGreenControlled
                        && !refObject.GetComponent<EggsController>().stopControlGreen)   //si l'oeuf est infecté, ne se met pas kinematic
                    {
                        //EC2.substractScoreWhenDing();                                   //supprime le score de cette oeuf au joueur
                        refObject.GetComponent<EggsController>().setKinetic(true);
                    }

                    break;
                case TypeOfObject.Player:
                    if (refObject.GetComponent<PlayerController>() && !refObject.GetComponent<PlayerController>().isSlow
                        && refObject.GetComponent<PlayerController>().getTypePowerPlayer() != 4)
                    {
                        //PC.VibrationGamePad(other.GetComponent<PlayerController>().nb_player, 0, other.GetComponent<PlayerController>().vibrateRightWhenSlow, other.GetComponent<PlayerController>().BothSlowTiming);
                        StartCoroutine(refObject.GetComponent<PlayerController>().setSlow());
                    }

                    break;
            }
        }
    }
    #endregion
}
