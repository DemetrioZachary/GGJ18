using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;


public class MenuNavigator : MonoBehaviour {

    private Selectable currButton;
    GamePadState state;

	void Start () {

    }
	
	void Update () {
        float xValue = state.ThumbSticks.Left.X, yValue = state.ThumbSticks.Left.Y;
        if (Mathf.Abs(yValue) > 0.01 || Mathf.Abs(xValue) > 0.01) {
            if (Mathf.Abs(yValue) > Mathf.Abs(xValue)) {
                if (yValue > 0) { currButton = currButton.FindSelectableOnUp(); }
                else { currButton = currButton.FindSelectableOnDown(); }
            }
            else {
                if (xValue > 0) { currButton = currButton.FindSelectableOnRight(); }
                else { currButton = currButton.FindSelectableOnLeft(); }
            }
            currButton.Select();
        }
    }

    private void OnEnable() {
        state = GamePad.GetState(PlayerIndex.One);
        Button[] buttons = GetComponentsInChildren<Button>();
        if (buttons.Length > 0) {
            foreach(Button button in buttons) {
                if (button.IsInteractable()) {
                    button.Select();
                    currButton = button;
                }
            }
        }
    }
}
