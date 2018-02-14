using UnityEngine;
using UnityEngine.UI;
using Rewired;
using Rewired.UI;
using Rewired.UI.ControlMapper;

public class LevelManagerSettings : MonoBehaviour {

    public GameObject exit;
    public Button[] listButtonEscape;
    private TimeWithNoEffect TWNE;
    private QuitOnClick QOC;
    public int locationQuit = 0;
    public bool exitActive = false;

    private ControlMapper controlMapper;

    private GameObject GlobalVariableManager;
    private PlayerConnected PC;

    private void Awake()
    {
        GlobalVariableManager = GameObject.FindGameObjectWithTag("Global");
        controlMapper = GlobalVariableManager.GetComponent<ControlMapper>();
        PC = GlobalVariableManager.GetComponent<PlayerConnected>();
    }
    
    /// <summary>
    /// initialise les variables
    /// </summary>
    private void Start()
    {
        QOC = gameObject.GetComponent<QuitOnClick>();
        //controlMapper.Open();
        Cursor.visible = true;
    }

    /// <summary>
    /// action quand on appruis sur echap ou B
    /// </summary>
    private void quitAction()
    {
        //action retour du joueur 0 ou 1 (clavier ou joystick 1)
        if (PC.getPlayer(0).GetButtonDown("UICancel") || PC.getPlayer(1).GetButtonDown("UICancel"))
        {
            //controlMapper.GetComponent<ControlMapper>().Close(true);
            QOC.jumpToScene("1_MainMenu");
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 100, 100, 30), "reset all progress"))
        {
            PlayerPrefs.getSingularity().resetAll();
        }
    }

    /// <summary>
    /// update
    /// </summary>
    private void Update()
    {
        quitAction();
    }
}
