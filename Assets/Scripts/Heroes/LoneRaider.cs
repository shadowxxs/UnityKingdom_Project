using System.Collections;
using UnityEngine;
public class LoneRaider : PlayerActive {
    public bool IsWalk;
    public bool IsDash;
    public bool mustDash;

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
            if (IsDash) {
                Dash();
            }
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
                if (dist > 7f) {
                    mustDash = true;
                }
                MoveUpdate = EnemyTarget.position - transform.position;
                MoveUpdate.Normalize();
                Anim.SetFloat("AxisX", MoveUpdate.x);
                Anim.SetFloat("AxisY", MoveUpdate.y);
                if (dist > 1.7f) {
                    Speed = 5f;
                    if (dist < 7f && mustDash) {
                        Speed = 5f * 3f;
                    }
                    Anim.SetBool("IsMoving", true);
                    return;
                }
            }
        }
        Speed = 0f;
        IsAttack = mustDash = false;
        Anim.SetBool("IsMoving", false);
        Anim.SetTrigger("Attack");
        var cast = Physics2D.CircleCast(transform.position, 3f, MoveUpdate, 1.7f, LayerMask.GetMask("Enemy"));
        if (cast.collider != null && cast.collider.gameObject) {
            var enemy = cast.collider.GetComponent<EnemyActive>();
            if (enemy.CurrentHp > 0) {
                enemy.TakeDamage = (transform, 1f);
                DamageActive.InstanceDamage(DamagePop, enemy.transform.position, 1f, DamageState.PlayerPhs);
            }
        }
        ActionTime[0] = 0.5f;
    }

    private void Move() {
        Speed = IsWalk ? 5f / 2f : 5f;
        if (!IsWalk) {
            Anim.SetFloat("AxisX", MoveUpdate.x);
            Anim.SetFloat("AxisY", MoveUpdate.y);
        }
        Anim.SetBool("IsMoving", true);
    }

    private void Dash() {
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine() {
        Speed = 5f * 3f;
        yield return new WaitForSeconds(0.15f);
        IsDash = false;
        ActionTime[1] = 2f;
    }
}
