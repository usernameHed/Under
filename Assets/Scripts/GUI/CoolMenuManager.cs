using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired.UI.ControlMapper;

public class CoolMenuManager : MonoBehaviour
{
    public List<Transform> listWorld = new List<Transform>();
    public List<Transform> listRealWorld = new List<Transform>();
    public List<CustomButton> listButtonWorld = new List<CustomButton>();
    public List<Tween> listTween = new List<Tween>();       //list des itween paths
    public List<CustomButton> listButtonMutli = new List<CustomButton>();

    /*private int stepMainMenu = 0;
    public int StepMainMenu
    {
        get
        {
            return (stepMainMenu);
        }
        set
        {
            if (stepMainMenu != value)
            {
                
                stepMainMenu = value;
                dotMove(listWorld[stepMainMenu], 0);
            }
        }
    }*/

    public Transform cam;
    private LevelManagerMainMenu LMM;
    private bool goToCampainForSecondTime = false;

    /// <summary>
    /// init
    /// </summary>
    private void Awake()
    {
        //cam.position = listWorld[0].position;    //set la caméra au début à la bonne positio !
        LMM = gameObject.GetComponent<LevelManagerMainMenu>();
    }

    // Use this for initialization
    void Start ()
    {
        listTween = DOTween.TweensById("menuToWorld");
        listTween[0].OnWaypointChange(whenIsOnWeapoint);
    }

    /// <summary>
    /// effectue une action à chauqe changement de waypoints
    /// </summary>
    private void whenIsOnWeapoint(int waypointIndex)
    {
        Debug.Log("index:" + waypointIndex);
        if (waypointIndex == 2 && LMM.SuperLocation == 1)    //si on a sélectionné le mode CAMPAIN et qu'on a fini de dezzom
        {
            Debug.Log("ici campain choice map");
            LMM.SuperLocation = 3;
        }
        //if (stepMainMenu == 1 && waypointIndex)
    }

    /// <summary>
    /// va vers la campagne
    /// </summary>
    public void playPathToCampain()
    {
        if (goToCampainForSecondTime)
            dotMove(listWorld[2], 3);
        else
            listTween[0].Play();
        goToCampainForSecondTime = true;
    }

    /// <summary>
    /// on est en mode solo et on clique sur le centre de clonage !
    /// </summary>
    public void moveClonage()
    {
        Debug.Log("ici pour zoomer sur le centre de clonage ???");
        dotMove(listWorld[3], 2);
    }

    /// <summary>
    /// ici on a cliqué sur un monde, zoomer dessus !
    /// </summary>
    public void moveWorld(int world)
    {
       dotMove(listWorld[3 + world], 1);
    }

    /// <summary>
    /// do a move of camera
    /// </summary>
    /// <param name="focus"></param>
    public void dotMove(Transform focus, int step)
    {
        switch (step)
        {
            case 0:
                cam.DOMove(focus.position, 4)
                  .SetEase(Ease.InOutQuint)
                  .OnComplete(mainChange);
                break;
            case 1:
                //on focus sur un monde en particulier !! / / sur "Worlds" (listWorld) / "Menu"
                cam.DOMove(focus.position, 2)
                  .SetEase(Ease.OutExpo)
                  .OnComplete(changeInWorld);
                break;
            case 2:
                //ici on focus le centre de clonage 
                cam.DOMove(focus.position, 1)
                  .SetEase(Ease.InOutQuint)
                  .OnComplete(mainChange);
                break;
            case 3:
                cam.DOMove(focus.position, 1)
                  .SetEase(Ease.InOutQuint)
                  .OnComplete(mainChange);
                break;
            case 4:
                //ici on focus un level random
                cam.DOMove(focus.position, 2)
                  .SetEase(Ease.InOutQuint)
                  .OnComplete(mainChange);
                break;
        }
    }
	
    void mainChange()
    {
        if (LMM.SuperLocation == 2)
        {
            Debug.Log("ici on est en mode selection multi");
            LMM.SuperLocation = 8;
        }
        else if (LMM.SuperLocation == 1)    //ici on va dans le mode campain pour la deuxieme fois
        {
            //Debug.Log("ici campain choice map 2");
            LMM.SuperLocation = 3;
        }
    }

    /// <summary>
    /// ici on viens d'arriver sur un monde
    /// </summary>
    void changeInWorld()
    {
        if (LMM.SuperLocation == 5)
        {
            //Debug.Log("ici on est revenu dans la selection des mondes");
            listButtonWorld[LMM.campainLocation].interactable = true;
            
            listButtonWorld[LMM.campainLocation].Select();
            //Debug.Log("ici campain choice map");
            LMM.SuperLocation = 6;
        }
        else if (LMM.SuperLocation == 7 || LMM.SuperLocation == 9)
        {
            LMM.SuperLocation = 10;
        }
        else
        {
            Debug.Log("ici on est focus sur le monde: " + LMM.campainLocation);
            LMM.SetTuto(false);
            /*listRealWorld[0].gameObject.SetActive(false);
            listRealWorld[LMM.campainLocation].GetChild(0).gameObject.GetComponent<FadeObjectInOut>().fadeOut = true;
            listRealWorld[LMM.campainLocation].GetChild(1).gameObject.SetActive(true);*/
            LMM.SuperLocation = 4;
        }
    }

    /// <summary>
    /// on a dezoomé et on reviens au "world"
    /// </summary>
    public void backToCampainLocation()
    {
        //listRealWorld[0].gameObject.SetActive(true);
        //listRealWorld[LMM.campainLocation].GetChild(0).gameObject.GetComponent<FadeObjectInOut>().fadeOut = false;
        listRealWorld[LMM.campainLocation].GetChild(1).GetChild(0).gameObject.GetComponent<FadeObjectInOut>().fadeOut = true;
        listButtonWorld[LMM.campainLocation].gameObject.GetComponent<FadeObjectInOut>().fadeOut = false;
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
