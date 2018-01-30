using UnityEngine;
using System.Collections;

public class ColoredProgressBar : ProgressBar
{
    public string descriptionScript = "mode ...";
    public Color finalColor;
    public Color startColor;
    public int addToFractionalPorgress = 0;                                        //en mode solo, lorsque le timer arrive de 0 à max, revient à 0, mais sauvegarde l'ajout
    public int scoreTeam = -1;                                              //précise si le script est attaché au chrono ou pas
    /// <summary>
    /// variable privée
    /// </summary>
    
    private GameObject gameManager;
    private bool multi = false;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    private void Start()
    {
        multi = gameManager.GetComponent<GameManager>().multi;
        if (!multi)
            descriptionScript = "mode solo...";
        else
            descriptionScript = "mode multi...";
    }

    public override ProgressBar setColor(Color c)
    {
        finalColor = c;
        return this;
    }

    protected override void calculateProgress()
    {
        if (isMinMax()) return;
        if (smoothly) {
            float fillNeed = currentProgress / (float)max;
            float currentFill = Mathf.Lerp(progress.fillAmount, fillNeed, smoothTime*Time.deltaTime);
            progress.fillAmount = currentFill;
            facticalProgress = Mathf.RoundToInt(currentFill * (float)max);
        } else {
            facticalProgress = currentProgress;
            if (!progress)
                return;
            progress.fillAmount = facticalProgress / (float)max;
        }
        progress.color = new Color(1 - facticalProgress / (float)max + (finalColor.r * (facticalProgress / (float)max)), 1 - facticalProgress / (float)max + (finalColor.g * (facticalProgress / (float)max)), 1 - facticalProgress / (float)max + (finalColor.b * (facticalProgress / (float)max)));
        float progressForColor = facticalProgress / (float)max;
        progress.color = startColor * (1 - progressForColor) + finalColor * progressForColor;

        int tmpToAdd = addToFractionalPorgress + facticalProgress;
        float minutes = Mathf.Floor(tmpToAdd / 60);
        float seconds = (tmpToAdd % 60);

        if (scoreTeam == -1)                                                                       //si on est dans le chrono, mettre le texte en seconde/minutes
        {
            text.text = addictionalText + minutes + ":" + ((seconds < 10) ? "0" : "") + seconds + afterText;
            if (!multi)                                                                                 //on est dans le mode solo...
            {
                if (getProgress() > max - (float)warningTime && getProgress() < max)                    //si on est inférieur à Max, et supérieur à max - warningTime, ping Pong rouge/blanc    
                    text.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 2, 1));
                else if (getProgress() >= max)                                                          //si on est à max, remetre à 0, à blanc, et ajoute ) addToFract max;
                {
                    gameManager.GetComponent<GameManager>().createEclosionEvent();
                    addToFractionalPorgress += max;                                                     //ajoute max pour sauvegarder le temps d'avant avant de remettre à 0.
                    text.color = Color.white;                                                           //change la couleur du texte en blanc
                    setProgress(0, true);                                                               //change le progret courant à 0;
                }
                else                                                                                    //sinon, on est en dessous de max - warningTime                                                           
                    text.color = Color.white;                                                           //change la couleur du texte en blanc;
            }
            else                                                                                        //on est dans le mode multi...
            {
                if (getProgress() <= (float)warningTime && getProgress() > 0)                           //on est dans le warning time, ping pong entre blanc et rouge le texte
                    text.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 2, 1));
                if (facticalProgress <= 0)                                                            //on est à 0: met en rouge le texte
                {
                    text.color = Color.red;
                    gameManager.GetComponent<GameManager>().timerEnd = true;                            //le timer est terminé, on a fini !
                }

                else                                                                                    //on est supérieur à warning time, met en blanc le texte
                    text.color = Color.white;
            }
        }
        else if (scoreTeam == 1)                                                                                            //sinon, on est dans les scores...
        {
            text.text = addictionalText + (max - facticalProgress).ToString();
            //text.color = Color.white;
        }
        else
        {
            //rien ?
        }
    }

}
