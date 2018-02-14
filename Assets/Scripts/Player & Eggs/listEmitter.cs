using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class listEmitter : MonoBehaviour
{
    public List<FmodEventEmitter> listEmmiterScript = new List<FmodEventEmitter>();
    
    public FmodEventEmitter getInList(int index)
    {
        if (listEmmiterScript.Count > index)
        {
            return (listEmmiterScript[index]);
        }
        else
            Debug.LogError("list empty ??");

        return (null);
    }

    /// <summary>
    /// est appelé lors du awake, pour ajouter l'id du joueurs aux FmodEventEmmiter !
    /// </summary>
    public void setupAdditiveName(string add)
    {
        for (int i = 0; i < listEmmiterScript.Count; i++)
        {
            listEmmiterScript[i].additionnalName = add;
        }
    }

    /// <summary>
    /// stop tout les emitter
    /// </summary>
    public void stopAllEmitter()
    {
        for (int i = 0; i < listEmmiterScript.Count; i++)
        {
            SoundManager.GetSingleton.playSound(listEmmiterScript[i], true);
        }
    }
}
