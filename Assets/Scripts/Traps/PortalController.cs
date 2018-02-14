using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/*
edit:
https://www.draw.io/#G0Byzet-SVq6ipSzFzUFZjcl9LTDg
see:
https://drive.google.com/file/d/0Byzet-SVq6ipczlJWmw4LThuSEk/view
*/
/// <summary>
/// Gère la téléportation des objets qui entre dans les portals
/// Un portal à un id et une cible goTo.
/// Lorsqu'un objet entre dans le portal, celui-ci le téléporte à la cible goTo.
///     Si goTo = -1, la cible est aléatoire. 
///     Pour les joueurs, il y a un temps d'attentre entre 2 téléportations
///     Pour les oeufs, les portal les propules une fois de l'autre coté
/// </summary>

public class PortalController : MonoBehaviour
{
    public bool aspire = true;                                              //défini si le portal peu aspirer ou pas
    [Range(-500f, 500f)]    public float strenghtOfRepulsion = 300.0f;      //force de répulsion

    [DisableIf("aspire")]
    public Transform spawnLocation;                                          //ref du second portal

    [EnableIf("aspire")]
    public Transform targetLocation;                                          //ref du second portal

    /// <summary>
    /// Parcourt la liste les Portals, quand on trouve celui avec le même id que celui qu'on cherche,
    /// on téléporte, et on applique une répulsion
    /// </summary>
    /// <param name="idTarget">le portal cible</param>
    /// <param name="other">l'objet à téléporter</param>
    private void goToTarget(Collider other)
    {
        if (targetLocation)
        {
            Rigidbody Rbother = other.GetComponent<Rigidbody>();
            //change de position sur le spawnLocation cible, et effectue une force (pour le joueur, cela ne va pas marcher.
            other.transform.position = new Vector3(targetLocation.position.x, targetLocation.position.y, 0.0f);
            //if (other.tag != "Pushable")
            //{
                Vector3 moveDir = (targetLocation.position - other.transform.position).normalized;
                Rbother.velocity = moveDir * strenghtOfRepulsion * Time.deltaTime;
                Rbother.angularVelocity = moveDir * strenghtOfRepulsion * Time.deltaTime;
            //}
        }
    }

    /// <summary>
    /// gère les exeptions pour la téléportation des joueurs
    /// - Si on est en mode solo, tout les joueurs peuvent 
    /// - Si on est en mode multi, uniquement les joueurs de la team du portals peuvent
    /// <summary>
    bool checkIfTeleport(Collider other)
    {
        return (true);
        //if (gameManager && !gameManager.multi)                              //Si on est en mode solo
          //  return (true);                                                  //tous les joueurs sont autorisé

        //////////////////////////////////////////////////////////////////// ici on est en mode multi
        /*
        PlayerController PCtmp = other.GetComponent<PlayerController>();
        if (PCtmp)
        {
            if (teamPortal == -1)
                Debug.LogError("la team du portal n'a pas été définit !");
            if (PCtmp.nb_team == teamPortal)                                //si la team courante du portal est de la team du joueur, OK
                return (true);
        }*/
        return (false);
    }

    /// <summary>
    /// détecte si un objet oeuf ou player collide, et effectue une téléportation
    /// Si c'est un joueur, vérifie si isOk de TimeWithNoEffect est ok
    /// </summary>
    /// <param name="other">l'objet à téléporter</param>
    private void OnTriggerStay(Collider other)
    {
        if (!aspire)
            return;
        if (other.tag == "Eggs" || other.tag == "Pushable")                                            //si c'est un oeuf, téléporte
            goToTarget(other);//HandleTeleport(other);
        else if (other.tag == "Player")                                     //si c'est un joueur, téléporte SI isOK est vrai
        {
            TimeWithNoEffect TWNE = other.GetComponent<TimeWithNoEffect>(); //récupère le script du temps

            if (!checkIfTeleport(other))                                    //si les conditions sont défavorable à la téléportation, quitter
                return;
            
            if (TWNE && TWNE.isOk)                                          //si isOk est vrai...
            {
                TWNE.isOk = false;                                          //le mettre, à faux, il va se remettre à vrai dans X seconde !
                goToTarget(other);//HandleTeleport(other);                                      //téléporte le joueur
            }
        }
    }
}
