using System;
using UnityEngine;

public class InputActive : MonoBehaviour {
    public static InputActive Instance { get; private set;}
    public Action<Vector3> OnMovement { get; set; }
    public Action<Vector3> OnLook { get; set; }
    public Action<ActionState> OnAction { get; set; }
    public Action<ActionState> OnPocket { get; set; }
    public Action OnInteract { get; set; }

    private GameControllers input;

    private void Start() {
        Instance = this;
        input = new();
        input.Enable();

        input.GamePlay.Interaction.performed += (e) => {
            OnInteract?.Invoke();
        };

        input.GamePlay.Attack.performed += (e) => {
            OnAction?.Invoke(ActionState.Attack);
        };

        input.GamePlay.Pocket_1.performed += (e) => {
            OnPocket?.Invoke(ActionState.Pocket1);
        };

        input.GamePlay.Pocket_2.performed += (e) => {
            OnPocket?.Invoke(ActionState.Pocket2);
        };

        input.GamePlay.Pocket_3.performed += (e) => {
            OnPocket?.Invoke(ActionState.Pocket3);
        };

        //input.GamePlay.Dash.performed += (e) => {
        //    OnAction?.Invoke(ActionState.Dash);
        //};

        //input.GamePlay.Walk.performed += (e) => {
        //    OnAction?.Invoke(ActionState.Walk);
        //};

        input.GamePlay.Target.performed += (e) => {
            OnAction?.Invoke(ActionState.Target);
        };

        input.GamePlay.Quest.performed += (e) => {
            OnAction?.Invoke(ActionState.Objective);
        };
    }

    private void Update() {
        var axisX = input.GamePlay.Movement.ReadValue<Vector2>().x;
        var axisY = input.GamePlay.Movement.ReadValue<Vector2>().y;
        OnMovement?.Invoke(new Vector3(axisX, axisY));
    }
}

public enum ActionState {
    Pocket1, Pocket2, Pocket3,
    Attack, Target, Death, Walk, Dash,
    Objective,
}