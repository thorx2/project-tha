using Lean.Pool;
using SuperMaxim.Messaging;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dropPrefab;

    protected void Start()
    {
        Messenger.Default.Subscribe<OnUnitDeath>(OnUnitDeath);
    }

    protected void OnDestroy()
    {
        Messenger.Default.Unsubscribe<OnUnitDeath>(OnUnitDeath);
    }

    private void OnUnitDeath(OnUnitDeath unitDeath)
    {
        LeanPool.Spawn(dropPrefab, unitDeath.SpawnLocation, Quaternion.identity);
    }
}