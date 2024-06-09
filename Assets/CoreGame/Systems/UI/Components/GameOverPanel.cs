using System;
using Lean.Gui;
using ProjTha;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField]
    private LeanWindow leanWindow;

    [SerializeField]
    private TMP_Text contextMessage;

    public void ShowUIFor(bool isWin)
    {
        GameManager.PauseElementMovement = true;
        leanWindow.On = true;
        contextMessage.text = isWin ? "You Survived!!\nCare to try again?" : "Aww!! you tried\nCare to try again?";
    }

    public void GameRestartRequested()
    {
        GameManager.Instance.RestartGame();
        leanWindow.On = false;
    }

    public void QuitGame()
    {
        leanWindow.On = false;
        Messenger.Default.Publish(new GameStateData()
        {
            CurrentState = EGameState.EGameInMenu
        });
    }
}