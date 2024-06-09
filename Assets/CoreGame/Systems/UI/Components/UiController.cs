using System;
using ProjTha;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuCanvas;
    [SerializeField]
    private GameObject inGameCanvas;

    [SerializeField]
    private RoundBoostController roundBoostController;

    [SerializeField]
    private GameOverPanel gameOverPanel;

    private int lastBoostShowLevel;

    [SerializeField]
    private int GameRoundTimer;

    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private TMP_Text levelText;

    private float timeRemaining;

    private bool gameRunning = false;

    protected void Start()
    {
        Messenger.Default.Subscribe<GameStateData>(OnGameStateChange);
        Messenger.Default.Subscribe<PlayerData>(OnPlayerDataUpdated);
        Messenger.Default.Subscribe<GameEndData>(OnGameEnd);
    }

    private void OnGameEnd(GameEndData data)
    {
        gameOverPanel.ShowUIFor(data.IsWin);
    }

    protected void OnDestroy()
    {
        Messenger.Default.Unsubscribe<GameStateData>(OnGameStateChange);
        Messenger.Default.Unsubscribe<PlayerData>(OnPlayerDataUpdated);
        Messenger.Default.Unsubscribe<GameEndData>(OnGameEnd);
    }

    protected void Update()
    {
        if (gameRunning && !GameManager.PauseElementMovement)
        {
            var timeRemaining = GameManager.Instance.GameRoundDurationInSeconds - GameManager.Instance.GetPlayerData.CurrentRunDuration;
            TimeSpan time = TimeSpan.FromSeconds(timeRemaining);
            timerText.text = time.ToString(@"mm\:ss");
        }
    }

    private void OnPlayerDataUpdated(PlayerData data)
    {
        if (lastBoostShowLevel < data.CurrentPlayerLevel)
        {
            roundBoostController.gameObject.SetActive(true);
            lastBoostShowLevel = data.CurrentPlayerLevel;
            roundBoostController.ShowUI();
            levelText.text = $"{data.CurrentPlayerLevel}";
        }
    }

    private void OnGameStateChange(GameStateData data)
    {
        switch (data.CurrentState)
        {
            case EGameState.EGameInMenu:
                mainMenuCanvas.SetActive(true);
                inGameCanvas.SetActive(false);
                break;
            case EGameState.EGameRunning:
                lastBoostShowLevel = 1;
                inGameCanvas.SetActive(true);
                mainMenuCanvas.SetActive(false);
                gameRunning = true;
                break;
            case EGameState.EGameOver:
                gameOverPanel.gameObject.SetActive(true);
                break;
        }
    }

    public void StartGameRequested()
    {
        GameManager.Instance.StartGame();
    }

    public void OnGameRestart()
    {
        Messenger.Default.Publish(new GameStateData()
        {
            CurrentState = EGameState.EGameRunning
        });
    }

    public void OnGameExit()
    {
        Messenger.Default.Publish(new GameStateData()
        {
            CurrentState = EGameState.EGameInMenu
        });
    }
}
