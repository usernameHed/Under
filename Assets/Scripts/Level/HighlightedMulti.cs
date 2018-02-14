using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightedMulti : MonoBehaviour
{
    public bool highlight = false;
    public LevelManagerMainMenu LMMM;
    public LevelManagerSolo LMS;
    public LevelManagerMulti LMM;    
    public int menuType = 2;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            highlight = true;
        else
        {
            highlight = false;
             if (LMM.mapLocation == gameObject.transform.GetSiblingIndex())
                 gameObject.GetComponent<Toggle>().Select();
        }
    }
}