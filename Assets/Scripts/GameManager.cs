using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using XInputDotNetPure;

public class GameManager : MonoBehaviour {

    private float[] startPositionsX = { };
    private float[] startPositionsY = { };

    public enum State { Splash, MainMenu, Game, Pause, GameOver};
    public enum Types { None, Green, Red, Blue, Yellow };

    public float splashTime = 5;
    public RectTransform blackScreen, splashScreen, mainMenu, pauseMenu, gameOverScreen, creditsScreen;
    public Button btn2p, btn3p, btn4p;

    private State state = State.Splash;
    [SerializeField]
    private PlayerController[] players;
    private int playerNumber = 2;
    private float delayTime = 0;

    private bool playerIndexSet = false;
    GamePadState prevState;

    private void Awake() {
        players = FindObjectsOfType<PlayerController>();
        StartState(State.Splash);
    }

    void Update() {
        switch (state) {
            case State.Splash:
                if (delayTime > splashTime) {
                    ChangeState(State.MainMenu);
                    delayTime = 0;
                } 
                delayTime += Time.deltaTime;
                break;
            case State.MainMenu:
                if (delayTime < 5) {
                    int count = 0;
                    for (int i = 0; i < 4; ++i) {
                        PlayerIndex testPlayerIndex = (PlayerIndex)i;
                        GamePadState testState = GamePad.GetState(testPlayerIndex);
                        if (testState.IsConnected) { count++; }
                    }
                    btn2p.interactable = count >= 2;
                    btn3p.interactable = count >= 3;
                    btn4p.interactable = count == 4;
                    delayTime = 0;
                }
                else { delayTime += Time.deltaTime; }
                break;
            case State.Game:
                if (delayTime < 0.6) { delayTime += Time.deltaTime; }
                else if(Input.GetKeyDown(KeyCode.Escape)){
                    delayTime = 0;
                    ChangeState(State.Pause);
                }
                break;
            case State.Pause:
                if (delayTime < 0.6) { delayTime += Time.deltaTime; }
                else if (Input.GetKeyDown(KeyCode.Escape)) {
                    delayTime = 0;
                    ChangeState(State.Game);
                }
                break;
            default:
                break;
        }
    }

    // ------------------------------------------------------
    private void ChangeState(State newState) {
        switch (state) {
            case State.Splash:
                splashScreen.DOAnchorPosY(1200, 1).OnComplete(() => { splashScreen.gameObject.SetActive(false); StartState(newState); });
                break;
            case State.MainMenu:
                mainMenu.DOAnchorPosY(1200, 1).OnComplete(() => { mainMenu.gameObject.SetActive(false); StartState(newState); });
                break;
            case State.Game:
                if (newState != State.Pause) {
                    // TODO Stop game
                }
                StartState(newState);
                break;
            case State.Pause:
                Time.timeScale = 1;
                pauseMenu.DOAnchorPosY(1200, 0.3f).OnComplete(() => { pauseMenu.gameObject.SetActive(false);
                    if (newState == State.Game) { state = newState; }
                    else { /* Stop game */ StartState(newState); } });
                break;
            case State.GameOver:
                gameOverScreen.DOAnchorPosY(1200, 1).OnComplete(() => { gameOverScreen.gameObject.SetActive(false); StartState(newState); });
                break;
        }
    }
    private void StartState(State newState) {
        state = newState;
        switch (state) {
            case State.Splash:
                splashScreen.DOAnchorPosY(0, 0.1f);
                blackScreen.GetComponent<RawImage>().color = Color.black;
                blackScreen.gameObject.SetActive(true);
                splashScreen.gameObject.SetActive(true);
                blackScreen.GetComponent<RawImage>().DOFade(0, 1).OnComplete(()=> { blackScreen.gameObject.SetActive(false); });
                break;
            case State.MainMenu:
                mainMenu.gameObject.SetActive(true);
                mainMenu.DOAnchorPosY(0, 1);
                break;
            case State.Game:
                // TODO start game

                break;
            case State.Pause:
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.DOAnchorPosY(0, 0.3f).OnComplete(() => { Time.timeScale = 0; });
                break;
            case State.GameOver:
                gameOverScreen.gameObject.SetActive(true);
                gameOverScreen.DOAnchorPosY(0, 1);
                break;
        }
    }// ------------------------------------------------------------------
    
    public void StartNewGame(int playerNumber) {
        this.playerNumber = playerNumber;
    }

    private IEnumerator SpawnPlayers() {
        for (int i = 0; i < playerNumber; ++i) {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected) {
                GamePad.SetVibration((PlayerIndex)i, 0.7f, 0.7f);
                players[i].gameObject.SetActive(true);
                players[i].transform.DOMoveX(startPositionsX[playerNumber - 2], 2).OnComplete(() => { GamePad.SetVibration((PlayerIndex)i, 0, 0); });
            }
            else { StopGame(); ChangeState(State.MainMenu); }
            yield return new WaitForSeconds(3);
        }
    }

    private void StopGame() {

    }

    public void Resume() {
        ChangeState(State.Game);
    }

    public void StartGame() {
        ChangeState(State.Game);
    }

    public void OpenCredits() {
        // TODO
    }

    public void OpenMainMenu() {
        ChangeState(State.MainMenu);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
