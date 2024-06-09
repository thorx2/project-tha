using System.Collections.Generic;
using UnityEngine;

namespace ProjTha
{
    [CreateAssetMenu(fileName = "SpawnConfiguration", menuName = "ProjectTha/Gameplay/SpawnConfig")]
    public class MobSpawnConfig : ScriptableObject
    {
        public MobMetadata[] MobSpawnConfigList;

        //COntains information on how to change the spawn duration as the player progresses in level
        public SerializableDictionary<int, float> LevelSpawnDeltaFactor;
    }
}