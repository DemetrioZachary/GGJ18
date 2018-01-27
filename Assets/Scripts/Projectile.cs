using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed = 3;

    private GameManager.Types type;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        transform.Translate(speed * Vector3.right * Time.deltaTime);
    }

    public GameManager.Types GetProjectileType() { return type; }

    public void Initialize(GameManager.Types type) {
        
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

    private void OnTriggerEnter2D(Collider2D coll) {
        if (coll.CompareTag("Player")) {
            coll.gameObject.GetComponent<PlayerController>().HandleHit(type);
        }
        else if (coll.gameObject.CompareTag("Bomb")) {
            coll.gameObject.GetComponent<Bomb>().TriggerBomb(type);
        }
    }
}
