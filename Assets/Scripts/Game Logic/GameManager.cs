using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameModes{
    HighScore,
    TimeAttack,
    Null
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text gameModeName;
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private GameObject gameOverUI;
    private float timer;
    private static bool gameStarted = false;
    public static GameModes GameMode{
        get => gameMode;
        set{
            if(!gameStarted){
                gameMode = value;
            }
        }
    }
    private static GameModes gameMode;
    public static float TimeLimit{
        get => timeLimit;
        set{
            if(!gameStarted){
                timeLimit = value;
            }
        }
    }
    private static float timeLimit;
    public static float TargetScore{
        get => targetScore;
        set{
            if(!gameStarted){
                targetScore = value;
            }
        }
    }
    private static float targetScore;

    // Isto deve ser um singleton
    public static GameManager instance = null;
    public void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }
    public void OnDestroy(){
        if(instance == this){
            instance = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        gameStarted = true;
    }

    public static string GetTimeString(float timer){
        System.TimeSpan timeSpan = new System.TimeSpan(0, 0, (int)timer);
        string format = (timeSpan.TotalHours >= 1? "h\\:" : "") + (timeSpan.TotalMinutes >= 1? "mm\\:" : "") + "ss";
        return string.Format("{0:" + format + "}", timeSpan);
    }

    private void CheckGameEnd(){
        switch(gameMode){
            case GameModes.HighScore:
                if(timer > timeLimit){
                    GameOver(Slime.HerdSize);
                }
                break;
            case GameModes.TimeAttack:
                if(Slime.HerdSize >= targetScore){
                    GameOver(timer);
                }
                break;
        }
    }

    // Função de game over para quando o jogador perder
    public void GameOver(){
        Shepherd.instance.Lost();
        switch(gameMode){
            case GameModes.TimeAttack:
                GameOver(timer, true);
                break;
            default:
                GameOver((int)0, true);
                break;
        }
    }

    // Função de game over para quando acaba o time attack
    private void GameOver(float time, bool lost = false){
        gameStarted = false;
        Shepherd.instance.Stop();
        foreach(Enemy enemy in FindObjectsOfType<Enemy>()){
            enemy.Stop();
        }
        finalScoreText.text = GetTimeString(time);
        gameOverUI.SetActive(true);
    }

    // Função de game over para quando acaba o tempo do high score challenge
    private void GameOver(int slimeCount, bool lost = false){
        gameStarted = false;
        Shepherd.instance.Stop();
        foreach(Enemy enemy in FindObjectsOfType<Enemy>()){
            enemy.Stop();
        }
        finalScoreText.text = slimeCount + " slimes";
        gameOverUI.SetActive(true);
    }

    public void GoToMainMenu(){
        // TO DO: Muda de cena pro menu principal
    }

    public void Pause(){
        Time.timeScale = 0f;
        // TO DO: Spawna as UI na tela
    }

    public void Unpause(){
        Time.timeScale = 0f;
        // TO DO: Spawna as UI na tela
    }

    // Update is called once per frame
    void Update()
    {
        if(gameStarted){
            System.TimeSpan timeSpan = new System.TimeSpan(0, 0, (int)timer);
            if(timeSpan.Days > 0){
                timerText.text = "STOP!";
            }else{
                timerText.text = GetTimeString(timer);
            }
            scoreText.text = Slime.HerdSize + ((GameMode == GameModes.TimeAttack)? ("/" + targetScore) : "");
            //CheckGameEnd();

            timer += Time.deltaTime;
            // A pessoa precisa jogar por quase 25 mil dias pra isso aqui ser relevante, mas vai que
            timer = Mathf.Min(timer, int.MaxValue);
        }
    }
}
