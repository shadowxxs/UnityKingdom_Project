using System;
using Unity.VisualScripting;
using UnityEngine;

public class PocketActive : MonoBehaviour {
    [Serializable]
    public struct ItemInfo {
        public string Name;
        public GameObject Point;
        public int Qty;
    }

    public GameObject HeadDisplayGet;
    public GameObject HeadDisplaySet;
    public ItemInfo[] Pockets;
    public bool IsFull;

    public static ItemInfo ItemTmp;

    public ItemInfo Itemhand { 
        get {
            return ItemTmp;
        }
        set {
            if (HeadDisplayGet.activeInHierarchy == true) {
                HeadDisplayGet.SetActive(false);
            }
            ItemTmp = value;
            for (int i = 0; i < Pockets.Length; i++) {
                if (value.Name == Pockets[i].Name) {
                    PocketUpdate(i);
                    return;
                } 
            }
            for (int i = 0; i < Pockets.Length; i++) {
                if (Pockets[i].Point == null) {
                    IsFull = true;
                    break;
                } 
            }
            if (!IsFull) {
                return;
            }
            HeadDisplayGet.SetActive(true);
            InputActive.Instance.OnPocket = PocketInsert;
        }
    }

    public Action<(int, GameObject)> AddToGUIAction { get; set; }
    public Action<(int, GameObject)> EditToGUIAction { get; set; }
    public Action<(int, GameObject)> SubToGUIAction { get; set; }

    public Action PocketHandler {
        get {
            return () => {
                HeadDisplaySet.SetActive(!HeadDisplaySet.activeInHierarchy);
                InputActive.Instance.OnPocket = HeadDisplaySet.activeInHierarchy ? PocketRemove : null;
            };
        }
    }

    public delegate bool ExtraAction(int n);

    public Action Callback { get; set; }

    public event ExtraAction ExtraEvent;

    private void PocketInsert(ActionState state) {
        var i = (int)state;
        if (Pockets[i].Point == null) {
            Pockets[i].Name = ItemTmp.Name;
            Pockets[i].Point = ItemTmp.Point;
            Pockets[i].Qty = Mathf.Clamp(ItemTmp.Qty, 0, 9);

            AddToGUIAction?.Invoke((i, Pockets[i].Point));
            ExtraEvent?.Invoke(i);
            HeadDisplayGet.SetActive(IsFull = false);
            Callback?.Invoke();
        }
    }

    private void PocketUpdate(int i) {
        Pockets[i].Qty += Mathf.Clamp(ItemTmp.Qty, 0, 9);
        EditToGUIAction?.Invoke((i, Pockets[i].Point));
        ExtraEvent?.Invoke(i);

        if (Pockets[i].Qty == 0) {
            Pockets[i].Name = null;
            Pockets[i].Point = null;
        }
        Callback?.Invoke();
    }

    private void PocketRemove(ActionState state) {
        var i = (int)state;
        if (Pockets[i].Qty > 0) {
            if (ExtraEvent?.Invoke(i) == true) {
                Pockets[i].Qty -= Mathf.Clamp(1, 0, 9);
                if (Pockets[i].Qty == 0) {
                    HeadDisplaySet.SetActive(false);
                    Pockets[i].Name = null;
                    Pockets[i].Point = null;
                }
                SubToGUIAction?.Invoke((i, Pockets[i].Point));
            }
        }
    }

    private void Start() {
        Pockets = new ItemInfo[3];
    }
}
