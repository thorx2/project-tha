using System;
using System.Collections.Generic;
using Lean.Pool;
using SuperMaxim.Messaging;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ProjTha
{
    public class HoadMovementManager : MonoBehaviour
    {
        public Transform playerTransformRef;
        private List<BaseUnit> homingObjects = new();

        protected void Start()
        {
            Messenger.Default.Subscribe<EnemyTransport>(OnEnemyStatusChange);
        }

        protected void OnDestroy()
        {
            Messenger.Default.Unsubscribe<EnemyTransport>(OnEnemyStatusChange);
        }

        protected void FixedUpdate()
        {
            int count = homingObjects.Count;

            if (count == 0 || GameManager.PauseElementMovement || playerTransformRef == null) return;

            NativeArray<Vector2> positions = new NativeArray<Vector2>(count, Allocator.TempJob);
            NativeArray<Vector2> newPositions = new NativeArray<Vector2>(count, Allocator.TempJob);
            NativeArray<Vector2> directions = new NativeArray<Vector2>(count, Allocator.TempJob);
            NativeArray<Vector3> transforms = new NativeArray<Vector3>(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
            {
                transforms[i] = homingObjects[i].transform.position;
            }

            HomingJob job = new HomingJob
            {
                transforms = transforms,
                targetPosition = playerTransformRef.position,
                speed = homingObjects[0].UnitSpeed,
                deltaTime = Time.fixedDeltaTime,
                positions = positions,
                directions = directions,
                newPositions = newPositions
            };

            JobHandle handle = job.Schedule(count, 64);
            handle.Complete();

            for (int i = 0; i < count; i++)
            {
                homingObjects[i].MoveTowardsTarget(newPositions[i]);
            }

            positions.Dispose();
            directions.Dispose();
            newPositions.Dispose();
            transforms.Dispose();
        }

        private void OnEnemyStatusChange(EnemyTransport transport)
        {
            if (transport.IsActivated)
            {
                homingObjects.Add(transport.unitActivated);
            }
            else
            {
                homingObjects.Remove(transport.unitActivated);
            }
        }

        public void SetTargetDestination(PlayerUnit spawnedPlayer)
        {
            playerTransformRef = spawnedPlayer.transform;
        }
    }
}