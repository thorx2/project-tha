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

        [SerializeField]
        private int surviveDurationInMins;

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

        private float aliveDuration;

        //TODO Make this a scriptable object for OTA turing using Addressables?
        //For now, key is level and value is the XP required to attain that level (Cumulative)
        [SerializeField]
        private SerializableDictionary<int, int> XpLevelMapData;


        private int MaxLevelAttainable;

        protected override void SingletonAwakened()
        {
            currentRunData = new(1);
            foreach (var item in XpLevelMapData)
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
            if (spawnedPlayer != null && spawnedPlayer.IsAlive())
            {
                aliveDuration += Time.deltaTime;

                if (aliveDuration > surviveDurationInMins * 60)
                {
                    //YOU WIN HERE
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

            currentRunData.CurrentXPValue += amount.XpAddOnCollect;

            foreach (var item in XpLevelMapData)
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
                return;
            }

            if (newPossibleLevel != currentRunData.CurrentPlayerLevel)
            {
                currentRunData.CurrentPlayerLevel = newPossibleLevel;
                spawnedPlayer.OnLevelUp();
            }

            float top = currentRunData.CurrentXPValue - XpLevelMapData[currentRunData.CurrentPlayerLevel];

            float bottom = XpLevelMapData[currentRunData.CurrentPlayerLevel + 1] - XpLevelMapData[currentRunData.CurrentPlayerLevel];

            currentRunData.LevelProgressionPercentage = top / bottom;

            Messenger.Default.Publish(currentRunData);
        }

        public void StartGame()
        {
            ResetRun();
            spawnedPlayer = Instantiate(playerPrefab);
            spawnedPlayer.ReInitWithConfiguration(playerConfiguration);
            movementManager.SetTargetDestination(spawnedPlayer);
            mapManager.SetPlayerMovementRef(spawnedPlayer.MovementCompRef);
            aliveDuration = 0;
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
    }
}