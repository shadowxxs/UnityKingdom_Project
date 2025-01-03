using UnityEngine;

public class BaseActive : MonoBehaviour, IUnit {
    [Header("Character Data")]
    public float CurrentHp;
    public float MaximumHp;

    public (Transform, float) TakeDamage {
        set {
            CurrentHp = Mathf.Clamp(CurrentHp - value.Item2, 0f, MaximumHp);
        }
    }
    private void Start() {
        
    }
}
