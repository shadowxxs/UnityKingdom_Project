using UnityEngine;

public class ItemActive : MonoBehaviour {
    public bool IsPowerUp;
    public GameObject Player;

    [Header("Item Data")]
    public string Name;
    public string Description;
    public string Icon;

    public PocketActive PocketPlayer { get; set; }

    private void PickItem() {
        PocketPlayer = Player.GetComponent<PocketActive>();
        PocketPlayer.Callback = () => {
            Destroy(gameObject);
        };
        PocketPlayer.Itemhand = new() { Name = Name, Point = Resources.Load<GameObject>("ItemWorld/" + Icon), Qty = 1 };
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player = collision.gameObject;
            InputActive.Instance.OnInteract += PickItem;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player = collision.gameObject;
            PocketPlayer = Player.GetComponent<PocketActive>();
            if (PocketPlayer.HeadDisplayGet.activeInHierarchy) {
                PocketPlayer.HeadDisplayGet.SetActive(false);
            }
        }
        InputActive.Instance.OnInteract -= PickItem;
        InputActive.Instance.OnPocket = null;
    }

    private void OnDestroy() {
        try {
            InputActive.Instance.OnInteract -= PickItem;
            PocketPlayer.Itemhand = new();
            PocketPlayer.Callback = null;
        } catch { } 
    }

}
