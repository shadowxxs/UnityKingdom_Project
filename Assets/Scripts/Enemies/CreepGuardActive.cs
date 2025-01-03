using System;
//using UnityEditor.iOS;
using UnityEngine;

public class CreepGuardActive : EnemyActive {
    public GameObject BasePoint;
    public GameObject Target;
    //public bool IsChange;

    public override (Transform, float) TakeDamage {
        set {
            if (value.Item1.CompareTag("Player")) {
                HasStampede = false;
            }
            CurrentHp = Mathf.Clamp(CurrentHp - value.Item2, 0f, MaximumHp);
            StartCoroutine(KnockbackCoroutine(value.Item1.position, 0.5f));
        }
    }

    protected override GameObject TargetLock {
        get {
            return Target;
        }
        set {
            Target = value;
            if (!HasStampede) {
                var playerState = !PlayerActive.Instance.Anim.GetBool("IsDeath");
                var dist = Vector3.Distance(BasePoint.transform.position, ActiveTarget.transform.position) <= 13f;
                if (playerState && dist) {
                    Target = ActiveTarget;
                } else {
                    if (Provoked != null) {
                        HasStampede = true;
                    }
                }
            } 
        }
    }

    protected override void Movement() {
        if (HasStampede) {
            TargetLock = ActiveTarget = Provoked;
        } else {
            ActiveTarget = PlayerActive.Instance.gameObject;
            TargetLock = BasePoint;
        }
        base.Movement();
    }

    protected override void Attack() {
        var dist = Vector3.Distance(ActiveTarget.transform.position, transform.position) <= 1f;
        var check = dist || CollisionName != string.Empty;
        if (!((ActionTime == 0 ^ check) || Anim.GetBool("IsDeath")) && Target == ActiveTarget) {
            Anim.SetTrigger("Attack");
            var cast = Physics2D.CircleCast(transform.position, 3f, MoveUpdate, 1.7f, LayerMask.GetMask("Player"));
            if (cast.collider != null && cast.collider.gameObject) {
                var player = cast.collider.GetComponent<IUnit>();
                player.TakeDamage = (transform, 1f);
            }
            ActionTime = 3f;
        }
    }

    protected override void Initialize() {
        Body = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        ActiveTarget = HasStampede ? Provoked : PlayerActive.Instance.gameObject;
    }

    private void OnDestroy() {
        var rush = Resources.Load<GameObject>("Obstacles/Enemy_Rush");
        rush.GetComponent<CreepRushActive>().enabled = true;
        Instantiate(rush, transform.position, Quaternion.identity);
        OnEliminate?.Invoke();
    }
}