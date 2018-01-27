using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour {
    public int player;
    public int teamMatePlayer;
    public float speed = 1;
    public float railLength = 10;
    public float angle = 0;
    public Projectile projectilePrefab;

    private GameManager.Types shield = GameManager.Types.Green;
    private string inputStr = "";
    private SpriteRenderer spriteRenderer;

    PlayerIndex playerIndex;
    GamePadState[] state = new GamePadState[2];
    GamePadState[] prevState = new GamePadState[2];

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.green;
        if (player == 2) { speed *= -1; }
        inputStr = "P" + player;
    }

    void Update() {

        prevState[0] = state[0];
        state[0] = GamePad.GetState((PlayerIndex)player);

        prevState[1] = state[1];
        state[1] = GamePad.GetState((PlayerIndex)teamMatePlayer);

        Move();
        SetShield();
        Fire();
    }

    void FixedUpdate()
    {
        // SetVibration should be sent in a slower rate.
        // Set vibration according to triggers
        GamePad.SetVibration((PlayerIndex)teamMatePlayer, state[0].Triggers.Left, state[0].Triggers.Right);
    }

    private void Move() {
        float yPos = Mathf.Sin(angle) * railLength / 2f;
        transform.position = new Vector3(transform.position.x, yPos, 0);
        angle += Time.deltaTime * speed;
        if (Mathf.Abs(angle) >= Mathf.PI * 2) { angle = 0; }
    }

    private void SetShield() {
        shield = GameManager.Types.None;
        //if (prevState[1].DPad.Down == ButtonState.Released && state[1].DPad.Down == ButtonState.Pressed) { shield = GameManager.Types.Green; }
        //else if(prevState[1].DPad.Right == ButtonState.Released && state[1].DPad.Right == ButtonState.Pressed) { shield = GameManager.Types.Red; }
        //else if (prevState[1].DPad.Left == ButtonState.Released && state[1].DPad.Left == ButtonState.Pressed) { shield = GameManager.Types.Blue; }
        //else if (prevState[1].DPad.Up == ButtonState.Released && state[1].DPad.Up == ButtonState.Pressed) { shield = GameManager.Types.Yellow; }

        if (prevState[1].Buttons.A == ButtonState.Released && state[1].Buttons.A == ButtonState.Pressed) { shield = GameManager.Types.Green; }
        else if (prevState[1].Buttons.B == ButtonState.Released && state[1].Buttons.B == ButtonState.Pressed) { shield = GameManager.Types.Red; }
        else if (prevState[1].Buttons.X == ButtonState.Released && state[1].Buttons.X == ButtonState.Pressed) { shield = GameManager.Types.Blue; }
        else if (prevState[1].Buttons.Y == ButtonState.Released && state[1].Buttons.Y == ButtonState.Pressed) { shield = GameManager.Types.Yellow; }

        switch (shield) {
            case GameManager.Types.Green:
                spriteRenderer.color = Color.green;
                break;
            case GameManager.Types.Red:
                spriteRenderer.color = Color.red;
                break;
            case GameManager.Types.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case GameManager.Types.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
            //default:
            //    spriteRenderer.color = Color.white;
            //    break;

        }
    }

    private void Fire() {
        GameManager.Types type;
        if (prevState[0].Buttons.A == ButtonState.Released && state[0].Buttons.A == ButtonState.Pressed) { type = GameManager.Types.Green; }
        else if (prevState[0].Buttons.B == ButtonState.Released && state[0].Buttons.B == ButtonState.Pressed) { type = GameManager.Types.Red; }
        else if (prevState[0].Buttons.X == ButtonState.Released && state[0].Buttons.X == ButtonState.Pressed) { type = GameManager.Types.Blue; }
        else if (prevState[0].Buttons.Y == ButtonState.Released && state[0].Buttons.Y == ButtonState.Pressed) { type = GameManager.Types.Yellow; }
        else { return; }
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity) as Projectile;
        projectile.Initialize(player == 1 ? 1 : -1, type);
    }
}
