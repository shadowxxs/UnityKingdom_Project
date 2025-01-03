using UnityEngine;
using TMPro;

public class DamageActive : MonoBehaviour {
    public static DamageActive InstanceDamage(Transform popup, Vector3 position, float amount, DamageState state) {
        var damage = Instantiate(popup, position, Quaternion.identity);
        var damagepop = damage.GetComponent<DamageActive>();
        damagepop.Setup((int)amount, state);
        return damagepop;
    }

    public void Setup(int damage, DamageState state) {
        TxtPopup.SetText(damage.ToString());
        timer = 1f;
        switch (state) {
            case DamageState.PlayerPhs:
                textcolor = Color.red;
                movevector = new Vector3(Random.Range(0.5f, 1.5f), 1) * Random.Range(4f, 6f);
                break;
            case DamageState.PlayerMag:
                textcolor = Color.blue;
                movevector = new Vector3(Random.Range(0.5f, 1.5f), 1) * Random.Range(4f, 6f);
                break;
            case DamageState.EnemyPhs:
                textcolor = Color.yellow;
                movevector = new Vector3(Random.Range(-0.5f, -1.5f), 1) * Random.Range(4f, 6f);
                break;
            case DamageState.AllyHeal:
                textcolor = Color.green;
                movevector = new Vector3(-1, -1) * Random.Range(4f, 6f);
                break;
            case DamageState.AllyRest:
                textcolor = Color.cyan;
                movevector = new Vector3(-1, -1) * 6f; // arah popup + 6f sebagai pengatur jarak popup
                break;
        }
        TxtPopup.color = textcolor;
    }

    private TextMeshPro TxtPopup;
    private Vector3 movevector;
    private Color textcolor;
    private float timer;

    private void Awake() {
        TxtPopup = transform.GetComponent<TextMeshPro>();
    }

    private void Update() {
        // kontrol gerak lengkung popup
        transform.position += movevector * Time.deltaTime;
        movevector -= movevector * 4f * Time.deltaTime; // 4f kontrol jarak turun lemparan popup

        if (timer > 0.5f) {  // kontrol perbesaran popup
            transform.localScale += Vector3.one * Time.deltaTime;
        } else {
            transform.localScale -= Vector3.one * Time.deltaTime;
        }

        timer -= Time.deltaTime;
        if (timer < 0) {
            float timerspeed = 5f;
            textcolor.a -= timerspeed * Time.deltaTime;
            TxtPopup.color = textcolor;
            if (textcolor.a < 0) {
                Destroy(gameObject);
            }
        }
    }
}

public enum DamageState {
    PlayerPhs, PlayerMag, 
    EnemyPhs,
    AllyHeal, AllyRest,
}
