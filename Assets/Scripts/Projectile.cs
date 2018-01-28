using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed = 15;
    public MeshRenderer mesh;
    public ParticleSystem trail, explosion;

    private GameManager.Types type;


    private void Awake() {
        trail.Play();
    }

    private void Update() {
        transform.Translate(speed * Vector3.right * Time.deltaTime);
    }

    public GameManager.Types GetProjectileType() { return type; }

    public void Initialize(GameManager.Types type) {
        
        this.type = type;
        Color color = Color.white;
        switch (type) {
            case GameManager.Types.Green:
                color = Color.green;
                break;
            case GameManager.Types.Red:
                color = Color.red;
                break;
            case GameManager.Types.Blue:
                color = Color.blue;
                break;
            case GameManager.Types.Yellow:
                color = Color.yellow;
                break;
        }
        mesh.material.color = color;
        ParticleSystem.MainModule main = trail.main;
        main.startColor = color;
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
