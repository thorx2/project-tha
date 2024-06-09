using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjTha
{
    public class PlayerUnit : BaseUnit
    {
        public UserMovementInput MovementCompRef { get => unitMovement; }

        [SerializeField]
        private UserMovementInput unitMovement;

        [SerializeField]
        private Slider playerHealthBar;

        [SerializeField]
        private WeaponController weaponController;

        protected override void Start()
        {
            base.Start();
            isPlayer = true;
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