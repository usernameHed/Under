using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Cyborg_Assets.Camera_Get_Pivots.Examples;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipWGFSX1RhNlhHeGc%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipWGFSX1RhNlhHeGc
see:
https://drive.google.com/file/d/0Byzet-SVq6ipVVEzQWNLUnRTbHM/view
*/

/// <summary>
/// Gère la caméra, plusieurs facteurs influent:
/// - les joueurs
/// - les objets à focus
/// La caméra a pour but de se placer à la position m_DesiredPosition en smooth(lissé).
/// Cette position correspond au barycentre de la liste d'objet Targets.
/// </summary>

public class CameraController : MonoBehaviour
{
    [Range(0, 5f)]    public float m_DampTime = 0.2f;           //Temps d'attente de la caméra pour changer de position
    [Range(0, 15f)]    public float minZoom = 4.0f;             //Zoom minimum
    [Range(5, 150f)]    public float maxZoom = 15.0f;           //Zoom maximum - étape 1
    [Range(5, 150f)]    public float maxZoomMax = 15.0f;        //Zoom maximum au-delà du maximum de base (si le zoom a besoin d'être agrandi)
    [Range(5, 150f)]    public float maxOfMaxMax = 30.0f;       //Zoom Max que le maxZoomMax peut aller
    public float basicZoom = 6.0f;                              //Zoom de base quand il n'y a qu'une seule Target
    [Range(0, 50f)]    public float EdgeMarge = 4f;             //Espace entre les cibles et les bords de la map
    [Range(0, 10f)]    public float reduceWhenTooBig = 0f;      //Zoom quand celui-ci est trop grand
    [Range(0, 10f)]    public float addWhenTooSmall = 0f;       //Dezoom quand celui-ci est trop petit (lorsque l'une des targets touche le mur !)
    [Range(0, 50f)]    public float dezoomCoef = 4;             //Dezoom quand une cible est hors de la map (la caméra a besoin de dézoomer !)
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;           //Optimisation des fps
    [Range(0, 100f)]    public float margeErrorAddCam = 1f;   //Marge de distance avant d'ajouter à la caméra

    [Space(10)]
    public Transform alternativeFocus;                          //Target alternative à focus s'il n'y en a aucune
    public Transform secondAlternativeFocus;                          //Target alternative à focus s'il n'y en a aucune

    [Space(10)]
    [Header("Debug")]
    public List<GameObject> Targets = new List<GameObject>();           //Toutes les Targets, joueur inclus
    public List<GameObject> TargetsPlayers = new List<GameObject>();    //Toutes les Targets de type joueur
    private bool isWallsTouchingPlayer = false;                         //Variable qui définit si l'une des cibles est contre un mur
    public GameObject walls;                                            //Référence aux murs de la caméra
    public bool showWalls = false;                                      //Variable qui définit si l'on doit afficher ou non les murs
    public bool isOnAlternativeFocus = false;                           //défini si la caméra est arrivé sur son focus alternatif
    public bool isOnFocusQueen = false;                                 //le jeu est fini, la caméra se trouve sur la reine (en multi)
    public TimeWithNoEffect TWNE;                                  //temps au début du jeu qui focus sur la queen
   


    /// <summary>
    /// 
    /// </summary>

    private Vector3 m_MoveVelocity;                 //Vitesse de référence, qu'est-ce qu'elle fout là ???
    public  Vector3 m_DesiredPosition;              //La position où la caméra est en train de se déplacer.
    private float timeToGo;                         //Variable d'optimisation
    private float timeZoom;                         //Variable d'optimisation
    public float timeZoomOpti = 2f;                         //Variable d'optimisation

    private GameObject gameController;              //Référence du GameController
    private bool stopMoving = false;                //arrete de bouger la camera !
    private float saveDistanceWhenAdding = -1;      //sauvegarde la distance courante juste avant d'ajouter à la caméra

