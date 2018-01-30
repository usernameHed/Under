using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalLinks : MonoBehaviour
{
    public FirePortals refPlayer;                                   //la référence officiel du joueur
    public GameObject Range;                                        //la range... TODO ENLEVER ???
    public GameObject Attract;                                      //l'objet attract (pour définir qui il attire)
    public PortalController PC;                                     //l'objet a-t-il un portal Controller ?

    [SerializeField] private ParticleSystem[] PSportals;                     //particle system des portals
    [SerializeField] private bool attract = false;                  //est-ce l'attract ou le repulse ?
    [SerializeField] private bool repulse = false;                  //est-ce l'attract ou le repulse ?
    [SerializeField] private Material[] matPortals;                         //matérials des portal blue/red

    /// <summary>
    /// initialise les references du projectile au player
    /// </summary>
    public void initPlayerParticle(FirePortals FPplayer, int nbTeam, int typeColor)
    {
        refPlayer = FPplayer;
        if (Attract)
            Attract.GetComponent<TrapsController>().idTeam = nbTeam;

        colorPortalInit(typeColor);
    }

    /// <summary>
    /// ici est appelé lorsque le portal attract n'emmet plus...
    /// </summary>
    public void stopAttract()
    {
        refPlayer.stopAttract();
    }

    /// <summary>
    /// ici color les portal Init/aspire/repulse en bleu ou rouge si on est dans le mode multi !
    /// </summary>
    void colorPortalInit(int typeColor)
    {
        //0: yellow, 1: orange, 2: blue, 3: red
        
        //ici le joueur coop
        if (typeColor == 1)
        {
            if (attract)
            {
                Debug.Log("ici attract ?");

                //parent
                PSportals[0].Stop();
                var parentParticle = PSportals[0].main;
                parentParticle.startColor = Color.red;
                PSportals[0].Play();

                //Glow
                PSportals[1].Stop();
                var glowParticle = PSportals[1].main;

                glowParticle.startColor = new Color(255 / 255.0f, 4 / 255.0f, 4 / 255.0f, 255 / 255.0f);
                PSportals[1].Play();

                //Funnel
                PSportals[2].Stop();
                var funnelParticle = PSportals[2].main;

                funnelParticle.startColor = new Color(255 / 255.0f, 26 / 255.0f, 0 / 255.0f, 255 / 255.0f);
                PSportals[2].Play();
            }
            else if (repulse)
            {
                Debug.Log("ici repulse ?");
                //parent
                PSportals[0].Stop();
                var parentParticle = PSportals[0].main;
                parentParticle.startColor = new Color(255 / 255.0f, 6 / 255.0f, 6 / 255.0f, 159 / 255.0f);
                PSportals[0].Play();

                //Glow
                PSportals[1].Stop();
                var glowParticle = PSportals[1].main;

                glowParticle.startColor = Color.red;
                PSportals[1].Play();

                //Funnel
                PSportals[2].Stop();
                var funnelParticle = PSportals[2].main;

                funnelParticle.startColor = new Color(255 / 255.0f, 90 / 255.0f, 0 / 255.0f, 251 / 255.0f);
                PSportals[2].Play();
            }
        }
    }
}
