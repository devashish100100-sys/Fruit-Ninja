using UnityEngine;
using UnityEngine.SceneManagement;

public enum State { MainMenu, PauseMenu, Playing, GameOver };

public class GameManager : MonoSingleton<GameManager>
{
    public State State { get; private set; } = State.MainMenu;

    [Header("Gameplay Settings")]
    [SerializeField] int score;
    [SerializeField] int lives = 3;
    int actualScore;
    int actualLives;

    [Header("UI References")]
    [SerializeField] UIManager uiManager;
    [SerializeField] GameOverUI gameOverUI;  

    void Start()
    {
        actualScore = score;
        actualLives = lives;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == State.Playing)
            {
                PauseGame();
            }
            else if (State == State.PauseMenu)
            {
                ContinueGame();
            }
        }
    }

    public void StartGame()
    {
        actualScore = score;
        actualLives = lives;
        uiManager.UpdateScore(score);
        uiManager.UpdateBestScore(PlayerPrefs.GetInt("best score"));
        uiManager.UpdateLives(lives);
        uiManager.DisplayMainMenu(false);
        uiManager.DisplayHUD(true);

        if (gameOverUI != null)
            gameOverUI.HideGameOver(); 

        State = State.Playing;
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        if (State != State.Playing) return;

        State = State.PauseMenu;
        Time.timeScale = 0f;
        uiManager.DisplayPauseMenu(true);
    }

    public void ContinueGame()
    {
        uiManager.DisplayPauseMenu(false);
        Time.timeScale = 1f;
        State = State.Playing;
    }

    public void GameOver()
    {
        Debug.Log("⚠️ GAME OVER TRIGGERED");

        if (State == State.Playing)
        {
            if (actualScore > PlayerPrefs.GetInt("best score"))
                PlayerPrefs.SetInt("best score", actualScore);

            uiManager.DisplayHUD(false);

            if (gameOverUI != null)
            {
                gameOverUI.ShowGameOver();
            }

            State = State.GameOver;
        }
    }

    public void AddScore(int scoreToAdd)
    {
        actualScore += scoreToAdd;
        uiManager.UpdateScore(actualScore);
    }

    public void RemoveAllLives()
    {
        actualLives = 0;
        uiManager.UpdateLives(actualLives);
        GameOver();
    }

    public void RemoveLife()
    {
        actualLives--;
        uiManager.UpdateLives(actualLives);
        if (actualLives <= 0)
        {
            GameOver();
        }
    }
}
