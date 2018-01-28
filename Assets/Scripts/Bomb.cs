using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float speed = 7;
    public GameObject bombBase, bombBody;

    private bool triggered = false;
    private Vector3 velocity = Vector3.zero;

    private GameManager.Types type = GameManager.Types.Green;

    void Start() {

    }

    void Update() {

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
