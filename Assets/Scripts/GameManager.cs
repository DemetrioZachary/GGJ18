using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour {

    public enum State { Splash, MainMenu, Game, Pause, GameOver};
    public enum Types { None, Green, Red, Blue, Yellow };

    public float splashTime = 5;
    public RectTransform blackScreen, splashScreen, mainMenu, pauseMenu, gameOverScreen, creditsScreen;

    private State state = State.Splash;
    private PlayerController[] players;
    private float delayTime = 0;

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
    
    private void StartNewGame() {

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
