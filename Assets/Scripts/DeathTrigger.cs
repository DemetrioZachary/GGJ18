using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeathTrigger : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D coll) {
        if (coll.CompareTag("Player")) {
            coll.GetComponent<PlayerController>().enabled = false;
            coll.transform.DOMoveX(-50, 5);
            // BOOM
        }
    }
}
