using UnityEngine;

namespace ProjTha
{
    [CreateAssetMenu(fileName = "Gameplay Configuration", menuName = "ProjectTha/Gameplay/Game Configuration")]
    public class GameCoreParameters : ScriptableObject
    {
        public int AdditionalAttackDamage;
        public int AdditionalHP;
        public float BaseReductionInAttackInterval;

        public int RoundDurationInMins;
        public int XpPerPickup;
        public SerializableDictionary<int, int> XpLevelMapData;
    }
}