using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed = 15;
    public MeshRenderer mesh;

    private GameManager.Types type;


    private void Awake() {   }

    private void Update() {
        transform.Translate(speed * Vector3.right * Time.deltaTime);
    }

    public GameManager.Types GetProjectileType() { return type; }

    public void Initialize(GameManager.Types type) {
        
        this.type = type;
        switch (type) {
            case GameManager.Types.Green:
                mesh.material.color = Color.green;
                break;
            case GameManager.Types.Red:
                mesh.material.color = Color.red;
                break;
            case GameManager.Types.Blue:
                mesh.material.color = Color.blue;
                break;
            case GameManager.Types.Yellow:
                mesh.material.color = Color.yellow;
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
