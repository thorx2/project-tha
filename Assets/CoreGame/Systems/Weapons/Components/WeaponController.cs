using System.Collections.Generic;
using Lean.Pool;
using SuperMaxim.Messaging;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace ProjTha
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField]
        private Projectile spawnProjectilePrefab;

        [SerializeField]
        private float fireInterval;

        [SerializeField]
        private SpriteRenderer visual;

        private float lastFiredTick;

        private List<BaseUnit> unitsNearby = new();

        private List<BaseUnit> sortedTransforms = new();

        private BaseUnit nearestTarget;

        public void TickUpdate()
        {
            //TODO Needs a faster method, maybe SystemAPIs perhaps? Revisit later if possible.
            lastFiredTick -= Time.deltaTime;
            if (lastFiredTick < 0)
            {
                FireWeaponToTarget();
            }
        }

        private void FireWeaponToTarget()
        {
            lastFiredTick = fireInterval - GameManager.Instance.GetPlayerData.WeaponFireRateReduction;
            lastFiredTick = math.max(0.1f, lastFiredTick);
            var projectile = LeanPool.Spawn(spawnProjectilePrefab, transform.position, Quaternion.identity);
            projectile.SetAdditionalDamage(GameManager.Instance.GetPlayerData.AdditionalAttackDamage);
            projectile.SetTargetTransform(nearestTarget, visual.flipX ? Vector2.right : -Vector2.right);
        }


        protected void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<BaseUnit>(out var x))
            {
                unitsNearby.Add(x);
                SortTransformsByDistance();
                nearestTarget = sortedTransforms.Count > 0 ? sortedTransforms[0] : null;
            }
        }

        protected void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.TryGetComponent<BaseUnit>(out var x))
            {
                unitsNearby.Remove(x);
                SortTransformsByDistance();
                nearestTarget = sortedTransforms.Count > 0 ? sortedTransforms[0] : null;
            }
        }

        private void SortTransformsByDistance()
        {
            int count = unitsNearby.Count;


            if (sortedTransforms.Capacity < count)
            {
                sortedTransforms.Capacity = count;
            }
            sortedTransforms.Clear();

            if (count == 0) return;

            NativeArray<TransformData> transformDataArray = new NativeArray<TransformData>(count, Allocator.TempJob);
            NativeArray<float> distances = new NativeArray<float>(count, Allocator.TempJob);
            NativeArray<int> indices = new NativeArray<int>(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
            {
                transformDataArray[i] = new TransformData { Index = i, Position = unitsNearby[i].transform.position };
                indices[i] = i;
            }

            DistanceCalculationJob distanceJob = new()
            {
                Transforms = transformDataArray,
                Distances = distances,
                TargetPosition = transform.position
            };

            JobHandle distanceHandle = distanceJob.Schedule(count, 64);
            distanceHandle.Complete();

            SortDistancesJob sortJob = new SortDistancesJob
            {
                Distances = distances,
                Indices = indices
            };

            JobHandle sortHandle = sortJob.Schedule();
            sortHandle.Complete();

            for (int i = 0; i < count; i++)
            {
                sortedTransforms.Add(unitsNearby[indices[i]]);
            }

            unitsNearby.Clear();
            unitsNearby.AddRange(sortedTransforms);

            transformDataArray.Dispose();
            distances.Dispose();
            indices.Dispose();
        }
    }
}