    /// <summary>
    /// Initialise le gameController
    /// </summary>
    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
    }

    /// <summary>
    /// Initialise l'optimisation et le maxZoomMax au zoom
    /// </summary>
    void Start()
    {
        maxZoomMax = maxZoom;
        gameController.GetComponent<DistanceForWalls>().distance = maxZoom;
        timeToGo = Time.fixedTime + timeOpti;
        timeZoom = Time.fixedTime - 1;
        saveDistanceWhenAdding = -1;
    }

    /// <summary>
    /// Ajoute un joueur à la liste TargetsPlayers
    /// </summary>
    /// <param name="player">joueur à rajouter</param>
    public void AddPlayer(GameObject player)
    {
        if (TargetsPlayers.IndexOf(player) < 0)                 //Si l'objet n'est pas déjà dans l'array !
            TargetsPlayers.Add(player);                         //Ajoute player
    }


    /// <summary>
    /// Ajoute un objet la liste Targets pour la caméra
    /// </summary>
    public void addToCam(GameObject other)
    {
        if (Targets.IndexOf(other) < 0)                         //Si l'objet n'est pas déjà dans l'array !
        {
            stopMoving = false;
            Targets.Add(other);                                 //Ajoute un objet
        }
            
    }

    /// <summary>
    /// Supprime un objet s'il se trouve dans la liste
    /// </summary>
    /// <param name="other">objet à supprimer de la liste</param>
    public void deleteToCam(GameObject other)
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i].GetInstanceID() == other.GetInstanceID())
            {
                Targets.RemoveAt(i);
                return;
            }                
        }
    }

    /// <summary>
    /// Garde le maxZoomMax toujours supérieur ou égal au zoom
    /// </summary>
    void Zoom()
    {
        if (maxZoomMax < maxZoom)
        {
            maxZoomMax = maxZoom;
            gameController.GetComponent<DistanceForWalls>().distance = maxZoom;
        }

        /*if (maxZoomMax > maxOfMaxMax)
        {
            maxZoomMax = maxOfMaxMax;
            gameController.GetComponent<DistanceForWalls>().distance += maxOfMaxMax;
        }*/
            
    }

    /// <summary>
    /// Actualise les listes de la caméra en supprimant les cases vides
    /// (Si un objet a été détruit, la case devient vide)
    /// </summary>
    private void ActualizeTarget()
    {
        isWallsTouchingPlayer = false;                              //Réinitialise
        for (int i = 0; i < Targets.Count; i++)
        {
            //Teste si un objet (Eggs ou autre) est proche d'un mur
            if (Targets[i] && Targets[i].gameObject.activeSelf && Targets[i].GetComponent<BoolCloseToWalls>() && Targets[i].GetComponent<BoolCloseToWalls>().isCloseToWalls)
            {
                isWallsTouchingPlayer = true;           //c'est la cas
                if (Targets[i].tag == "Eggs")           //Attention si c'est un métal, appliquer le changement UNIQUEMENT si celui-ci est contrôlé
                {
                    if (!Targets[i].GetComponent<EggsController>().isControlled)
                        isWallsTouchingPlayer = false;  //en fait non
                }
            }
            //Si la target est désactivée ou inexistante, supprimer la case du tableau
            if (!Targets[i] || !Targets[i].gameObject.activeSelf)
                Targets.Remove(Targets[i]);
        }
        //Réitère le procédé avec les joueurs
        for (int j = 0; j < TargetsPlayers.Count; j++)
        {
            //Teste si le joueur est proche d'un mur
            if (TargetsPlayers[j] && TargetsPlayers[j].gameObject.activeSelf && TargetsPlayers[j].GetComponent<BoolCloseToWalls>() && TargetsPlayers[j].GetComponent<BoolCloseToWalls>().isCloseToWalls)
                isWallsTouchingPlayer = true;
            //Si un joueur est mort ou inactif, le supprimer
            if (!TargetsPlayers[j] || !TargetsPlayers[j].gameObject.activeSelf)
                TargetsPlayers.Remove(TargetsPlayers[j]);
        }
    }

    /// <summary>
    /// Active ou désactive les murs (les affiche ou les cache aussi)
    /// </summary>
    /// <param name="active">vrai ou faux</param>
    private void activeWallsCollider(bool active)
    {
        bool tmpShowWalls = (showWalls) ? active : false;
        //active collider or not
        walls.transform.GetChild(0).GetComponent<BoxCollider>().enabled = active;
        walls.transform.GetChild(1).GetComponent<BoxCollider>().enabled = active;
        walls.transform.GetChild(2).GetComponent<BoxCollider>().enabled = active;
        walls.transform.GetChild(3).GetComponent<BoxCollider>().enabled = active;
        walls.transform.GetChild(4).GetComponent<BoxCollider>().enabled = active;
        //show walls or not
        walls.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = tmpShowWalls;
        walls.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = tmpShowWalls;
        walls.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = tmpShowWalls;
        walls.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = tmpShowWalls;
        walls.transform.GetChild(4).GetComponent<MeshRenderer>().enabled = tmpShowWalls;
    }

    /// <summary>
    /// clear la liste des targets
    /// </summary>
    public void clearTarget()
    {
        Debug.Log("ici clean target");
        Targets.Clear();
        TargetsPlayers.Clear();
    }

    /// <summary>
    /// Définit la position où la caméra doit se rendre
    /// </summary>
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();                            //Position finale
        int numTargets = 0;                                            //Compte le nombre de targets de la caméra
        float minX = 0;                                                //Définit le min/max en X et Y
        float maxX = 0;
        float minY = 0;
        float maxY = 0;
        bool neverOut = true;                                          //Sert à savoir si aucune target n'est hors de l'écran
        float biggestDist = 0.0f;

        //Boucle sur chaque target
        for (int i = 0; i < Targets.Count; i++)
        {
            GameObject tmpTarget = Targets[i];
            if (!tmpTarget || !tmpTarget.activeSelf)       //Si elle est inactive, passer à la suivante
                continue;

            //si c'est un player, prendre son pointSmooth !
            //if (tmpTarget.CompareTag("Player"))
              //  tmpTarget = tmpTarget.GetComponent<PlayerController>().pointCamSmooth.gameObject;

            if (tmpTarget.transform.position == Vector3.zero)
            {
                Debug.Log("oko totototo");
            }
            if (i == 0)                                                 //Si c'est le début de boucle,
            {                                                           //Définit min/max à la première target
                minX = maxX = tmpTarget.transform.position.x;          //  en x
                minY = maxY = tmpTarget.transform.position.y;          //  en y
            }
            else                                                        //Sinon, agrandit/diminue selon la target courante pour définir un carré qui englobe toutes les targets
            {
                minX = (tmpTarget.transform.position.x < minX) ? tmpTarget.transform.position.x : minX;
                maxX = (tmpTarget.transform.position.x > maxX) ? tmpTarget.transform.position.x : maxX;
                minY = (tmpTarget.transform.position.y < minY) ? tmpTarget.transform.position.y : minY;
                maxY = (tmpTarget.transform.position.y > maxY) ? tmpTarget.transform.position.y : maxY;
            }

            
            //Teste si la target n'est pas dans l'écran
            if (tmpTarget.GetComponent<IsOnScreen>() && !tmpTarget.GetComponent<IsOnScreen>().isOnScreen)
            {
                //Debug.Log("agrandir...");                               //Agrandit le zoom !
                neverOut = false;                                       //Définit qu'on est sorti... pour la suite
                activeWallsCollider(false);                             //Désactive les murs (on les réactivera plus tard)

                if (tmpTarget.GetComponent<BoolCloseToWalls>())
                    tmpTarget.GetComponent<BoolCloseToWalls>().isCloseToWalls = false;
                else
                    Debug.Log("error");


                //Définit biggestDist la distance à rajouter au zoom
                Vector2 screenPoint2d = Camera.main.WorldToViewportPoint(tmpTarget.transform.position);
                Vector2 center = new Vector2(0.5f, 0.5f);
                float distTmp = Mathf.Abs(Vector2.SqrMagnitude(screenPoint2d - center) - 0.25f) * dezoomCoef;
                biggestDist = (distTmp > biggestDist) ? distTmp : biggestDist;
                biggestDist = addWhenTooSmall / 2;
                
            }


            numTargets++;                                               //Ajoute le compteur pour connaître le nombre de targets actives
        }

        if (neverOut)                                                   //Si toutes les targets sont dedans, referme les murs !
            activeWallsCollider(true);

        //S'il y a plus d'une target, définit le milieu par rapport aux min/max !
        if (numTargets > 0)
        {
            averagePos.x = (minX + maxX) / 2;
            averagePos.y = (minY + maxY) / 2;
        }
        //S'il n'y a aucune target... sélectionne le focus alternatif ! (la reine)
        if (Targets.Count == 0)
        {
            if (alternativeFocus)
                averagePos = alternativeFocus.position;
            else if (secondAlternativeFocus)
                averagePos = secondAlternativeFocus.position;
            else
            {
                stopMoving = true;
                return;
            }
                
        }
        else
        {
            stopMoving = false;
        }
            
        if (!TWNE.isOk)
        {
            if (secondAlternativeFocus)
                averagePos = secondAlternativeFocus.position;
        }


        //Ajoute au zoom biggestDist
        //(Vous vous souvenez, c'est juste en haut, si une target s'est retrouvée hors de l'écran)
        gameController.GetComponent<DistanceForWalls>().distance += biggestDist;
        maxZoomMax += biggestDist;

        //Définit la distance à mesurer
        float dist = Mathf.Max(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY));
        averagePos.z = (Targets.Count > 1) ? -Mathf.Min(Mathf.Max(minZoom, dist + EdgeMarge), Mathf.Max(maxZoom, maxZoomMax)) : -basicZoom;

        littleChangeCamera(averagePos, neverOut, dist);

         //Finalement, change la cible pour la caméra
        m_DesiredPosition = averagePos;
    }

    /// <summary>
    /// ajuste la caméra:
    /// - zoom de la camere progressivement SI on a ajouté un extrat
    /// - réajuster le maxZoom
    /// - dezzom un extra si un élément touche les murs de la caméra
    /// </summary>
    void littleChangeCamera(Vector3 averagePos, bool neverOut, float dist)
    {
        if (Time.fixedTime >= timeZoom)
        {
            
            //si distance actuelle < distance XXX, ok diminue
            //Ici réduit la caméra
            if (neverOut && Mathf.Abs(averagePos.z) > Mathf.Abs(maxZoom) && averagePos.z < maxZoomMax + (reduceWhenTooBig * 2) && !isWallsTouchingPlayer
                && dist < saveDistanceWhenAdding - margeErrorAddCam)
            {
                //Debug.Log("distance 2: " + dist);

                maxZoomMax -= reduceWhenTooBig;
                gameController.GetComponent<DistanceForWalls>().distance -= reduceWhenTooBig;
            }
            //timeZoom = Time.fixedTime + timeZoomOpti;
        }
        //Ici réajuste le maxZoom
        if (Mathf.Abs(averagePos.z) < Mathf.Abs(maxZoom))
        {
            //Debug.Log("ici on réajute");
            maxZoomMax = maxZoom;
            gameController.GetComponent<DistanceForWalls>().distance = maxZoom;
        }
        //Ajoute à la caméra si l'une des targets touche les murs OU si une des target est en dehors !
        if (isWallsTouchingPlayer/* || !neverOut*/)
        {
            Debug.Log("ici ??");
            maxZoomMax += addWhenTooSmall;
            gameController.GetComponent<DistanceForWalls>().distance += addWhenTooSmall;
            timeZoom = Time.fixedTime + timeZoomOpti;
            saveDistanceWhenAdding = dist;
            //Debug.Log("distance: " + dist);
            //save distance = XXX;
        }
    }

    /// <summary>
    /// N'est jamais appelé pour l'instant
    /// Initialise la position de la caméra
    /// </summary>
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();                                      // Trouve le point cible pour la caméra
        transform.position = m_DesiredPosition;
    }

    /// <summary>
    /// renvoi true si la caméra se trouve sur sa position visé
    /// </summary>
    /// <returns></returns>
    bool camIsOnFocus(Transform desiredPos)
    {
        if (transform.position.x > desiredPos.position.x - 1
                && transform.position.x < desiredPos.position.x + 1
                && transform.position.y > desiredPos.position.y - 1
                && transform.position.y < desiredPos.position.y + 1)
        {
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// test si la caméra est sur sa cible
    /// si oui et que levelFailedSolo est true, alors cela implique la fin du niveau
    /// </summary>
    void testIfCamIsOnDesiredPos()
    {
        //pour le solo si on a fail
        if (gameController.GetComponent<GameManager>().TEL.levelFailedSolo && camIsOnFocus(alternativeFocus))
        {
            isOnAlternativeFocus = true;
            stopMoving = true;
        }
        //pour solo et multi
        //la reine est full (en multi ?), set le focus de la queen a vrai
        if (gameController.GetComponent<GameManager>().queenIsFull && alternativeFocus && camIsOnFocus(alternativeFocus))
        {
            isOnFocusQueen = true;
            stopMoving = true;
        }
    }

    private void Update()
    {
        if (Time.fixedTime >= timeToGo && !stopMoving)
        {
            Zoom();
            ActualizeTarget();
            FindAveragePosition();
            testIfCamIsOnDesiredPos();
            timeToGo = Time.fixedTime + timeOpti;
        } 
    }

    /// <summary>
    /// Bouge la caméra lentement (smoothy) vers la position souhaitée
    /// </summary>
    private void LateUpdate()
    {
        if (stopMoving)
            return;
        //bouger vers la position souhaité
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }
}