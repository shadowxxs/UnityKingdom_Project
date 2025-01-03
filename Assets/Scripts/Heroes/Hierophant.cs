using System;
using System.Collections;
using UnityEngine;

public class Hierophant : PlayerActive {
    public GameObject Projectile;
    protected override void Movement(Vector3 vector) {
        if (IsInteract) {
            Body.constraints = RigidbodyConstraints2D.FreezeAll;
            Idle();
            return;
        }
        Body.constraints = RigidbodyConstraints2D.None;
        Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (vector == Vector3.zero) {
            Idle();
            if (IsAttack) {
                Attack();
            }
        } else {
            Move();
            MoveUpdate = vector;
            IsAttack = false;
        }
        Body.velocity = MoveUpdate * Speed;
    }

    protected override void Attack() {
        TargetClosest();
        if (EnemyTarget) {
            var dist = Vector3.Distance(EnemyTarget.position, transform.position);
            if (dist < 12f) {
                MoveUpdate = EnemyTarget.position - transform.position;
                MoveUpdate.Normalize();
                Anim.SetFloat("AxisX", MoveUpdate.x);
                Anim.SetFloat("AxisY", MoveUpdate.y);
                if (dist > 4.3f) { // <-- Range Point
                    Speed = 5f;
                    Anim.SetBool("IsMoving", true);
                    return;
                }
            }
        }
        Speed = 0f;
        IsAttack = false;
        Anim.SetBool("IsMoving", false);
        Anim.SetTrigger("Attack");

        var pos = (transform.position + MoveUpdate - transform.position).normalized;
        var zValue = Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
        var rotate = Quaternion.Euler(0f, 0f, zValue);
        var bullet = Instantiate(Projectile, transform.position, rotate);
        bullet.GetComponent<ProjectileActive>().Owner = transform;
        bullet.GetComponent<ProjectileActive>().MoveUpdate = MoveUpdate;
        ActionTime[0] = 0.5f;
    }

    private void Move() {
        Speed = 5f;
        Anim.SetFloat("AxisX", MoveUpdate.x);
        Anim.SetFloat("AxisY", MoveUpdate.y);
        Anim.SetBool("IsMoving", true);
    }


}
