using System;
using UnityEngine;

public class CreepScoutActive : EnemyActive {
    public Transform[] BasePatroll;
    public GameObject Projectile;
    public int PosIndex;

    protected override void Idle() {
        Body.velocity = MoveUpdate = Vector3.zero;
        MovePosition = (TargetLock.transform.position - transform.position).normalized;
        if (ActionTime == 0 && !Anim.GetBool("IsDeath")) {
            Attack();
        }
    }

    protected override void Movement() {
        Anim.SetFloat("AxisX", MovePosition.x);
        Anim.SetFloat("AxisY", MovePosition.y);
        var dist = Vector3.Distance(transform.position, BasePatroll[PosIndex].transform.position);
        if (dist > 1.7f) {
            var subdist = Vector3.Distance(transform.position, TargetLock.transform.position);
            if (subdist <= 10f) {
                Idle();
            } else {
                var velocity = (BasePatroll[PosIndex].transform.position - transform.position).normalized * Speed;
                Body.velocity = velocity;
                MoveUpdate = MovePosition = velocity;
            }
        } else {
            PosIndex = (PosIndex++) % BasePatroll.Length;
        }
        Anim.SetBool("IsMove", MoveUpdate != Vector3.zero);
    }

    protected override void Attack() {
        var pos = (transform.position + MovePosition - transform.position).normalized;
        var zValue = Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
        var rotate = Quaternion.Euler(0f, 0f, zValue);
        Instantiate(Projectile, transform.position, rotate);
        ActionTime = 1f;
    }
}
