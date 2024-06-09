using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjTha
{
    public class PlayerUnit : BaseUnit
    {
        [SerializeField]
        private Movement unitMovement;

        public Movement MovementCompRef { get => unitMovement; }

        [SerializeField]
        private Slider playerHealthBar;

        [SerializeField]
        private WeaponController weaponController;

        private float AdditionalHP;

        public override void OnSpawn()
        {
            isPlayer = true;
        }

        public override void OnDespawn()
        {

        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            playerHealthBar.value = HealthPercentage;
            Debug.Log($"Player health at {playerHealthBar.value * 100}%");
        }

        public override void ReInitWithConfiguration(UnitData newData)
        {
            base.ReInitWithConfiguration(newData);
            OnLevelUp();
        }

        public override void CustomFixedUpdate()
        {
            base.CustomFixedUpdate();
            unitMovement.CustomFixedTick();
        }

        public override void CustomUpdate()
        {
            base.CustomUpdate();
            if (!IsAlive())
            {
                GameManager.Instance.OnGameOver(false);
                Destroy(gameObject);
            }
            else
            {
                weaponController.TickUpdate();
            }
        }

        public void OnLevelUp()
        {
            MaxHealUnit();
            playerHealthBar.value = 1;
        }
    }
}