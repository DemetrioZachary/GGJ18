using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float speed = 7;
    public GameObject bombBase, bombBody;
    public ParticleSystem pulse, explosion;

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

    private IEnumerator Detonate() {
        pulse.Stop();
        explosion.Play();
        Destroy(bombBody.gameObject);
        Destroy(bombBase.gameObject);
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void TriggerBomb(GameManager.Types type) {
        this.type = type;
        ParticleSystem.MainModule main = pulse.main;
        switch (type) {
            case GameManager.Types.Green:
                main.startColor = Color.green;
                break;
            case GameManager.Types.Red:
                main.startColor = Color.red;
                break;
            case GameManager.Types.Blue:
                main.startColor = Color.blue;
                break;
            case GameManager.Types.Yellow:
                main.startColor = Color.yellow;
                break;
        }
        if (!triggered) {
            Destroy(bombBase);
            triggered = true;
            pulse.Play();
        }
    }
}
