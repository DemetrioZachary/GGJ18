using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeathTrigger : MonoBehaviour {

    public ParticleSystem explosion;

    private void OnTriggerEnter2D(Collider2D coll) {
        if (coll.CompareTag("Player")) {
            coll.GetComponent<PlayerController>().enabled = false;
            coll.transform.DOMoveX(-50, 5).OnComplete(() => { Destroy(coll.gameObject); });
            StartCoroutine(Detonate(coll.transform.position));
        }
    }

    private IEnumerator Detonate(Vector3 pos) {
        ParticleSystem exp = Instantiate(explosion, pos, Quaternion.identity) as ParticleSystem;
        yield return new WaitForSeconds(2);
        Destroy(exp.gameObject);
    }
}
