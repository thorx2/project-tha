using System.Collections.Generic;
using Lean.Pool;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

namespace ProjTha
{
    public class SpawnController : MonoBehaviour, IMonoUpdateHook
    {
        [SerializeField]
        private MobSpawnConfig spawnConfig;

        [SerializeField]
        private float spawnDistance;

        private int playerLevel;

        private Dictionary<BaseUnit, float> spawnTimerData = new();

        public bool CanFixedUpdate()
        {
            return false;
        }

        public bool CanUpdate()
        {
            return true;
        }

        public void CustomFixedUpdate()
        {

        }

        public void CustomUpdate()
        {
            var currentTimeTick = Time.realtimeSinceStartup;
            foreach (var creep in spawnConfig.MobSpawnConfigList)
            {
                if (playerLevel >= creep.Level)
                {
                    if (spawnTimerData.TryGetValue(creep.SpawnPrefab, out var timer))
                    {
                        var adjustedTimer = creep.SpawnRate - (creep.SpawnRate * spawnConfig.LevelSpawnDeltaFactor[playerLevel]);
                        if (currentTimeTick - timer > adjustedTimer)
                        {
                            var spawnedCreep = LeanPool.Spawn(creep.SpawnPrefab, GetRandomPositionOutsideCameraView(), Quaternion.identity);
                            spawnedCreep.ReInitWithConfiguration(creep.CreepUnitData);
                            spawnTimerData[creep.SpawnPrefab] = currentTimeTick;
                        }
                    }
                    else
                    {
                        var spawnedCreep = LeanPool.Spawn(creep.SpawnPrefab, GetRandomPositionOutsideCameraView(), Quaternion.identity);
                        spawnedCreep.ReInitWithConfiguration(creep.CreepUnitData);
                        spawnTimerData.Add(creep.SpawnPrefab, currentTimeTick);
                    }
                }
            }
        }

        protected void Start()
        {
            Messenger.Default.Publish(new MonoTransport()
            {
                IsSpawned = true,
                MonoSpawned = this
            });

            Messenger.Default.Subscribe<PlayerData>(OnPlayerDataChange);
        }

        protected void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerData>(OnPlayerDataChange);
        }

        private void OnPlayerDataChange(PlayerData data)
        {
            playerLevel = data.CurrentPlayerLevel;
        }

        private Vector3 GetRandomPositionOutsideCameraView()
        {
            //Again Camera.main is fine now, Unity fixed that original blatant bug...
            float camHeight = 2f * Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            Vector3 camPosition = Camera.main.transform.position;

            float minX = camPosition.x - camWidth / 2 - spawnDistance;
            float maxX = camPosition.x + camWidth / 2 + spawnDistance;
            float minY = camPosition.y - camHeight / 2 - spawnDistance;
            float maxY = camPosition.y + camHeight / 2 + spawnDistance;
            bool spawnOnX = Random.Range(0, 2) == 0;
            float spawnX, spawnY;
            if (spawnOnX)
            {
                spawnX = Random.Range(0, 2) == 0 ? Random.Range(minX, camPosition.x - camWidth / 2) : Random.Range(camPosition.x + camWidth / 2, maxX);
                spawnY = Random.Range(minY, maxY);
            }
            else
            {
                spawnX = Random.Range(minX, maxX);
                spawnY = Random.Range(0, 2) == 0 ? Random.Range(minY, camPosition.y - camHeight / 2) : Random.Range(camPosition.y + camHeight / 2, maxY);
            }
            return new Vector3(spawnX, spawnY, 0);
        }
    }
}