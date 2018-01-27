using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed = 3;

    private int direction = 1;
    private GameManager.Types type;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        transform.Translate(new Vector3(speed * direction * Time.deltaTime, 0, 0));
    }

    public void Initialize(int direction, GameManager.Types type) {
        this.direction = direction;
        this.type = type;
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
}
