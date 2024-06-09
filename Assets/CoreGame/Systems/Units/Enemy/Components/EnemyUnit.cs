using ProjTha;
using UnityEngine;

public class EnemyUnit : BaseUnit
{
    private PlayerUnit overlappedUnit;

    private float overLapTime;

    public override void CustomUpdate()
    {
        base.CustomUpdate();

        if (IsAlive())
        {
            if (overlappedUnit != null)
            {
                if (overLapTime <= 0)
                {
                    overLapTime = unitData.DamageRate;
                    overlappedUnit.TakeDamage(unitData.DamagePerTick);
                }
                else
                {
                    overLapTime -= Time.deltaTime;
                }
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out overlappedUnit))
        {
            overLapTime = 0;
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        overlappedUnit = null;
        overLapTime = unitData.DamageRate;
    }
}