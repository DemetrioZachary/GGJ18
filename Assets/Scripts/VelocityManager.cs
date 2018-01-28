using DG.Tweening;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class VelocityManager : MonoBehaviour
{
    private List<PlayerController> players = new List<PlayerController>();
    public int playerNumber = 2;
    public PlayerController[] playerPrefabs;
    //public DeathTrigger deathTrigger;
    //public WinTrigger winTrigger;

    private float[] startPositionsX = { -18, -22, -30 };
    private float[] startPositionsY = { 6, -5, 16, -15 };

    [SerializeField]
    OffsetAnimator offAnimator;

    private float deltaSequence;

    public GameObject BombAir;
    public GameObject BombWater;

    // This stores all spawned prefabs, so they can be despawned later
    private Stack<GameObject> spawnedPrefabs = new Stack<GameObject>();

    public void SpawnBomb()
    {
        bool isAir = Random.value > 0.5f;
        float yPos = isAir ? (Random.value > 0.5f ? 6 : 8) : (Random.value > 0.5f ? -10 : -12);

        var clone = LeanPool.Spawn(isAir ? BombAir : BombWater, Vector3.right * 30f + Vector3.up * yPos, Quaternion.identity, null);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(clone.transform.DOMoveX(-30, Random.Range(40f, 45f)))
            .Join(clone.transform.DOShakePosition(5,0.2f,1).SetLoops(-1, LoopType.Yoyo));

        // Add the clone to the clones stack if it doesn't exist
        // If this prefab can be recycled then it could already exist
        if (spawnedPrefabs.Contains(clone) == false)
        {
            spawnedPrefabs.Push(clone);
        }
    }

    public void DespawnPrefab()
    {
        if (spawnedPrefabs.Count > 0)
        {
            // Get the last clone
            var clone = spawnedPrefabs.Pop();

            // Despawn it
            LeanPool.Despawn(clone);

        }
    }

    private void Awake()
    {

    }

    public void StartPlayers()
    {
        StartCoroutine(SpawnPlayers());
    }

    public IEnumerator SpawnPlayers()
    {
        // Randomizzo la scelta del modello di dirimarino
        int[] rndIdx = { 0, 1, 2, 3 };
        for (int i = 0; i < 10; ++i)
        {
            int swapf = Random.Range(0, 3);
            int swapt = Random.Range(0, 3);
            int app = rndIdx[swapf];
            rndIdx[swapf] = rndIdx[swapt];
            rndIdx[swapt] = app;
        }

        for (int i = 0; i < playerNumber; ++i)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected)
            {
                PlayerController pl = Instantiate(playerPrefabs[rndIdx[i]], new Vector3(-50, startPositionsY[i], 0), Quaternion.identity) as PlayerController;

                pl.SetPlayerNumber(i);
                if (pl.transform.position.y > 0f)
                {
                    pl.transform.Rotate(Vector3.right, 180f);
                    pl.bubblesPs.Stop();
                    pl.GetComponent<Rigidbody2D>().angularDrag = 100;
                    pl.GetComponent<Rigidbody2D>().drag = 100;
                }
                pl.transform.DOMoveX(startPositionsX[playerNumber - 2], 2f).OnUpdate(() => GamePad.SetVibration((PlayerIndex)i, 1f, 1f));

                players.Add(pl);
            }

            yield return new WaitForSeconds(3);
        }
        // TODO
        // Start spawn Bombs
        // Start Sequences
        //deathTrigger.gameObject.SetActive(true);
        //winTrigger.transform.position = new Vector3(-startPositionsX[playerNumber - 2] + 15, 0, 0);
        //deathTrigger.transform.position = new Vector3(startPositionsX[playerNumber - 2] - 15, 0, 0);
        deltaSequence = Random.Range(5f, 7f);
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
    void Start()
    {

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
            SpawnBomb();
            foreach (PlayerController pl in players)
            {
                if (pl.gameObject.activeSelf)
                {
                    pl.sequenceUltimated = false;
                    float deltaScore = (plMaxScore.latestScore - plMinScore.latestScore);
                    if (deltaScore > 0)
                    {
                        if (plMaxScore.transform.position.x > 10)
                        {
                            pl.speed -= 0.2f * (plMaxScore.latestScore - pl.latestScore) / deltaScore;
                        }
                        else
                        {
                            pl.speed += 0.2f * (pl.latestScore - plMinScore.latestScore) / deltaScore;
                        }
                    }
                }
            }

            float speedForBck = float.MaxValue;
            foreach (PlayerController pl in players)
            {
                if (pl.gameObject.activeSelf)
                {
                    if (speedForBck > pl.speed)
                        speedForBck = pl.speed;
                }
            }

            foreach (PlayerController pl in players)
            {
                if (pl.gameObject.activeSelf)
                {
                    pl.speed -= speedForBck;
                }
            }

            offAnimator.Speed += speedForBck * 0.1f;
        }

        // Swap dei binari e impostazione del livello
        foreach (PlayerController pl in players)
        {
            if (pl.gameObject.activeSelf)
            {
                if (pl.transform.position.x < -10f)
                    pl.CurrLevel = 0;
                else if (pl.transform.position.x > 10f)
                    pl.CurrLevel = 2;
                else
                    pl.CurrLevel = 1;

            }
        }
    }

    public void SetPlayerList(List<PlayerController> players)
    { //<---
        this.players = players;
    }
}
