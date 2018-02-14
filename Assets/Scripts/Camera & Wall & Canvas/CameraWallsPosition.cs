using Assets.Cyborg_Assets.Camera_Get_Pivots.Examples;  //Created by Cyborg Assets
using UnityEngine;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipc3ViUmRIUzNkTzg%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipc3ViUmRIUzNkTzg
see:
https://drive.google.com/file/d/0Byzet-SVq6ipdWtPOThUS05SR0E/view
*/
/// <summary>
/// Permet de positionner les 4 mur autour de la caméra pour bloquer
/// le joueur d'aller hors caméra. Les joueur/eggs se trouvant proche des
/// mur augmente la caméra
/// + place un 5ème trigger en haut au centre pour déterminer
/// si le joueur / les métaux se trouvent au dessus des éléments canva d'affichage
/// (pour qu'ensuite le sccript CollideWithWalls se charge de tester si les objets le touche)
/// </summary>

namespace Assets.Camera_Extend_In_Direction.Examples
{
	public class CameraWallsPosition : MonoBehaviour
	{
        [Header("main")]

        [Space(10)]
        [Header("debug")]
        public Transform Walls;                                         //Walls parent contenant les 4 fils
        [Range(0, 10f)]    public float timeOpti = 0.1f;                //optimisation FPS
        [Range(0, 10f)]    public float  edgeAdjust = 0f;                //optimisation FPS

        /// <summary>
        /// variable privé
        /// </summary>

        private float timeToGo;                                         //optimisation FPS
        private Camera cc;                                              //référence de la camera

        /// <summary>
        /// Initialisation en début de jeu
        /// </summary>
        private void Awake()
        {
            cc = Camera.main;                                           //initialise camera
        }

        /// <summary>
        /// Positionne les enfants de Walls autour de la caméra pour créé des murs
        /// Remet ensuite la position en Z à 0 pour que les mur reste au niveau du sol !!
        /// </summary>
        private void setWall5ToCanvas()
        {
            if (!Walls)
                return;
            Vector3 topCenter = cc.GetTopPosition(DistanceForWalls.instance.distance);                  // top center:      UP
            Walls.GetChild(4).transform.position = new Vector3(topCenter.x, topCenter.y - 1.0f, 0.0f);
        }

        /// <summary>
        /// - Positionne les enfants de walls autour de la caméra pour créé des murs, en mettant Z à 0
        /// </summary>
        void setEdge()
        {
            if (!Walls)
                return;
            Vector3 topCenter = cc.GetTopPosition(DistanceForWalls.instance.distance);                  // top center:      UP
            Walls.GetChild(0).transform.position = new Vector3(topCenter.x, topCenter.y - edgeAdjust, 0.0f);
  
            Vector3 bottomCenter = cc.GetBottomPosition(DistanceForWalls.instance.distance);            // bottom center:   DOWN
            Walls.GetChild(1).transform.position = new Vector3(bottomCenter.x, bottomCenter.y + edgeAdjust, 0.0f);

            Vector3 leftCenter = cc.GetLeftPosition(DistanceForWalls.instance.distance);                // Left center:     LEFT
            Walls.GetChild(2).transform.position = new Vector3(leftCenter.x, leftCenter.y, 0.0f);

            Vector3 rightCenter = cc.GetRightPosition(DistanceForWalls.instance.distance);              // Right center:    RIGHT
            Walls.GetChild(3).transform.position = new Vector3(rightCenter.x, rightCenter.y, 0.0f);
        }

        void Update()
        {
            if (Time.fixedTime >= timeToGo)
            {
                setEdge();
                setWall5ToCanvas();
                timeToGo = Time.fixedTime + timeOpti;
            }
		}
	}
}
