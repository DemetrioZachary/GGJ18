using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float acceleration = 5;

    private GameManager.Types type = GameManager.Types.Green;

	void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.CompareTag("Projectile")) {
            SetBombType(coll.gameObject.GetComponent<Projectile>().GetProjectileType());
            EngageBomb();
        }
        else if (coll.gameObject.CompareTag("Player")) {
            // TODO hit player
        }
    }

    private void SetBombType(GameManager.Types type) {
        this.type = type;
        // TODO change asset
    }

    private void EngageBomb() {
        // TODO sganciare vincolo
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, acceleration));
    }
}
