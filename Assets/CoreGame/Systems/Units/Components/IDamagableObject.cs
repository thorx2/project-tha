using Unity.Mathematics;
using UnityEngine;

namespace ProjTha
{
    /// <summary>
    /// Simple interface to be used for to search for any damageable system or object in the world
    /// </summary>
    public interface IDamagableObject
    {
        int GetMaxHealth();
        int GetCurrentHealth();
        void TakeDamage(int damage);
        bool IsAlive();

    }
}