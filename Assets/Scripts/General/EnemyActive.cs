using System;
using System.Collections;
using UnityEngine;

public class EnemyActive : MonoBehaviour, IUnit {
    public Rigidbody2D Body;
    public Animator Anim;
    public Vector3 MoveUpdate;
    public Vector3 MovePosition;
    public GameObject Provoked;
    public GameObject ActiveTarget;
    public bool HasStampede;

    public struct LootAttribute {
        public GameObject Item;
        public int Chance;
    }

    [Header("Unit Data")]
    public float CurrentHp;
    public float MaximumHp;
    public float Speed;
    public float ActionTime;

    [Header("Unit HUD")]
    public Transform HPBar;

    [Header("Unit Loot")]
    public LootAttribute ItemDrop;

    public Action OnEliminate { get; set; }

    public virtual (Transform, float) TakeDamage {
        set {
            CurrentHp = Mathf.Clamp(CurrentHp - value.Item2, 0f, MaximumHp);
            StartCoroutine(KnockbackCoroutine(value.Item1.position, 0.5f));
        }
    }

    protected Vector3 RandomPosition() {
        var x = UnityEngine.Random.Range(-2f, 2f);
        var y = UnityEngine.Random.Range(-2f, 2f);
        return transform.position + new Vector3(x, y, 0);
    }

    protected virtual GameObject TargetLock { get; set; }

    protected virtual string CollisionName { get; set; }

    protected virtual void Idle() {
        Body.velocity = MoveUpdate = Vector3.zero;
        Attack();
    }

    protected virtual void Movement() {
        Anim.SetFloat("AxisX", MovePosition.x);
        Anim.SetFloat("AxisY", MovePosition.y);
        var dist = Vector3.Distance(transform.position, TargetLock.transform.position);
        if (dist < 1f || CollisionName == TargetLock.name) {
            Idle();
        } else {
            var velocity = (TargetLock.transform.position - transform.position).normalized * Speed;
            Body.velocity = velocity;
            MoveUpdate = MovePosition = velocity;
        }
        Anim.SetBool("IsMove", MoveUpdate != Vector3.zero && CurrentHp > 0);
    }

    protected virtual void Attack() {
        var dist = Vector3.Distance(ActiveTarget.transform.position, transform.position) <= 1f;
        var check = dist || CollisionName != string.Empty;
        if (!((ActionTime == 0 ^ check) || Anim.GetBool("IsDeath"))) {
            Anim.SetTrigger("Attack");
            var cast = Physics2D.CircleCast(transform.position, 3f, MoveUpdate, 1.7f, LayerMask.GetMask("Player"));
            if (cast.collider != null && cast.collider.gameObject) {
                var player = cast.collider.GetComponent<IUnit>();
                player.TakeDamage = (transform, 1f);
            }
            ActionTime = 3f;
        }
    }

    protected virtual void Death() {
        Anim.SetBool("IsDeath", CurrentHp <= 0);
        if (Anim.GetBool("IsDeath")) {
            Body.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject, 2f);   
        }
    }

    protected virtual void Initialize() {
        Body = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    protected IEnumerator KnockbackCoroutine(Vector3 pos, float range) {
        yield return new WaitForSeconds(0.5f);
        transform.position += (transform.position - pos).normalized * range;
    }

    private void MonitorHUD() {
        var percent = CurrentHp / MaximumHp;
        HPBar.localScale = new Vector3(percent, 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Enemy")) {
            CollisionName = collision.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Enemy")) {
            CollisionName = string.Empty;
        }
    }

    private void Start() {
        Initialize();
    }

    private void Update() {
        MonitorHUD();
        if (ActionTime > 0) {
            ActionTime -= Time.deltaTime;
        }

        if (ActionTime < 0) {
            ActionTime = 0;
        }
        Movement();
        Death();
    }
}
