using UnityEngine;

namespace ProjTha
{
    public class PlayerData
    {
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
        }
    }
}