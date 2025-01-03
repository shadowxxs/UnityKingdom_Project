using System;
using UnityEngine;

public class ProjectileActive : MonoBehaviour {
    public Transform DamagePop;
    public Transform Owner;
    public Vector3 MoveUpdate;
    public Rigidbody2D Body;
    public float Penetration;
    public float Speed;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            var enemy = collision.GetComponent<EnemyActive>();
            if (enemy.CurrentHp > 0) {
                enemy.TakeDamage = (transform, 1f);
                DamageActive.InstanceDamage(DamagePop, enemy.transform.position, 1f, DamageState.PlayerPhs);
            }
        }
    }

    private void Start() {
        Body = GetComponent<Rigidbody2D>(); 
    }

    private void Update() {
        Body.velocity = MoveUpdate * Speed;
        Penetration -= Time.deltaTime;
        if (Penetration < 0) {
            Destroy(gameObject);
        }
    }
}
