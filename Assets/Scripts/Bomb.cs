using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float speed = 7;
    public GameObject bombBase;

    private bool triggered = false;
    private Vector3 velocity = Vector3.zero;
    private float angle = 0;
    private Vector3 startPosition;

    private GameManager.Types type = GameManager.Types.Green;

    void Start() {
        startPosition = transform.position;
    }

    void Update() {
        transform.position = new Vector3(startPosition.x + Mathf.Sin(angle) / 2f, transform.position.y, transform.position.z);
        angle += Time.deltaTime;
        if (angle >= 2 * Mathf.PI) { angle = 0; }

        if (triggered) {
            transform.Translate(Vector3.SmoothDamp(Vector3.zero, Vector3.up * speed, ref velocity, 1f));
        }
    }

    private void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.CompareTag("Player")) {
            coll.gameObject.GetComponent<PlayerController>().HandleHit(type);
        }
    }

    public void TriggerBomb(GameManager.Types type) {
        this.type = type;
        // TODO change color
        Destroy(bombBase);
        triggered = true;
    }
}
