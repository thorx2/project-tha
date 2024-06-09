using System.Collections.Generic;
using SuperMaxim.Messaging;
using UnityEngine;

namespace ProjTha
{
    public class MonoHookGroupUpdater : MonoBehaviour
    {
        private HashSet<IMonoUpdateHook> monoUpdateHooks;

        protected void Awake()
        {
            monoUpdateHooks ??= new HashSet<IMonoUpdateHook>(50);
            Messenger.Default.Subscribe<MonoTransport>(OnNewIMonoSpawned);
        }

        protected void OnDestroy()
        {
            Messenger.Default.Unsubscribe<MonoTransport>(OnNewIMonoSpawned);
        }


        private void OnNewIMonoSpawned(MonoTransport transport)
        {
            if (transport.IsSpawned)
            {
                monoUpdateHooks.Add(transport.MonoSpawned);
            }
            else
            {
                monoUpdateHooks.Remove(transport.MonoSpawned);
            }
        }

        protected void Update()
        {
            if (!GameManager.PauseElementMovement)
            {
                foreach (var mono in monoUpdateHooks)
                {
                    if (mono.CanUpdate())
                    {
                        mono.CustomUpdate();
                    }
                }
            }
        }

        protected void FixedUpdate()
        {
            if (!GameManager.PauseElementMovement)
            {
                foreach (var mono in monoUpdateHooks)
                {
                    if (mono.CanFixedUpdate())
                    {
                        mono.CustomFixedUpdate();
                    }
                }
            }
        }
    }
}