using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud_Active : MonoBehaviour {
    public GameObject Objective;
    public float Delay;

    public static Hud_Active Instance { get; private set; } 

    public void ShowObjective(bool state) {
        Objective.SetActive(state);
        if (state) {
            Delay = 3f;
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
    }
}
