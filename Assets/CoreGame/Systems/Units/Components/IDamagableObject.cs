using Unity.Mathematics;
using UnityEngine;

namespace ProjTha
{
    public interface IDamagableObject
    {
        int GetMaxHealth();
        int GetCurrentHealth();
        void TakeDamage(int damage);
        bool IsAlive();

    }
}