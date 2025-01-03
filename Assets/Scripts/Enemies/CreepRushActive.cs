using System;
using UnityEngine;

public class CreepRushActive : EnemyActive {
    public override (Transform, float) TakeDamage {
        set {
            if (value.Item1.CompareTag("Player")) {
                HasStampede = false;
            }
            CurrentHp = Mathf.Clamp(CurrentHp - value.Item2, 0f, MaximumHp);
            StartCoroutine(KnockbackCoroutine(value.Item1.position, 0.5f));
        }
    }

    protected override void Movement() {
        ActiveTarget = HasStampede ? Provoked : PlayerActive.Instance.gameObject;
        TargetLock = ActiveTarget;
        base.Movement();
    }

    private void OnDestroy() {
        var blow = Resources.Load<GameObject>("Effects/FireEnd");
        Destroy(Instantiate(blow, transform.position, Quaternion.identity), 1.3f);

        var loot1 = Resources.Load<GameObject>("ItemWorld/Bone");
        Instantiate(loot1, RandomPosition(), Quaternion.identity);
        OnEliminate?.Invoke();
    }
}
