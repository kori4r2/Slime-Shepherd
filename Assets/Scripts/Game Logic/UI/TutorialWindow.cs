using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWindow : MonoBehaviour
{

    public TMPro.TMP_Text gamemodeTitle;
    public TMPro.TMP_Text gamemodeDescription;


    public string titleTimeAttack = "Time Attack";
    public string titleHighScore = "High Score";

    void Start()
    {
        GameManager.instance.Pause();
        Shepherd.instance.GetComponent<Movable>().CanMove = false;

        if(GameManager.GameMode == GameModes.TimeAttack)
        {
            gamemodeTitle.text = titleTimeAttack;
            gamemodeDescription.text = "Get yourself a herd of " + GameManager.TargetScore + " slimes as quick as posible!";
        }
        else
        {
            gamemodeTitle.text =       titleHighScore;
            gamemodeDescription.text = "Get yourself a plenty of slimes in " + GameManager.TimeLimit + " seconds!";
        }


    }

    public void Unpause()
    {
        GameManager.instance.Unpause();
        Shepherd.instance.GetComponent<Movable>().CanMove = true;
    }

}
