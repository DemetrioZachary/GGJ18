using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour
{
    [SerializeField]
    PlayerController[] players;

    [SerializeField]
    OffsetAnimator offAnimator;

    private void Awake()
    {
        foreach(PlayerController pl in players)
        {
            pl.OnScorePoint += OnScorePoint;
        }
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
        bool updateVel = true;

            PlayerController plMinScore = null;
            PlayerController plMaxScore = null;
            foreach (PlayerController pl in players)
            {
                if (plMinScore == null || pl.totalScore < plMinScore.totalScore)
                    plMinScore = pl;
                if (plMaxScore == null || pl.totalScore > plMaxScore.totalScore)
                    plMaxScore = pl;
            }

        { 
            if (plMaxScore.totalScore - plMinScore.totalScore > 0)
            {
                foreach (PlayerController pl in players)
                {
                    pl.velocity += 0.2f * (pl.totalScore - plMinScore.totalScore) / (plMaxScore.totalScore - plMinScore.totalScore);
                }
            }
        }
    }
}
