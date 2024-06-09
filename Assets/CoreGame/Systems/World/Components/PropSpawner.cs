using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace ProjTha
{
    public class PropSpawner : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private List<Transform> propSpawnLocation;

        [SerializeField]
        private List<GameObject> propPrefabs;

        public void SpawnProps()
        {
            foreach (Transform t in propSpawnLocation)
            {
                int rand = Random.Range(0, propPrefabs.Count);
                LeanPool.Spawn(propPrefabs[rand], t);
            }
        }

        public void OnSpawn()
        {
            SpawnProps();
        }

        public void OnDespawn()
        {
        }
    }
}