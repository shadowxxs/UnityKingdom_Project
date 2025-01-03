using System.Collections.Generic;
using UnityEngine;
using static PocketActive;

public class GeneratorActive : MonoBehaviour {
    public GameObject EnemySeed;
    public List<Transform> EnemiesCollection;
    public float ActionTime;
    public float Lifetime;
    public int EnemyStock;
    public bool IsUnlimited;

    public System.Action<Transform> ExtraAction { get; set; }

    private void SpawnEnemy() {
        if (Lifetime <= 0) {
            if (!IsUnlimited && EnemiesCollection.Count == 0) {
                Destroy(gameObject, 2f);
            }
        } else {
            if (EnemiesCollection.Count < EnemyStock && ActionTime == 0) {
                var enemy = Instantiate(EnemySeed, RandomPosition(), Quaternion.identity, transform).transform;
                var enemyType = enemy.GetComponent<EnemyActive>();
                if (enemyType is CreepGuardActive guard) {
                    guard.BasePoint = gameObject;
                }
                var stampede = TargetStampede();
                if (stampede) {
                    enemyType.HasStampede = true;
                    enemyType.Provoked = stampede;
                }
                enemyType.OnEliminate = () => EnemiesCollection.Remove(enemy);
                EnemiesCollection.Add(enemy);
                ExtraAction?.Invoke(enemy);
                ActionTime = 3f;
                Lifetime--;
            }
        }
    }

    private GameObject TargetStampede() {
        try {
            var target = GameObject.FindWithTag("Target");
            return target;
        } catch (System.Exception) {
            return null;
        }
    }

    private Vector3 RandomPosition() {
        var x = Random.Range(-2f, 2f);
        var y = Random.Range(-2f, 2f);
        return transform.position + new Vector3(x, y, 0);
    }

    private void Awake() {
        EnemiesCollection = new();
        //Lifetime = EnemyStock * 2;
    }

    private void Update() {
        if (Lifetime < EnemyStock * 2f && EnemiesCollection.Count == 0 && IsUnlimited) {
            Lifetime += Time.deltaTime;
        } else {
            if (ActionTime > 0) {
                ActionTime -= Time.deltaTime;
            }

            if (ActionTime < 0) {
                ActionTime = 0;
            }
            Lifetime = Mathf.RoundToInt(Lifetime);
            SpawnEnemy();
        }
    }
}
