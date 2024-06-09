using UnityEngine;

namespace ProjTha
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "ProjectTha/Units/BaseData")]
    public class UnitData : ScriptableObject
    {
        public int MaxHealth;
        public int DamagePerTick;
        public float DamageRate;
        public float MovementSpeed;
    }
}