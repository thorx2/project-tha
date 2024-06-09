using System;
using System.Collections;
using Lean.Pool;
using SuperMaxim.Messaging;
using UnityEngine;

namespace ProjTha
{
    public class Projectile : MonoBehaviour, IMonoUpdateHook, IPoolable
    {
        [SerializeField]
        private float movementSpeed;
        [SerializeField]
        private float turnSpeed;

        [SerializeField]
        private int baseDamage;

        [SerializeField]
        private float lifeTime;

        [SerializeField]
        private AudioClip effectClip;

        [SerializeField]
        private Rigidbody2D rigidbodyRef;

        private BaseUnit targetDestination;

        private int additionalDamage;

        private bool isAlive;

        private Vector2 defaultForward;

        private float aliveDuration;

        protected void Start()
        {
            Messenger.Default.Publish(new MonoTransport
            {
                MonoSpawned = this,
                IsSpawned = true
            });
        }

        protected void OnDestroy()
        {
            Messenger.Default.Publish(new MonoTransport
            {
                MonoSpawned = this,
                IsSpawned = false
            });
        }

        public void SetTargetTransform(BaseUnit target, Vector2 defaultForward)
        {
            targetDestination = target;
            this.defaultForward = defaultForward;
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<BaseUnit>(out var unit))
            {
                if (unit.IsAlive())
                {
                    unit.TakeDamage(baseDamage + additionalDamage);
                    StartCoroutine(FrameSkipDespawn());
                }
            }
        }

        private IEnumerator FrameSkipDespawn()
        {
            yield return new WaitForEndOfFrame();
            LeanPool.Despawn(gameObject);
        }

        public void CustomFixedUpdate()
        {
            aliveDuration += Time.deltaTime;
            if (aliveDuration >= lifeTime)
            {
                aliveDuration = 0;
                StartCoroutine(FrameSkipDespawn());
            }

            Vector2 moveDirection;
            if (targetDestination != null && targetDestination.IsAlive())
            {
                Vector2 direction = (Vector2)targetDestination.transform.position - rigidbodyRef.position;
                direction.Normalize();
                moveDirection = direction;

                var dir = targetDestination.transform.position - transform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                float rotateAmount = Vector3.Cross(direction, transform.right).z;
                rigidbodyRef.angularVelocity = -rotateAmount * turnSpeed;
            }
            else
            {
                targetDestination = null;
                moveDirection = defaultForward;
            }

            rigidbodyRef.MovePosition(rigidbodyRef.position + moveDirection * movementSpeed * Time.deltaTime);
        }

        public void CustomUpdate() { }

        public void OnDespawn()
        {
            isAlive = false;
        }

        public void OnSpawn()
        {
            AudioManager.Instance.PlaySFX(effectClip);
            isAlive = true;
            aliveDuration = 0;
        }

        public bool CanUpdate()
        {
            return false;
        }

        public bool CanFixedUpdate()
        {
            return isAlive && aliveDuration < lifeTime;
        }

        public void SetAdditionalDamage(float additionalDamage)
        {
            additionalDamage = (int)additionalDamage;
        }
    }
}