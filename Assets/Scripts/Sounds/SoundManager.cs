using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using FMOD.Studio;

/// <summary>
/// description
/// </summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class SoundManager : MonoBehaviour                                   //commentaire
{
    [FMODUnity.EventRef]

    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public static SoundManager SS;

    //ici lorsque la musique est changé, elle se joue, puis la variable revient instantanément à NONE
    //ajoute autant de nom dans cette liste
    public List<string> musicList;
    public List<string> sfxList;
    public List<string> sfxGameList;

    public FmodEventEmitter musicEmitterScript;

    private int musicState = 0;
    public int MusicState
    {
        get
        {
            return (musicState);
        }
        set
        {
            if (musicState != value)
            {
                musicState = value;
                stateMusicChanged();
            }
        }
    }

    private bool musicWin = false;
    public bool MusicWin
    {
        get
        {
            return (musicWin);
        }
        set
        {
            if (musicWin != value)
            {
                musicWin = value;
                winMusicChanged();
            }
        }
    }

    //[OnValueChanged("value changed ?")]

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>

    /// <summary>
    /// variable privé serealized
    /// </summary>


    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        if (SS == null)
            SS = this;
        else if (SS != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {

    }
    #endregion

    #region core script

    /// <summary>
    /// appelé lorsque la state de la musique a changé
    /// </summary>
    private void stateMusicChanged()
    {
        if (!musicEmitterScript)
            Debug.LogError("nop");
        playSound(musicEmitterScript, "Checkpoint", musicState);
    }

    /// <summary>
    /// est appelé lors d'un changement de state de la musique
    /// </summary>
    private void winMusicChanged()
    {
        if (!musicEmitterScript)
            Debug.LogError("nop");
        playSound(musicEmitterScript, "Win", 100f);
    }

    /// <summary>
    /// joue un son de menu (sans emmiter)
    /// </summary>
    public void playSound(string sound)
    {
        if (sound == null)
            return;

        //vérifie que "sound" existe bien dans l'une des 3 liste: musique, son menu, son in game
        //vérifie que sound n'est pas unne chaine vide, un espace ou commence par un '-'
        if (!((musicList.Contains(sound) || sfxList.Contains(sound) || sfxGameList.Contains(sound)))
            || !sound.Contains("event:/"))
        {
            //////////////////////////ICI mettre le debug
            Debug.Log("le son appelé n'existe dans aucune liste (" + sound + ")");
            return;
        }
        Debug.Log("jouer le son:" + sound);
        FMODUnity.RuntimeManager.PlayOneShot(sound);   //methode 1 
    }

    /// <summary>
    /// ici play l'emitter (ou le stop)
    /// </summary>
    /// <param name="emitterScript"></param>
    public void playSound(FmodEventEmitter emitterScript, bool stop = false)
    {
        if (!stop)
            emitterScript.play();
        else
            emitterScript.stop();
    }

    /// <summary>
    /// ici change le paramettre de l'emitter
    /// </summary>
    /// <param name="emitterScript"></param>
    public void playSound(FmodEventEmitter emitterScript, string paramName, float value)
    {
        emitterScript.setParameterValue(paramName, value);
    }

    /// <summary>
    /// DEBUG
    /// </summary>
	[Button("playSound 'Bouton Methode2'")]    public void testSoundDefault2()    {        FMODUnity.RuntimeManager.PlayOneShot ("event:/SFX/Bouton");    } // Methode 2

    #endregion

    #region unity fonction and ending
    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {

    }
    #endregion
}
