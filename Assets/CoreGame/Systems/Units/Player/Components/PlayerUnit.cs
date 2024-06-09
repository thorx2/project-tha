using System;
using DG.Tweening;
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

        private bool isImmortal = false;

        protected override void Start()
        {
            base.Start();
            isPlayer = true;
        }

        public void RunImmortalFrameTimer()
        {
            isImmortal = true;
            var seq = DOTween.Sequence();
            seq.Append(visual.DOColor(Color.blue, 0.1f));
            seq.Append(visual.DOColor(Color.red, 0.1f));
            seq.Append(visual.DOColor(Color.green, 0.1f));
            seq.Append(visual.DOColor(Color.white, 0.1f));
            seq.OnComplete(() =>
            {
                isImmortal = false;

            });
        }

        public override void TakeDamage(int damage)
        {
            if (!isImmortal)
            {
                base.TakeDamage(damage);
                playerHealthBar.value = HealthPercentage;
                if (lastSfxTime <= 0)
                {
                    lastSfxTime = timeBetweenGrunts;
                    AudioManager.Instance.PlaySFX(takeDamageClip[UnityEngine.Random.Range(0, takeDamageClip.Length)]);
                }
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