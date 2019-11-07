using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWindow : MonoBehaviour
{

    void Start()
    {
        GameManager.instance.Pause();
    }

    public void Unpause()
    {
        GameManager.instance.Unpause();
    }

}
