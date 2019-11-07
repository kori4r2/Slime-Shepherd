using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Selectable firstActiveMainMenuItem;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Selectable firstActiveOptionsMenuItem;
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private Selectable firstActiveGameModeMenuItem;
    [SerializeField] private Selectable firstActiveCreditsMenuItem;
    private GameObject lastSelect;

    public float timeLimit = 10;
    public float targetScore = 10;

    // Start is called before the first frame update
    void Start()
    {
        Shepherd.instance.GetComponent<Movable>().CanMove = false;
        lastSelect = firstActiveMainMenuItem.gameObject;
        GoToMainMenu();
    }

    public void GoToMainMenu(){
        gameModeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        creditsPanel.SetActive(false);
        firstActiveMainMenuItem.Select();
    }

    public void GoToGameModeSelection(){
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        gameModeMenu.SetActive(true);
        creditsMenu.SetActive(false);
        creditsPanel.SetActive(false);
        firstActiveGameModeMenuItem.Select();
    }

    public void GoToOptions(){
        mainMenu.SetActive(false);
        gameModeMenu.SetActive(false);
        optionsMenu.SetActive(true);
        creditsMenu.SetActive(false);
        creditsPanel.SetActive(false);
        firstActiveOptionsMenuItem.Select();
    }

    public void GoToCredits(){
        mainMenu.SetActive(false);
        gameModeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(true);
        creditsPanel.SetActive(true);
        firstActiveCreditsMenuItem.Select();
    }

    public void StartTimeAttackGame(){
        GameManager.GameMode = GameModes.TimeAttack;
        GameManager.TargetScore = targetScore;
        SceneManager.LoadScene("Woods1");
    }

    public void StartHighScoreGame(){
        GameManager.GameMode = GameModes.HighScore;
        GameManager.TimeLimit = timeLimit;
        SceneManager.LoadScene("Woods1");
    }

    public void QuitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void Update(){
        if(EventSystem.current.currentSelectedGameObject == null){
            EventSystem.current.SetSelectedGameObject(lastSelect);
        }else{
            lastSelect = EventSystem.current.currentSelectedGameObject;
        }
    }
}
