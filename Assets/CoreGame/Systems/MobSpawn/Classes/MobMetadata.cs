using System;
using UnityEngine;

namespace ProjTha
{
    [Serializable]
    public struct MobMetadata
    {
        public int Level;
        public float SpawnRate;
        public BaseUnit SpawnPrefab;
        public int CountPerSpawnCall;
        public UnitData CreepUnitData;
    }
}