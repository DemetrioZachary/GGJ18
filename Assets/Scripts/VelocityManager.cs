using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour
{
    [SerializeField]
    PlayerController[] players;

    [SerializeField]
    OffsetAnimator offAnimator;

    private float deltaSequence;

    private void Awake()
    {
        deltaSequence = Random.Range(5f, 7f);
    }

    public void OnScorePoint(PlayerController sender, float latestScore)
    {
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(deltaSequence > 0f)
        {
            deltaSequence -= Time.deltaTime;
            if(deltaSequence < 0f)
            {
                deltaSequence = Random.Range(8f, 10f);
                foreach (PlayerController pl in players)
                {
                    pl.StartSequence();
                }
            }
        }
        bool updateVel = true;

        PlayerController plMinScore = null;
        PlayerController plMaxScore = null;
        foreach (PlayerController pl in players)
        {
            if (pl.gameObject.activeSelf)
            {
                if (plMinScore == null || pl.latestScore < plMinScore.latestScore)
                    plMinScore = pl;
                if (plMaxScore == null || pl.latestScore > plMaxScore.latestScore)
                    plMaxScore = pl;

                if (!pl.sequenceUltimated)
                    updateVel = false;
            }
        }

        if (updateVel && (plMaxScore.latestScore - plMinScore.latestScore) > 0)
        {
            foreach (PlayerController pl in players)
            {
                pl.sequenceUltimated = false;
                pl.velocity += 0.2f * (pl.latestScore - plMinScore.latestScore) / (plMaxScore.latestScore - plMinScore.latestScore);
            }
        }
    }
}
