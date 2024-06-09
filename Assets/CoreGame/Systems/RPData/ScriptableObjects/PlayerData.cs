using UnityEngine;

namespace ProjTha
{
    /// <summary>
    /// Per game run data container, will be created and reset on each start/restart of the round
    /// </summary>
    public class PlayerData
    {
        public float CurrentRunDuration;

        public int CurrentPlayerLevel;
        public int CurrentXPValue;
        public float LevelProgressionPercentage;

        public float WeaponFireRateReduction;
        public float AdditionalBoostHealth;
        public float AdditionalAttackDamage;

        public PlayerData(int lvl)
        {
            CurrentPlayerLevel = lvl;
            CurrentXPValue = 0;
            LevelProgressionPercentage = 0;
            WeaponFireRateReduction = 0;
            AdditionalBoostHealth = 0;
            AdditionalAttackDamage = 0;
            CurrentRunDuration = 0;
        }
    }
}