using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitOnClick : MonoBehaviour
{
    AsyncOperation async;
    private bool isCharging = false;             //détermine si une scène est en chargement

    /// <summary>
    /// jump à une scène
    /// </summary>
    /// <param name="scene"></param>
    public void jumpToScene(string scene = "1_MainMenu")
    {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// ajoute une scène à celle courrante
    /// </summary>
    /// <param name="scene"></param>
    public void jumpAdditiveScene(string scene = "1_MainMenu")
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// charge une scène en mode async
    /// </summary>
    public void StartLoading(string scene = "1_MainMenu", bool swapWhenFinish = true)
    {
        StartCoroutine(load(scene, swapWhenFinish));
    }

    IEnumerator load(string scene, bool swapWhenFinish)
    {
        Debug.LogWarning("ASYNC LOAD STARTED - " +
           "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
        async = Application.LoadLevelAsync(scene);
        async.allowSceneActivation = swapWhenFinish;
        isCharging = true;
        yield return async;
        if (swapWhenFinish)
            ActivateScene();
    }

    [ContextMenu("ActivateScene")]
    public void ActivateScene()
    {
        if (!isCharging)
            return;
        isCharging = false;
        async.allowSceneActivation = true;
    }

    public void jumpToSceneWithFade(string scene = "1_MainMenu")
    {
        if (!gameObject.GetComponent<Fading>().enabled)
        {
            gameObject.GetComponent<Fading>().setupWorld(PlayerPrefs.PP.lastLevelPlayerId[0]);
            gameObject.GetComponent<Fading>().enabled = true;
        }
        StartCoroutine(jumpToSceneWithFadeWait(scene));
    }

    IEnumerator jumpToSceneWithFadeWait(string scene = "1_MainMenu")
    {
        float fadeTime = gameObject.GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        jumpToScene(scene);
    }

    /// <summary>
    /// quite le jeu (si on est dans l'éditeur, quite le mode play)
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
