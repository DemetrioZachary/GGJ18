using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour
{
    List<PlayerController> players = new List<PlayerController>();//<---

    [SerializeField]
    OffsetAnimator offAnimator;

    private float deltaSequence;

    private void Awake()
    {
    }

    public void OnScorePoint(PlayerController sender, float latestScore)
    {
    }

    // Use this for initialization
    void Start () {
		
	}

    public void StartSequences()
    {
        deltaSequence = Random.Range(5f, 7f);
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count == 0) { return; }//<---
        if (deltaSequence > 0f)
        {
            deltaSequence -= Time.deltaTime;
            if (deltaSequence < 0f)
            {
                foreach (PlayerController pl in players)
                {
                    if (pl.gameObject.activeSelf)
                    {
                        pl.StartSequence();
                    }
                }
            }
        }
        bool updateVel = true;

        PlayerController plMinScore = players[0];
        PlayerController plMaxScore = players[1];
        foreach (PlayerController pl in players)
        {
            if (pl.gameObject.activeSelf)
            {
                if (pl.latestScore < plMinScore.latestScore)
                    plMinScore = pl;
                else if (pl.latestScore > plMaxScore.latestScore)
                    plMaxScore = pl;

                if (!pl.sequenceUltimated)
                    updateVel = false;
            }
        }

        if (updateVel)
        {
            deltaSequence = Random.Range(4f, 6f);
            foreach (PlayerController pl in players)
            {
                if (pl.gameObject.activeSelf)
                {
                    pl.sequenceUltimated = false;
                    if ((plMaxScore.latestScore - plMinScore.latestScore) > 0)
                        pl.speed += 0.2f * (pl.latestScore - plMinScore.latestScore) / (plMaxScore.latestScore - plMinScore.latestScore);
                }
            }
        }

        foreach (PlayerController pl in players)
        {
            if (pl.gameObject.activeSelf)
            {
            }
        }
    }

    public void SetPlayerList(List<PlayerController> players) { //<---
        this.players = players;
    }
}
