using System;
using System.Collections;
using System.Collections.Generic;
using ProjTha;
using SuperMaxim.Messaging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    [SerializeField]
    private Slider xpBar;

    private float targetXpValue;

    protected void Start()
    {
        Messenger.Default.Subscribe<PlayerData>(OnPlayerDataUpdate);

        Messenger.Default.Subscribe<GameStateData>(OnGameStateChange);
    }

    protected void OnDestroy()
    {
        Messenger.Default.Unsubscribe<PlayerData>(OnPlayerDataUpdate);

        Messenger.Default.Unsubscribe<GameStateData>(OnGameStateChange);
    }

    private void OnGameStateChange(GameStateData data)
    {
        if (data.CurrentState == EGameState.EGameRunning)
        {
            xpBar.value = 0;
        }
    }

    private void OnPlayerDataUpdate(PlayerData data)
    {
        targetXpValue = data.LevelProgressionPercentage;
        xpBar.value = targetXpValue;
    }
}
