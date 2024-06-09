using System;
using Cinemachine;
using Lean.Pool;
using SuperMaxim.Messaging;
using Unity.Mathematics;
using UnityEngine;

namespace ProjTha
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        public static bool PauseElementMovement = true;

        public PlayerData GetPlayerData { get => currentRunData; }

        private PlayerData currentRunData;

        public float GameRoundDurationInSeconds { get => gameCoreParameters.RoundDurationInMins * 60; }
        [SerializeField]
        private GameCoreParameters gameCoreParameters;


        //TODO Move to spawn system? All spawning handled by one single class
        [SerializeField]
        private PlayerUnit playerPrefab;

        [SerializeField]
        private UnitData playerConfiguration;

        [SerializeField]
        private HoadMovementManager movementManager;

        [SerializeField]
        private MapController mapManager;

        [SerializeField]
        private CinemachineVirtualCamera followCam;

        private PlayerUnit spawnedPlayer;

        private int MaxLevelAttainable;

        protected override void SingletonAwakened()
        {
            currentRunData = new(1);
            foreach (var item in gameCoreParameters.XpLevelMapData)
            {
                MaxLevelAttainable = math.max(MaxLevelAttainable, item.Key);
            }

        }

        protected override void SingletonStarted()
        {
            Messenger.Default.Subscribe<GameStateData>(OnGameStateChanged);
            Messenger.Default.Subscribe<OnXpCollect>(OnPlayerCollectXP);
        }

        protected void Update()
        {
            if (!PauseElementMovement && spawnedPlayer != null && spawnedPlayer.IsAlive())
            {
                currentRunData.CurrentRunDuration += Time.deltaTime;

                if (currentRunData.CurrentRunDuration > gameCoreParameters.RoundDurationInMins * 60)
                {
                    OnGameOver(true);
                }
            }
        }

        public void ResetRun()
        {
            currentRunData = new(1);

            if (spawnedPlayer != null)
            {
                Destroy(spawnedPlayer.gameObject);
            }
        }

        public void StartGame()
        {
            ResetRun();
            spawnedPlayer = Instantiate(playerPrefab);
            spawnedPlayer.ReInitWithConfiguration(playerConfiguration);
            movementManager.SetTargetDestination(spawnedPlayer);
            mapManager.SetPlayerMovementRef(spawnedPlayer.MovementCompRef);
            PauseElementMovement = false;
            followCam.LookAt = spawnedPlayer.transform;
            followCam.Follow = spawnedPlayer.transform;
            Messenger.Default.Publish(new GameStateData()
            {
                CurrentState = EGameState.EGameRunning
            });
            Messenger.Default.Publish(currentRunData);
        }

        public void OnGameOver(bool isWin)
        {
            Messenger.Default.Publish(new GameStateData()
            {
                CurrentState = EGameState.EGameOver
            });
            Messenger.Default.Publish(new GameEndData()
            {
                IsWin = isWin,
            });
        }

        public void RestartGame()
        {
            LeanPool.DespawnAll();
            StartGame();
        }

        public void BoostSelected(EBoostType selectedBoost)
        {
            spawnedPlayer.RunImmortalFrameTimer();
            switch (selectedBoost)
            {
                case EBoostType.ERateOfFire:
                    currentRunData.WeaponFireRateReduction += gameCoreParameters.BaseReductionInAttackInterval;
                    break;
                case EBoostType.EDamage:
                    currentRunData.AdditionalAttackDamage += gameCoreParameters.AdditionalAttackDamage;
                    break;
                case EBoostType.EHealth:
                    currentRunData.AdditionalBoostHealth += gameCoreParameters.AdditionalHP;
                    break;
            }
        }

        private void OnGameStateChanged(GameStateData data)
        {
            switch (data.CurrentState)
            {
                case EGameState.EGameInMenu:
                    {
                        PauseElementMovement = true;
                        LeanPool.DespawnAll();
                        Destroy(spawnedPlayer.gameObject);
                    }
                    break;
            }
        }

        private void OnPlayerCollectXP(OnXpCollect amount)
        {
            //Early return on player max level
            if (MaxLevelAttainable == currentRunData.CurrentPlayerLevel)
            {
                currentRunData.LevelProgressionPercentage = 1f;
                return;
            }

            int newPossibleLevel = 0;

            currentRunData.CurrentXPValue += gameCoreParameters.XpPerPickup;

            foreach (var item in gameCoreParameters.XpLevelMapData)
            {
                if (item.Value <= currentRunData.CurrentXPValue)
                {
                    newPossibleLevel = math.max(item.Key, newPossibleLevel);
                }
            }

            //Again if you leveled up to max now, looks weird, needs more change
            if (MaxLevelAttainable == newPossibleLevel)
            {
                currentRunData.CurrentPlayerLevel = newPossibleLevel;
                currentRunData.LevelProgressionPercentage = 1f;
                if (newPossibleLevel != currentRunData.CurrentPlayerLevel)
                {
                    currentRunData.CurrentPlayerLevel = newPossibleLevel;
                    spawnedPlayer.OnLevelUp();
                }
                Messenger.Default.Publish(currentRunData);
                return;
            }

            if (newPossibleLevel != currentRunData.CurrentPlayerLevel)
            {
                currentRunData.CurrentPlayerLevel = newPossibleLevel;
                spawnedPlayer.OnLevelUp();
            }

            float top = currentRunData.CurrentXPValue - gameCoreParameters.XpLevelMapData[currentRunData.CurrentPlayerLevel];

            float bottom = gameCoreParameters.XpLevelMapData[currentRunData.CurrentPlayerLevel + 1] - gameCoreParameters.XpLevelMapData[currentRunData.CurrentPlayerLevel];

            currentRunData.LevelProgressionPercentage = top / bottom;

            Messenger.Default.Publish(currentRunData);
        }
    }
}