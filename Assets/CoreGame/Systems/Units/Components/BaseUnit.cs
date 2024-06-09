using System;
using System.Collections;
using System.Net.NetworkInformation;
using DG.Tweening;
using Lean.Pool;
using SuperMaxim.Messaging;
using Unity.Mathematics;
using UnityEngine;

namespace ProjTha
{
    public class BaseUnit : MonoBehaviour, IMonoUpdateHook, IDamagableObject, IPoolable
    {
        protected bool isPlayer;
        protected UnitData unitData;
        protected float HealthPercentage { get => currentHealth / (float)maxHealth; }

        public bool IsPlayer { get => isPlayer; }
        public float UnitSpeed { get => unitData.MovementSpeed; }

        [SerializeField]
        private Rigidbody2D rigidbodyRef;

        [SerializeField]
        private Animator unitAnimator;

        [SerializeField]
        protected SpriteRenderer visual;

        private int MOVE_ANIM_HASH = Animator.StringToHash("IsMoving");

        private int currentHealth;

        private int maxHealth;

        private MaterialPropertyBlock propertyBlock;
        private float emissionVal;

        #region Unity
        protected virtual void Start()
        {
            Messenger.Default.Publish(new MonoTransport
            {
                MonoSpawned = this,
                IsSpawned = true
            });

            propertyBlock = new MaterialPropertyBlock();
            visual.GetPropertyBlock(propertyBlock);
        }

        protected virtual void OnDestroy()
        {
            Messenger.Default.Publish(new MonoTransport
            {
                MonoSpawned = this,
                IsSpawned = false
            });
        }
        #endregion

        #region Functional

        public virtual void ReInitWithConfiguration(UnitData newData)
        {
            unitData = newData;
            maxHealth = unitData.MaxHealth;
            currentHealth = maxHealth;
        }

        public void MoveTowardsTarget(Vector2 vector2)
        {
            rigidbodyRef.MovePosition(vector2);
        }

        protected void MaxHealUnit()
        {
            currentHealth = maxHealth;
        }

        #endregion

        #region Custom Mono Hooks
        public bool CanFixedUpdate()
        {
            return gameObject.activeInHierarchy;
        }

        public bool CanUpdate()
        {
            return gameObject.activeInHierarchy;
        }

        public virtual void CustomFixedUpdate()
        {

        }

        public virtual void CustomUpdate()
        {
            if (!IsAlive())
            {
                Messenger.Default.Publish(new OnUnitDeath
                {
                    SpawnLocation = transform.position
                });

                unitAnimator.SetBool(MOVE_ANIM_HASH, false);
                if (!isPlayer)
                {
                    StartCoroutine(FrameSkipDespawn());
                }
            }
            else if (isPlayer)
            {
                if (rigidbodyRef.velocity.x != 0)
                {
                    visual.flipX = rigidbodyRef.velocity.x > 0;
                }
                unitAnimator.SetBool(MOVE_ANIM_HASH, rigidbodyRef.velocity.sqrMagnitude > 0.1);
            }
        }

        private IEnumerator FrameSkipDespawn()
        {
            yield return new WaitForEndOfFrame();
            LeanPool.Despawn(gameObject);
        }
        #endregion

        #region IDamagable

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
            currentHealth = math.max(currentHealth, 0);
            var seq = DOTween.Sequence();

            if (isPlayer && currentHealth > 0)
            {
                propertyBlock.SetColor("_Color", Color.red);
                visual.SetPropertyBlock(propertyBlock);
                seq.Append(DOTween.To(() => emissionVal, (x) =>
                {
                    emissionVal = x;
                    propertyBlock.SetFloat("_Blend", emissionVal);
                    visual.SetPropertyBlock(propertyBlock);
                }, 1f, 0.1f).SetEase(Ease.InSine));
                seq.Append(DOTween.To(() => emissionVal, (x) =>
                {
                    emissionVal = x;
                    propertyBlock.SetFloat("_Blend", emissionVal);
                    visual.SetPropertyBlock(propertyBlock);
                }, 0f, 0.1f));
            }
            else
            {
                seq.Append(DOTween.To(() => emissionVal, (x) =>
                {
                    emissionVal = x;
                    propertyBlock.SetFloat("_Blend", emissionVal);
                    visual.SetPropertyBlock(propertyBlock);
                }, 1f, 0.1f).SetEase(Ease.InSine));
                seq.Append(DOTween.To(() => emissionVal, (x) =>
                {
                    emissionVal = x;
                    propertyBlock.SetFloat("_Blend", emissionVal);
                    visual.SetPropertyBlock(propertyBlock);
                }, 0f, 0.1f));
            }

        }

        public bool IsAlive()
        {
            return currentHealth > 0;
        }
        #endregion

        #region Pooling

        public virtual void OnSpawn()
        {
            isPlayer = false;
            Messenger.Default.Publish(new EnemyTransport()
            {
                IsActivated = true,
                unitActivated = this
            });
            unitAnimator.SetBool(MOVE_ANIM_HASH, true);
        }

        public virtual void OnDespawn()
        {
            unitAnimator.SetBool(MOVE_ANIM_HASH, false);
            Messenger.Default.Publish(new EnemyTransport()
            {
                IsActivated = false,
                unitActivated = this
            });
            currentHealth = -1;
        }

        #endregion
    }

}