﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using XInputDotNetPure;

public class GameManager : MonoBehaviour {



    public enum State { Splash, Logo, MainMenu, Game, Pause, GameOver, Credits };
    public enum Types { None, Green, Red, Blue, Yellow };

    public float splashTime = 3;
    public RectTransform blackScreen, splashScreen, logoScreen, characterScreen, mainMenu, pauseMenu, gameOverScreen, creditsScreen;
    public Button btn2p, btn3p, btn4p;
    

    private State state = State.Splash;

    private float delayTime = 0;

    private void Awake() {
        StartState(State.Splash);
    }

    void Update() {
        switch (state) {
            case State.Splash:
                if (delayTime > splashTime) {
                    ChangeState(State.Logo);
                    delayTime = 0;
                }
                delayTime += Time.deltaTime;
                break;
            case State.Logo:
                if (delayTime > splashTime)
                {
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
                else if (Input.GetKeyDown(KeyCode.Escape)) {
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
                splashScreen.GetComponent<Image>().DOFade(0, 1).OnComplete(() => { splashScreen.gameObject.SetActive(false); StartState(newState); });
                break;
            case State.Logo:
                Sequence seq = DOTween.Sequence();
                seq.Append(logoScreen.DOAnchorPosY(1400, 0.5f));
                seq.Join(characterScreen.DOAnchorPosX(2600, 0.1f)).OnComplete(() => { characterScreen.gameObject.SetActive(false); StartState(newState); });
                break;
            case State.MainMenu:
                mainMenu.DOAnchorPosY(1200, 1).OnComplete(() => { mainMenu.gameObject.SetActive(false); StartState(newState); });
                break;
            case State.Game:
                if (newState != State.Pause) {
                    StopGame();
                }
                StartState(newState);
                break;
            case State.Pause:
                Time.timeScale = 1;
                pauseMenu.DOAnchorPosY(1200, 0.3f).OnComplete(() => {
                    pauseMenu.gameObject.SetActive(false);
                    if (newState == State.Game) { state = newState; }
                    else { StopGame(); StartState(newState); }
                });
                break;
            case State.GameOver:
                gameOverScreen.DOAnchorPosY(1200, 1).OnComplete(() => { gameOverScreen.gameObject.SetActive(false); StartState(newState); });
                break;
            case State.Credits:
                creditsScreen.position = new Vector2(960, 1615);
                creditsScreen.gameObject.SetActive(false);
                StartState(newState);
                break;
        }
    }
    private void StartState(State newState) {
        state = newState;
        switch (state) {
            case State.Splash:
                blackScreen.GetComponent<RawImage>().color = Color.black;
                blackScreen.gameObject.SetActive(true);
                splashScreen.gameObject.SetActive(true);
                splashScreen.DOAnchorPosY(0, 0.1f);
                blackScreen.GetComponent<RawImage>().DOFade(0, 1).OnComplete(() => { blackScreen.gameObject.SetActive(false); });
                break;
            case State.Logo:
                logoScreen.gameObject.SetActive(true);
                Sequence seq = DOTween.Sequence();
                seq.Append(logoScreen.DOAnchorPosY(0, 0.1f));
                seq.Join(characterScreen.DOAnchorPosX(600, 0.1f));
                splashScreen.GetComponent<RawImage>().DOFade(0, 1).OnComplete(() => { splashScreen.gameObject.SetActive(false); });
                break;
            case State.MainMenu:
                mainMenu.gameObject.SetActive(true);
                mainMenu.DOAnchorPosY(0, 1);
                break;
            case State.Game:
                if (!GetComponent<VelocityManager>().enabled)
                {
                    GetComponent<VelocityManager>().enabled = true;
                    GetComponent<VelocityManager>().StartPlayers();
                }
                break;
            case State.Pause:
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.DOAnchorPosY(0, 0.3f).OnComplete(() => { Time.timeScale = 0; });
                break;
            case State.GameOver:
                gameOverScreen.gameObject.SetActive(true);
                gameOverScreen.DOAnchorPosY(0, 1);
                break;
            case State.Credits:
                creditsScreen.gameObject.SetActive(true);
                creditsScreen.DOAnchorPosY(2700, 30).OnComplete(() => { ChangeState(State.MainMenu); });
                break;
        }
    }// ------------------------------------------------------------------

    public void StartNewGame(int playerNumber) {
       GetComponent<VelocityManager>().playerNumber = playerNumber;
        GetComponentInChildren<CameraController>().CheckPlayers(playerNumber);
        ChangeState(State.Game);

    }

    public void DeclareWinner() {
        ChangeState(State.GameOver);
    }


    private void StopGame() {
        GetComponent<VelocityManager>().StopGame();
        
    }

    public void OpenCredits() {
        ChangeState(State.Credits);
    }

    public void Resume() {
        ChangeState(State.Game);
    }

    public void StartGame() {
        ChangeState(State.Game);
    }

    public void OpenMainMenu() {
        ChangeState(State.MainMenu);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
