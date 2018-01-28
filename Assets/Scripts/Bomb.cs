using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bomb : MonoBehaviour
{

    public float minTime = 8;
    public float maxTime = 10;
    public GameObject bombBase, bombBody;
    public ParticleSystem pulse, explosion;

    private GameManager.Types type = GameManager.Types.Green;

    void Start()
    {

    }

    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            coll.gameObject.GetComponent<PlayerController>().HandleHit(type);


        }
    }

    private IEnumerator Detonate()
    {
        pulse.Stop();
        explosion.Play();
        Destroy(bombBody.gameObject);
        Destroy(bombBase.gameObject);
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void TriggerBomb(GameManager.Types type)
    {
        this.type = type;
        ParticleSystem.MainModule main = pulse.main;
        switch (type)
        {
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
        Destroy(bombBase);
        transform.DOMoveY(transform.position.y < 0 ? 18 : -18, Random.Range(minTime, maxTime));
        pulse.Play();
    }
}
