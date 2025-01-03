using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public interface IUnit {
    public (Transform, float) TakeDamage { set; }
}

public abstract class PlayerActive : MonoBehaviour, IUnit {
    public Transform DamagePop;
    public Rigidbody2D Body;
    public Animator Anim;
    public Vector3 MoveUpdate;
    public float[] ActionTime;

    [Header("Character Target")]
    public Transform EnemyTarget;
    public List<Transform> EnemiesList;
    public GameObject[] EnemiesAll;
    public float EnemyRange;
    public int EnemyIndex;

    [Header("Character Data")]
    public float CurrentHp;
    public float MaximumHp;
    public float Speed;

    [Header("Character State")]
    public bool IsAttack;
    public bool IsDeath;
    public bool IsInteract;

    public static PlayerActive Instance { get; private set; }

    public (Transform, float) TakeDamage {
        set {
            CurrentHp = Mathf.Clamp(CurrentHp - value.Item2, 0f, MaximumHp);
        //    StartCoroutine(KnockbackCoroutine(value.Item1.position, 0.25f)); 
        }
    }

    private void Action(ActionState State) {
        switch (State) {
            case ActionState.Attack:
                IsAttack = ActionTime[0] == 0;
                break;
            case ActionState.Target:
                TargetSwitch();
                break;
            case ActionState.Death:
                break;
            //case ActionState.Dash:
            //    IsDash = ActionTime[1] == 0;
            //    break;
            //case ActionState.Walk:
            //    if (ActionTime[2] == 0) {
            //        IsWalk = !IsWalk;
            //        ActionTime[2] = 5;
            //    }
            //    break;
            case ActionState.Objective:
                GuiActive.Instance.ShowObjective(true);
                break;
        }
    }

    protected abstract void Movement(Vector3 vector);

    protected abstract void Attack();

    protected void Idle() {
        Speed = 0f;
        Anim.SetBool("IsMoving", false);
    }

    protected void Death() {
        Anim.SetBool("IsDeath", IsDeath = CurrentHp <= 0);
        if (IsDeath) {
            Body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    //protected IEnumerator KnockbackCoroutine(Vector3 pos, float range) {
    //    yield return new WaitForSeconds(0.5f);
    //    var knockPos = (transform.position - pos).normalized * range;
    //    transform.position += knockPos;
    //}

    protected void TargetSwitch() {
        if (EnemiesList.Count > 0) {
            var count = EnemiesList.Count;
            if (count > 0) {
                ++EnemyIndex;
                if (EnemyIndex == count) {
                    EnemyIndex = 0;
                }
                EnemyTarget = EnemiesList[EnemyIndex];
            }
        } else {
            EnemyTarget = null;
            EnemiesList.Clear();
            TargetClosest();
            IsAttack = false;
        }
    }

    protected void TargetClosest() {
        if (!IsDeath) {
            EnemiesAll = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < EnemiesAll.Length; i++) {
                var distance = Vector3.Distance(transform.position, EnemiesAll[i].transform.position);
                if (distance > EnemyRange) {
                    EnemiesAll[i] = null;
                }
            }

            for (int i = 0; i < EnemiesAll.Length; i++) {
                if (EnemiesAll[i] != null && EnemiesList.Count < 4 && !EnemiesList.Contains(EnemiesAll[i].transform)) {
                    EnemiesList.Add(EnemiesAll[i].transform);
                }
            }

            if (EnemyTarget == null && EnemiesList.Count > 0) {
                EnemyTarget = EnemiesList[0];
            }
        }

        if (EnemiesList.Count > 0 && !IsDeath) {
            try {
                foreach (var enemy in EnemiesList) {
                    if (enemy == null) {
                        EnemiesList.Remove(enemy.transform);
                        break;
                    }

                    var distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance > EnemyRange) {
                        EnemiesList.Remove(enemy.transform);
                        EnemyTarget = EnemiesList.Count > 0 ? EnemiesList[0] : null;
                    }
                }
            } catch {
                EnemyTarget = null;
                EnemiesList.Clear();
                TargetClosest();
                IsAttack = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Base")) {
            //InputActive.Instance.OnInteract = 
            return;
        }

        var hitLayer = collision.collider.gameObject.layer;
        var layer = LayerMask.NameToLayer("Enemy");
        if (hitLayer == layer) {
            TakeDamage = (collision.collider.transform, 1f);
        }
    }

    private void Start() {
        Instance = this;
        Body = GetComponent<Rigidbody2D>();
        Anim = transform.Find("Sprite").GetComponent<Animator>();
        ActionTime = new[] { 0f, 0f, 0f };

        EnemiesList = new();
        EnemyRange = 15f;
        EnemyIndex = 0;

        InputActive.Instance.OnMovement = Movement;
        InputActive.Instance.OnAction = Action;
    }

    private void Update() {
        for (int i = 0; i < ActionTime.Length; i++) {
            if (ActionTime[i] > 0) {
                ActionTime[i] -= Time.deltaTime;
            }

            if (ActionTime[i] < 0) {
                ActionTime[i] = 0;
            }
        }
        Death();
    }
}