using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerConnected : MonoBehaviour
{
    private Player player0;     //keyboard zqsd;
    private Player player1;     //joypad 1;
    private Player player2;     //joypad 2;
    private Player player3;     //joypad 3;
    private Player player4;     //joypad 4;
    private Player player5;     //keyboard arrow
    public List<bool> playerControllerConnected = new List<bool>();
    public LevelManagerMainMenu LMMM;
    public LevelManagerGame LMG;

    private void Awake()
    {
        player0 = ReInput.players.GetPlayer(0);
        player1 = ReInput.players.GetPlayer(1);
        player2 = ReInput.players.GetPlayer(2);
        player3 = ReInput.players.GetPlayer(3);
        player4 = ReInput.players.GetPlayer(4);
        player5 = ReInput.players.GetPlayer(5);
        playerControllerConnected.Add(true);        //le player0 a toujours le clavier assigné (zqsd)
        playerControllerConnected.Add(false);       //le joypad1
        playerControllerConnected.Add(false);       //le joypad2
        playerControllerConnected.Add(false);       //le joypad3
        playerControllerConnected.Add(false);       //le joypad4
        playerControllerConnected.Add(true);        //le player5 a toujours le clavier assigné (arrow)
    }

    /// <summary>
    /// actualise le player ID si il est connecté ou déconnecté
    /// </summary>
    /// <param name="id">id du joueur</param>
    /// <param name="isConnected">statue de connection du joystick</param>
    public void updatePlayerController(int id, bool isConnected)
    {
        playerControllerConnected[id] = isConnected;
        if (LMMM)
            LMMM.joypadActualized();
        if (LMG)
        {
            LMG.handleDisconnect(id, isConnected);
        }
    }

    /// <summary>
    /// get id of player
    /// </summary>
    /// <param name="id"></param>
    public Player getPlayer(int id)
    {
        switch (id)
        {
            case -1:
                return (ReInput.players.GetSystemPlayer());
            case 0:
                return (player0);
            case 1:
                return (player1);
            case 2:
                return (player2);
            case 3:
                return (player3);
            case 4:
                return (player4);
            case 5:
                return (player5);
        }
        return (null);
    }

    /// <summary>
    /// retourne vrai si zqsd est pressé
    /// </summary>
    public bool keyboardZqsd()
    {
        if (getPlayer(0).GetAxis("UIHorizontal") != 0
            || getPlayer(0).GetAxis("UIVertical") != 0)
        {
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// retourne vrai si arrow est pressé
    /// </summary>
    public bool keyboardArrow()
    {
        if (getPlayer(5).GetAxis("UIHorizontal") != 0
            || getPlayer(5).GetAxis("UIVertical") != 0)
        {
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// fonction appelé si ECHAP est pressé, ou start dans n'importe quel manette
    /// </summary>
    /// <returns></returns>
    public bool exitAll(bool onlyJoypad = false)
    {
        if (!onlyJoypad)
        {
            if (getPlayer(-1).GetButtonDown("Start")
                || getPlayer(-1).GetButtonDown("Escape")
                || getPlayer(1).GetButtonDown("Start")
                || getPlayer(2).GetButtonDown("Start")
                || getPlayer(3).GetButtonDown("Start")
                || getPlayer(4).GetButtonDown("Start"))
                return (true);
        }
        else
        {
            if (getPlayer(1).GetButtonDown("Start")
                || getPlayer(2).GetButtonDown("Start")
                || getPlayer(3).GetButtonDown("Start")
                || getPlayer(4).GetButtonDown("Start"))
                return (true);
        }
        return (false);
    }

    /// <summary>
    /// Fonction appelé si l'une des 4 manette appuis sur B (UICancel)
    /// </summary>
    /// <returns></returns>
    public bool BAll()
    {
        if (getPlayer(1).GetButtonDown("UICancel")
            || getPlayer(2).GetButtonDown("UICancel")
            || getPlayer(3).GetButtonDown("UICancel")
            || getPlayer(4).GetButtonDown("UICancel"))
            return (true);
        return (false);
    }

    /// <summary>
    /// fonction appelé si droite est appelé par: clavier zqsd/arrow, ou l'une des 4 mannettes
    /// </summary>
    /// <returns></returns>
    public bool rightAll()
    {
        if (getPlayer(0).GetAxis("UIHorizontal") > 0
           || getPlayer(1).GetAxis("UIHorizontal") > 0.5f
           || getPlayer(2).GetAxis("UIHorizontal") > 0.5f
           || getPlayer(3).GetAxis("UIHorizontal") > 0.5f
           || getPlayer(4).GetAxis("UIHorizontal") > 0.5f
           || getPlayer(5).GetAxis("UIHorizontal") > 0)
            return (true);
        return (false);
    }

    /// <summary>
    /// fonction appelé si left est appelé par: clavier zqsd/arrow, ou l'une des 4 mannettes
    /// </summary>
    /// <returns></returns>
    public bool leftAll()
    {
        if (getPlayer(0).GetAxis("UIHorizontal") < 0
           || getPlayer(1).GetAxis("UIHorizontal") < -0.5f
           || getPlayer(2).GetAxis("UIHorizontal") < -0.5f
           || getPlayer(3).GetAxis("UIHorizontal") < -0.5f
           || getPlayer(4).GetAxis("UIHorizontal") < -0.5f
           || getPlayer(5).GetAxis("UIHorizontal") < 0)
            return (true);
        return (false);
    }

    /// <summary>
    /// fonction appelé si OK est appelé par: clavier ou l'une des 4 mannettes
    /// </summary>
    /// <returns></returns>
    public bool submitAll()
    {
        if (getPlayer(0).GetButtonDown("UISubmit")
           || getPlayer(1).GetButtonDown("UISubmit")
           || getPlayer(2).GetButtonDown("UISubmit")
           || getPlayer(3).GetButtonDown("UISubmit")
           || getPlayer(4).GetButtonDown("UISubmit"))
            return (true);
        return (false);
    }

    /// <summary>
    /// test si any input est pressé
    /// </summary>
    /// <returns></returns>
    public bool anyInput()
    {
        if (getPlayer(-1).GetAnyButton()
            || getPlayer(0).GetAnyButton()
            || getPlayer(1).GetAnyButton()
            || getPlayer(2).GetAnyButton()
            || getPlayer(3).GetAnyButton()
            || getPlayer(4).GetAnyButton()
            || getPlayer(5).GetAnyButton())
            return (true);
        return (false);
    }

    /// <summary>
    /// vibration de la mannette
    /// </summary>
    /// <param name="idPlayer"></param>
    /// <param name="vibrationLeft"></param>
    /// <param name="vibrationRight"></param>
    /// <param name="time"></param>
    public void VibrationGamePad(int idPlayer, float vibrationLeft, float vibrationRight, float time)
    {
        return;
        /*
        // Set vibration by motor index
        if (idPlayer == 0 || idPlayer == 5)
            return;
        
        Player currentPlayer = player1;
        switch (idPlayer)
        {
            case 1:
                currentPlayer = player1;
                break;
            case 2:
                currentPlayer = player2;
                break;
            case 3:
                currentPlayer = player3;
                break;
            case 4:
                currentPlayer = player4;
                break;
        }

        foreach (Joystick j in currentPlayer.controllers.Joysticks)
        {
            if (!j.supportsVibration)
                continue;
            if (j.vibrationMotorCount > 0 && vibrationLeft != 0)
                j.SetVibration(0, vibrationLeft, time);
            if (j.vibrationMotorCount > 1 && vibrationRight != 0)
                j.SetVibration(1, vibrationRight, time);
        }*/
    }

    /// <summary>
    /// stop les vibration
    /// </summary>
    /// <param name="idPlayer"></param>
    /// <param name="vibrationLeft"></param>
    /// <param name="vibrationRight"></param>
    /// <param name="time"></param>
    public void StopVibrationGamePad(int idPlayer, float vibrationLeft, float vibrationRight, float time)
    {
        // Set vibration by motor index
        if (idPlayer == 0 || idPlayer == 5)
            return;
        Player currentPlayer = player1;
        switch (idPlayer)
        {
            case 1:
                currentPlayer = player1;
                break;
            case 2:
                currentPlayer = player2;
                break;
            case 3:
                currentPlayer = player3;
                break;
            case 4:
                currentPlayer = player4;
                break;
        }

        foreach (Joystick j in currentPlayer.controllers.Joysticks)
        {
            j.StopVibration();
        }
    }


}