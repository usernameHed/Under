using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
edit:
https://www.draw.io/#G0Byzet-SVq6ipU1RYTGgwRFBGOW8
see:
https://drive.google.com/file/d/0Byzet-SVq6ipdC1DT29VTkJfTE0/view
*/

public class TrapsController : MonoBehaviour
{
    [Range(0, 10f)]    public float timeOpti = 0.1f;       //optimisation
    public enum TypeOfTraps                                 //enum type du piège
    {
        Force,
        Fire,
        Freez,
        Acid,
        Laser,
        Gaz,
    }
    public TypeOfTraps typeOfTraps;                         //type du piège
    public GameObject particles;                            //particule du piège

    [Space(10)]
    public bool desactiveOutOfScreen = true;                //se désactive (ou pas) s'il est en dehros de l'écran
    public bool onlyEggs = false;
    public bool notMyTeam = false;                          //uniquement pour les joueurs ennemi
    public int idTeam = 1;                                 //id de la team (si différent de -1);
    public bool miniAttractForTrail = false;                //spécificité de l'objet miniAttract pour les trails..
    public bool miniAttractEndTrail = false;                //est-ce que on est spécifiquement le mini attract de fin de trail ?
    public bool onYellow = false;
    public float divideForPlayer = 2;                        //réduit la poussé des joueurs
    public float addMultiplyForPushable = 1;
    public bool useRaycast = false;

    /// <summary>
    /// variable privé
    /// </summary>

    //ces 3 variable ci dessous sont changé grace a DrawSolidArc
    private float fovRange;                     //range du piège
    private float fovAngle;                     //angle du piège
    private float strengthOfAttraction;         //force du piège (si applicable)

    private int layerMask = 1 << 8;             //select layer 8 (metallica, players and colider)
    private float timeToGo;                     //optimisation
    private Collider[] gravityColliders;
    private int maxTab = 30;                    //max objectà trigger

