using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;


public class MenuNavigator : MonoBehaviour {

    private Selectable currButton;
    GamePadState state, prevState;

	void Start () {

    }

    void Update() {
        prevState = state;
        state = GamePad.GetState(0);
        Selectable newButton;
        if ((Mathf.Abs(prevState.ThumbSticks.Left.Y) < 0.5 && Mathf.Abs(state.ThumbSticks.Left.Y) > 0.5) || (Mathf.Abs(prevState.ThumbSticks.Left.X) < 0.5 && Mathf.Abs(state.ThumbSticks.Left.X) > 0.5)) {
            if (Mathf.Abs(state.ThumbSticks.Left.Y) > Mathf.Abs(state.ThumbSticks.Left.X)) {
                if (state.ThumbSticks.Left.Y > 0) { newButton = currButton.FindSelectableOnUp(); }
                else { newButton = currButton.FindSelectableOnDown(); }
            }
            else {
                if (state.ThumbSticks.Left.X > 0) { newButton = currButton.FindSelectableOnRight(); }
                else { newButton = currButton.FindSelectableOnLeft(); }
            }
            if (newButton) {
                currButton = newButton;
                currButton.Select();
            }
        }
        if(prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed) {
            currButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    private void OnEnable() {
        Button[] buttons = GetComponentsInChildren<Button>();
        if (buttons.Length > 0) {
            foreach(Button button in buttons) {
                if (button.IsInteractable()) {
                    if (!currButton || button.transform.position.y > currButton.transform.position.y) {
                        button.Select();
                        currButton = button;
                    }
                }
            }
        }
    }
}
