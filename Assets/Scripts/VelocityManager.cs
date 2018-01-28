using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class VelocityManager : MonoBehaviour
{
    private List<PlayerController> players = new List<PlayerController>();
    public int playerNumber = 2;

    private float[] startPositionsX = { -18, -22, -30 };
    private float[] startPositionsY = { 5, -5, 15, -15 };

    [SerializeField]
    OffsetAnimator offAnimator;

    private float deltaSequence;

    private void Awake()
    {
    }


    public IEnumerator SpawnPlayers(PlayerController[] playerPrefabs)
    {
        for (int i = 0; i < playerNumber; ++i)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
                GamePad.SetVibration((PlayerIndex)i, 0.7f, 0.7f);
                players.Add(Instantiate(playerPrefabs[i], new Vector3(-50, startPositionsY[i], 0), Quaternion.identity) as PlayerController);
                players[i].transform.DOMoveX(startPositionsX[playerNumber - 2], 2).OnComplete(() => { GamePad.SetVibration((PlayerIndex)i, 0, 0); });
                players[i].SetPlayerNumber(i);
            }
            //else { StopGame(); ChangeState(State.MainMenu); }
            yield return new WaitForSeconds(3);
        }
        // TODO
        // Start spawn Bombs
        // Start Sequences
        GetComponent<VelocityManager>().StartSequences();
    }

    public void StopGame()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Destroy(players[i].gameObject);
        }
        players.Clear();
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
        if (players.Count < 2) { return; }//<---
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
