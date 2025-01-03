using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static PocketActive;

public class GuiActive : MonoBehaviour {
    public GameObject Objective;
    public TextMeshProUGUI TmpObjective;
    public float Delay;

    public Image ItemHand;

    [Header("Pocket HUD")]
    public Image[] Pockets;
    public TextMeshProUGUI[] TxtItems;

    [Header("Pocket Get")]
    public Image[] AddPocket;

    [Header("Pocket Set")]
    public Image[] SubPocket;
    public TextMeshProUGUI[] TxtRemain;
    public Image[] Request;
    public TextMeshProUGUI[] TxtRequest;

    [Header("HP Bar")]
    public TextMeshProUGUI TxtHp;
    public Slider BarHp;
    public Sprite TmpIcon;

    public static GuiActive Instance { get; private set; }

    public void ShowObjective(bool state) {
        Objective.SetActive(state);
        if (state) {
            Delay = 2f;
        }
    }

    private void InsertItemToPocket(PocketActive pocket) {
        pocket.AddToGUIAction ??= (e) => {
            if (e.Item2) {
                TmpIcon = e.Item2.GetComponent<SpriteRenderer>().sprite;
                Pockets[e.Item1].sprite = AddPocket[e.Item1].sprite = SubPocket[e.Item1].sprite = TmpIcon;
                TxtItems[e.Item1].text = TxtRemain[e.Item1].text = pocket.Pockets[e.Item1].Qty.ToString();
            }
        };
    }

    private void UpdateItemToPocket(PocketActive pocket) {
        pocket.EditToGUIAction ??= (e) => {
            if (e.Item2) {
                TxtItems[e.Item1].text = TxtRemain[e.Item1].text = pocket.Pockets[e.Item1].Qty.ToString();
            }
        };
    }

    private void DeleteItemToPocket(PocketActive pocket) {
        pocket.SubToGUIAction ??= (e) => {
            TxtItems[e.Item1].text = TxtRemain[e.Item1].text = pocket.Pockets[e.Item1].Qty.ToString();
            if (e.Item2 == null) {
                TmpIcon = null;
                Pockets[e.Item1].sprite = SubPocket[e.Item1].sprite = TmpIcon;
                TxtItems[e.Item1].text = TxtRemain[e.Item1].text = string.Empty;
            }
        };
    }

    private void MonitorHUD() {
        var player = PlayerActive.Instance.GetComponent<PlayerActive>();
        var pocket = player.transform.GetComponent<PocketActive>();

        if (pocket.Itemhand.Point) {
            ItemHand.sprite = pocket.Itemhand.Point.GetComponent<SpriteRenderer>().sprite;
        }
        InsertItemToPocket(pocket);
        UpdateItemToPocket(pocket);
        DeleteItemToPocket(pocket);
        if (player.CurrentHp >= 0) {
            BarHp.maxValue = player.MaximumHp;
            BarHp.value = player.CurrentHp;
            TxtHp.text = player.CurrentHp.ToString() + " / " + player.MaximumHp.ToString();
        }
    }

    private void Start() {
        Instance = this;
    }

    private void LateUpdate() {
        if (Delay > 0f) {
            Delay -= Time.deltaTime;
        }

        if (Delay < 0f) {
            ShowObjective(false);
            Delay = 0;
        }
        MonitorHUD();
    }
}
