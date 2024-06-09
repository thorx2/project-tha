using System;
using Unity.Mathematics;
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

        [SerializeField]
        private AudioClip[] takeDamageClip;
        [SerializeField]
        private float timeBetweenGrunts;
        private float lastSfxTime;

        protected override void Start()
        {
            base.Start();
            isPlayer = true;
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            playerHealthBar.value = HealthPercentage;
            if (lastSfxTime <= 0)
            {
                lastSfxTime = timeBetweenGrunts;
                AudioManager.Instance.PlaySFX(takeDamageClip[UnityEngine.Random.Range(0, takeDamageClip.Length)]);
            }
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

            if (lastSfxTime > 0)
            {
                lastSfxTime -= Time.deltaTime;
            }

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