    //private GameObject gameController;          //référence au gameController
    /// <summary>
    /// initialise le gameController
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// initialise l'optimisation
    /// </summary>
    void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
        getPublicVariable();
        gravityColliders = new Collider[maxTab];
    }

    /// <summary>
    /// change les 3 variable phares de la détection par rapport au script DrawSolidArc
    /// (celui ci sert à visualiser dans l'éditeur les variables)
    /// </summary>
    private void getPublicVariable()
    {
        if (gameObject.GetComponent<DrawSolidArc>())
        {
            fovRange = gameObject.GetComponent<DrawSolidArc>().fovRange;
            fovAngle = gameObject.GetComponent<DrawSolidArc>().fovAngle;
            strengthOfAttraction = gameObject.GetComponent<DrawSolidArc>().strengthOfAttraction;
        }
    }

    /// <summary>
    /// Vérifie l'angle entre 2 vecteur, avec un 3ème vecteur de référence
    /// </summary>
    /// <param name="a">vecrteur a</param>
    /// <param name="b">vecteur b</param>
    /// <param name="n">vecteur de référence n</param>
    /// <returns>différence entre l'angle a et b</returns>
    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        float angle = Vector3.Angle(a, b);                                  // angle in [0,180]
        float sign = 1;//Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));   //Cross for testing -1, 0, 1
        float signed_angle = angle * sign;                                  // angle in [-179,180]
        float angle360 = (signed_angle + 360) % 360;                        // angle in [0,360]
        return (angle360);                                                  //retourne la différence
    }

    /// <summary>
    /// Vérifie la colision entre le piège et les objets
    /// effectuer un Overlap sphere (tout les objet autour d'une sphere)
    /// puis effectue un test raycast + range + distance + tag
    /// </summary>
    void CheckCollision()
    {
        //capture tout objet dans le layer layerMask, dans la range
        int otherObject = Physics.OverlapSphereNonAlloc(gameObject.transform.position, fovRange, gravityColliders, layerMask);
        //Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, fovRange, layerMask);
        if (otherObject > maxTab)
            otherObject = maxTab;
        for (int i = 0; i < otherObject; i++)                               //parcourt la liste de ces objets
        {
            Collider hitCollider = gravityColliders[i];
            if (hitCollider.tag == "Eggs" || hitCollider.tag == "Player" || hitCollider.tag == "Pushable")   //si l'objets est un oeuf ou player
            {
                if (hitCollider.tag == "Eggs" && hitCollider.GetComponent<EggsController>().isGreenControlled)
                    continue;
                if (onlyEggs && hitCollider.tag == "Player")        //uniquement les oeuf sont ok
                    continue;
                if (hitCollider.tag == "Player")     //tout les player sauf...
                {
                    PlayerController PCtmp = hitCollider.GetComponent<PlayerController>();
                    if (PCtmp)
                    {
                        //if (PCtmp.getTypePowerPlayer() == 4)                                        //si c'est la fourmis Verte, ne rien faire
                          //  return;
                        if (notMyTeam && PCtmp.nb_team == idTeam)                                   //si l'id du pouvoir est le même que le joueur, ne rien faire
                            return;
                    }
                }
                //teste l'angle de l'objet par rapport à l'angle du piège
                Vector3 B = hitCollider.transform.position - gameObject.transform.position;
                Vector3 C = Quaternion.AngleAxis(90 + fovAngle / 2, -transform.forward) * -transform.right;
                if (SignedAngleBetween(transform.up, B, transform.up) <= SignedAngleBetween(transform.up, C, transform.up) || fovAngle == 360)
                {
                    Debug.DrawRay(transform.position, B, Color.red, 0.5f);
                    if (useRaycast)
                    {
                        //l'objet est dans l'angle, créé un raycast pour savoir si l'objet est en ligne de mire directe
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, B, out hit, layerMask))
                        {
                            //on est en lign ede mire, affiche le rayon (debug) et applique la réponse du piège selon le type d'objet ciblé
                            //Debug.DrawRay(transform.position, B, Color.red, 0.5f);
                            //if (!hitColliders[i].GetComponent<Rigidbody>())                                       //si l'objet n'a pas de RigidBody, erreur ?
                            //return;
                            ApplyObjectForce(hitCollider);
                        }
                    }
                    else
                        ApplyObjectForce(hitCollider);
                }
            }
        }
    }

    void ApplyObjectForce(Collider hitColliders)
    {
        switch (hitColliders.tag)
        {
            case "Eggs":
                ApplyEggs(hitColliders);
                break;
            case "Player":
                ApplyPlayer(hitColliders);
                break;
            case "Pushable":
                ApplyPushable(hitColliders);
                break;
        }
    }

    /// <summary>
    /// applique force sur pushable
    /// </summary>
    void ApplyPushable(Collider other)
    {
        Vector3 forceDirection = transform.position - other.transform.position;     //créé le vecteur directeur piège -> objet cible
        if (other.transform.gameObject.GetComponent<Rigidbody>() && other.transform.gameObject.GetComponent<Pushable>())
        {
            other.transform.gameObject.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * addMultiplyForPushable * forceDirection * other.transform.gameObject.GetComponent<Pushable>().factorMultiply);
            if (!miniAttractForTrail && onYellow)
                other.transform.gameObject.GetComponent<Pushable>().setControl(true, 2);
            else if (miniAttractForTrail)
                other.transform.gameObject.GetComponent<Pushable>().setControl(true, 3);
        }
    }

    /// <summary>
    /// Un oeuf est dans la ligne de mire
    /// </summary>
    /// <param name="other">l'oeuf en question</param>
    void ApplyEggs(Collider other)
    {
        Vector3 forceDirection = transform.position - other.transform.position;     //créé le vecteur directeur piège -> objet cible

        switch (typeOfTraps)
        {
            case TypeOfTraps.Force:                                                 //répulse/attire si on est de type Forces                                   
                if (miniAttractForTrail)
                {
                    //si c'est un mini attract, regarde si l'oeuf est déja controllé par le trail, si oui, n'applique pas de force
                    if (!other.GetComponent<EggsController>().isGreenControlled
                        /*&& !other.GetComponent<EggsController>().stopControlGreen*/)
                    {
                        //if (miniAttractEndTrail && other.GetComponent<EggsController>().stopControlGreen)
                            //break;

                        other.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * forceDirection);
                        //other.GetComponent<EggsController>().stopGreenWithCoroutine();
                        other.GetComponent<EggsController>().greenControlled(true);
                    }

                }
                else
                {
                    if (onYellow)
                        other.GetComponent<EggsController>().yellowControlled(true);
                    other.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * forceDirection);
                    other.GetComponent<EggsController>().checkIfIsPushWhenInTrail();
                }
                break;

            case TypeOfTraps.Gaz:                                                  //détruit l'oeuf et enlève du score au joueur
                EggsController EC = other.GetComponent<EggsController>();
                if (EC)
                {
                    Debug.Log("gaz ?");
                    //EC.substractScoreWhenDing();                                    //supprime le score de cette oeuf au joueur s'il a trahi
                    EC.destroyThis();                                               //supprime cet oeuf
                }
                break;
        }
    }

    /// <summary>
    /// un joueur est dans la ligne de mire
    /// </summary>
    /// <param name="other">le joueur en question</param>
    void ApplyPlayer(Collider other)
    {
        Vector3 forceDirection = transform.position - other.transform.position;     //créé le vecteur directeur piège -> objet cible


        switch (typeOfTraps)
        {
            case TypeOfTraps.Force:                                                 //répulse/attire si on est de type Forces   
                //Debug.Log("ici un player !!!!!!!!!");
                if (other.GetComponent<PlayerController>() && other.GetComponent<Rigidbody>())
                    other.GetComponent<PlayerController>().pushAway((strengthOfAttraction * ((notMyTeam) ? 1f : 1)) * (other.GetComponent<Rigidbody>().mass / divideForPlayer) * forceDirection * Time.deltaTime * 90);
                //other.GetComponent<PlayerController>().GetComponent<Rigidbody>().AddForce((strengthOfAttraction * ((notMyTeam) ? 3f : 1)  ) * forceDirection);
                break;
            case TypeOfTraps.Gaz:                                                  //détruit le joueur, active vibration
                if (other.GetComponent<PlayerController>())
                {
                    other.GetComponent<PlayerController>().setSlowGaz();
                }
                break;
        }
    }

    /// <summary>
    /// désactive le piège s'il est hors de l'écran (grace a IsOnScreen, avec une marge d'erreur)
    /// </summary>
    /// <returns></returns>
    private bool testToDesactivate()
    {
        if (desactiveOutOfScreen && gameObject.GetComponent<IsOnScreen>() && !gameObject.GetComponent<IsOnScreen>().isOnScreen)
        {
            timeToGo = Time.fixedTime + timeOpti;
            if (particles)
                particles.SetActive(false);
            return (false);
        }
        if (particles && !particles.activeSelf)
            particles.SetActive(true);
        return (true);
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo && testToDesactivate())
        {
            if (typeOfTraps != TypeOfTraps.Acid)
                CheckCollision();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
