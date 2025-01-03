using System;
using UnityEngine;

public class PlantActive : MonoBehaviour {
    [Serializable]
    public struct Loot {
        public GameObject ItemDrop;
        public float Chance;
    }

    public float Cooldown;
    public float Duration;
    public float Range;
    public Loot[] LootItems;

    public int Filter;
    public int Counter;
    public float Divide;
    public float LootCast;

    public bool IsReady;
    public bool IsTrigger;

    public bool IsGrowth;           // <-- khusus yang ada mesh berkembang
    public Transform[] GrowthMesh;  // <-- khusus yang ada mesh berkembang
    public int GrowthLife;          // <-- khusus yang ada mesh berkembang

    public InputActive Input { get; set; }

    private Vector3 RandomPosition() {
        var n = UnityEngine.Random.Range(-1f, 1f);

        if (n > -0.7f && n < 0.7f) {
            n = 1;
        }

        var x = UnityEngine.Random.Range(0.7f, Range) * n;
        var y = UnityEngine.Random.Range(0.7f, Range) * n;
        return new Vector3(x, y);
    }

    private void Reset2DSprite() {   // << khusus game 2D
        foreach (var mesh in GrowthMesh) {
            mesh.gameObject.SetActive(false);
        }
    }

    private void ItemDrop() {
        for (int i = 0; i < LootItems.Length; i++) {
            if (LootItems[i].Chance < 100) {
                if (UnityEngine.Random.Range(1, 10) <= 5) {
                    var item = Instantiate(LootItems[i].ItemDrop, transform);
                    item.transform.localPosition += RandomPosition();
                }
            } else {
                var item = Instantiate(LootItems[i].ItemDrop, transform);
                item.transform.localPosition += RandomPosition();
            }
        }
        Reset2DSprite();
        IsReady = false;
    }

    private void InitGrowth() {
        if (IsGrowth && GrowthMesh.Length > 0) {
            Divide = 100 / GrowthMesh.Length;
            Counter = Filter = 0;
        }
        Cooldown = Duration;
    }

    private void ItemCheck() {
        if (transform.childCount == GrowthMesh.Length && Cooldown == 0 && !IsReady) {
            InitGrowth();
        }

        if (IsGrowth && Cooldown == 0 && Counter < GrowthMesh.Length) {
            GrowthMesh[Counter].gameObject.SetActive(true);
            Counter++;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player") && IsTrigger && IsReady && Cooldown == 0) {
            Input = other.GetComponent<InputActive>();
            Input.OnInteract = () => {
                if (LootCast < GrowthLife && IsReady) {
                    LootCast++;
                }
                if (LootCast >= GrowthLife && IsReady) {
                    ItemDrop();
                    GrowthMesh[0].gameObject.SetActive(true);
                    LootCast = 0;
                }
            };
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Input.OnInteract = () => {
            LootCast = 0;
        };
    }

    private void Start() {
        if (IsGrowth && GrowthMesh.Length > 0) {
            GrowthMesh[0].gameObject.SetActive(true);
        }
        InitGrowth();
    }

    private void Update() {
        if (Cooldown > 0) {
            Cooldown -= Time.deltaTime;
            if (IsGrowth && GrowthMesh.Length > 0) {
                var n = (int)(Cooldown / Duration * 100);
                if (Filter != n && n < 97 && (n % Divide) == 0) {
                    Reset2DSprite();
                    GrowthMesh[Counter].gameObject.SetActive(true);
                    Counter++;
                }
                Filter = n;
                if (Counter < GrowthMesh.Length) {
                    return;
                }
                IsReady = true;
            }
        }

        if (Cooldown < 0) {
            IsReady = true;
            if (!IsTrigger) {
                ItemDrop();
            }
            Cooldown = 0;
        }
        ItemCheck();

        //if (Cooldown > 0) {
        //    Cooldown -= Time.deltaTime;
        //    if (IsGrowth && GrowthMesh.Length > 0) {
        //        var n = (1f - (Cooldown / Duration)).ToString("0.##");
        //        var nFloat = Convert.ToSingle(n);
        //        var div = (1f / GrowthMesh.Length).ToString("0.##");
        //        //Debug.Log(nFloat + " % " + Convert.ToSingle(div) + " == " + (nFloat % Convert.ToSingle(div) == 0));

        //        if (nFloat > 0 && nFloat % Convert.ToSingle(div) == 0) {
        //            Debug.Log(nFloat + " == " + Convert.ToSingle(div));
        //            Reset2DSprite();
        //            GrowthMesh[++MeshIndex].gameObject.SetActive(true);
        //        } else if (nFloat >= 1) {
        //            Debug.Log(nFloat);
        //            Reset2DSprite();
        //            GrowthMesh[^1].gameObject.SetActive(true);
        //            MeshIndex = 0;
        //        }
        //        IsReady = true;
        //    }
        //}

        //if (Cooldown < 0) {
        //    if (!IsTrigger) {
        //        IsReady = true;
        //        ItemDrop(ActionState.Attack);
        //    }
        //    Cooldown = 0;
        //}
        //ItemCheck();
    }
}

