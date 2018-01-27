using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int player;
    public float speed = 1;
    public float railLength = 10;
    public float angle = 0;
    public Projectile projectilePrefab;

    private GameManager.Types shield = GameManager.Types.Green;
    private string inputStr = "";
    private SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.green;
        if (player == 2) { speed *= -1; }
        inputStr = "P" + player;
    }

    void Update() {
        Move();
        SetShield();
        Fire();
    }

    private void Move() {
        float yPos = Mathf.Sin(angle) * railLength / 2f;
        transform.position = new Vector3(transform.position.x, yPos, 0);
        angle += Time.deltaTime * speed;
        if (Mathf.Abs(angle) >= Mathf.PI * 2) { angle = 0; }
    }

    private void SetShield() {
        GameManager.Types type;
        if (Input.GetButtonDown(inputStr + "ShieldGreen")) { type = GameManager.Types.Green; }
        else if (Input.GetButtonDown(inputStr + "ShieldRed")) { type = GameManager.Types.Red; }
        else if (Input.GetButtonDown(inputStr + "ShieldBlue")) { type = GameManager.Types.Blue; }
        else if (Input.GetButtonDown(inputStr + "ShieldYellow")) { type = GameManager.Types.Yellow; }
        else { return; }
        shield = type;
        switch (type) {
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
        }
    }

    private void Fire() {
        GameManager.Types type;
        if (Input.GetButtonDown(inputStr + "FireGreen")) { type = GameManager.Types.Green; }
        else if (Input.GetButtonDown(inputStr + "FireRed")) { type = GameManager.Types.Red; }
        else if (Input.GetButtonDown(inputStr + "FireBlue")) { type = GameManager.Types.Blue; }
        else if (Input.GetButtonDown(inputStr + "FireYellow")) { type = GameManager.Types.Yellow; }
        else { return; }
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity) as Projectile;
        projectile.Initialize(player == 1 ? 1 : -1, type);
    }
}
