using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour {
    public int player;
    public float speed = 0f;
    public float fireDelay = 1;
    public Projectile projectilePrefab;

    public TransmissionElement[] currSequence = new TransmissionElement[10];
    public int currSequenceNumElement;
    public int currSequenceElement;
    public int currResponseElement;
    public bool sequenceUltimated = false;

    private GameManager.Types shield = GameManager.Types.Green;
    private string inputStr = "";
    private SpriteRenderer spriteRenderer;
    private float fireTime = 0;

    private float sequencePlayTime = -1f;
    private float responsePlayTime = -1f;

    private float delayCheck = -1f;

    public float latestScore = 0f;
    public float totalScore = 0f;

    public Text SequenceFeedback;

    GamePadState state;
    GamePadState prevState;

    private int CurrentSequence = 0;

    const float STANDARD_PRESSION_TIME = 1f;
    const float LONG_PRESSION_TIME = STANDARD_PRESSION_TIME * 1.5f;
    const float VERY_LONG_PRESSION_TIME = STANDARD_PRESSION_TIME * 1.75f;

    const float RUMBLE_TRIGGER = 0.7f;

    public void StartSequence() {
        sequencePlayTime = 0f;
        currSequenceElement = 0;
        currResponseElement = 0;
        sequenceUltimated = false;

        //        Destra, Sinistra, Entrambe

        //* = La pressione del tasto dura di più(+50 %)
        //** = +75 %
        //(Lo spazio tra i due asterischi è perchè sennò fanno casino qui su discord, non ci sono stacchi nelle sequenze)
        //FACILI: DDS - DSD - DDE * -EDE - ESE - SSD
        //NORMALI: EDSE - SDED - SSED - SEED - DDES - SEDE - DSSD - DESDE - SSDSE
        //DIFFICILI: ESDEDE - DSEDSD - DESEEE * -ESDSDD - SSDSEE
        //DIFFICILI ^ 2: EESDE DSD -SEDE EDDS

        CurrentSequence = Random.Range(0,5);
        if (CurrentSequence == 0) {
            currSequenceNumElement = 3;
            currSequence[0].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
            currSequence[1].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
            currSequence[2].Set(TransmissionElement.Types.Left, STANDARD_PRESSION_TIME);
        }
        else if (CurrentSequence == 1) {
            currSequenceNumElement = 3;
            currSequence[0].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
            currSequence[1].Set(TransmissionElement.Types.Left, STANDARD_PRESSION_TIME);
            currSequence[2].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
        }
        else if (CurrentSequence == 2) {
            currSequenceNumElement = 3;
            currSequence[0].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
            currSequence[1].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
            currSequence[2].Set(TransmissionElement.Types.Both, LONG_PRESSION_TIME);
        }
        else if (CurrentSequence == 3) {
            currSequenceNumElement = 3;
            currSequence[0].Set(TransmissionElement.Types.Both, STANDARD_PRESSION_TIME);
            currSequence[1].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
            currSequence[2].Set(TransmissionElement.Types.Both, STANDARD_PRESSION_TIME);
        }
        else if (CurrentSequence == 4) {
            currSequenceNumElement = 3;
            currSequence[0].Set(TransmissionElement.Types.Both, STANDARD_PRESSION_TIME);
            currSequence[1].Set(TransmissionElement.Types.Left, STANDARD_PRESSION_TIME);
            currSequence[2].Set(TransmissionElement.Types.Both, STANDARD_PRESSION_TIME);
        }
        else if (CurrentSequence == 5) {
            currSequenceNumElement = 3;
            currSequence[0].Set(TransmissionElement.Types.Left, STANDARD_PRESSION_TIME);
            currSequence[1].Set(TransmissionElement.Types.Left, STANDARD_PRESSION_TIME);
            currSequence[2].Set(TransmissionElement.Types.Right, STANDARD_PRESSION_TIME);
        }

        //CurrentSequence++;
        //if (CurrentSequence > 5) CurrentSequence = 0;
    }

    private void Awake() {
        for (int i = 0; i < 10; ++i) {
            currSequence[i] = new TransmissionElement();
        }
    }

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.green;
        inputStr = "P" + player;
    }

    void Update() {

        prevState = state;
        state = GamePad.GetState((PlayerIndex)player);

        //Move();
        SetShield();
        Fire();

        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    public void SetPlayerNumber(int number) {
        player = number;
    }

    private void UltimateSequence()
    {
        currResponseElement = currSequenceNumElement;
        latestScore = SequenceRatio();
        sequenceUltimated = true;
        totalScore += latestScore * 100f;
        responsePlayTime = -1f;
        GamePad.SetVibration((PlayerIndex)player, 0f, 0f);
    }

    void FixedUpdate() {
        if (sequencePlayTime >= 0f) {
            float leftRumble = currSequence[currSequenceElement].type == TransmissionElement.Types.Left || currSequence[currSequenceElement].type == TransmissionElement.Types.Both ? 1.0f : 0f;
            float rightRumble = currSequence[currSequenceElement].type == TransmissionElement.Types.Right || currSequence[currSequenceElement].type == TransmissionElement.Types.Both ? 1.0f : 0f;
            if (sequencePlayTime < currSequence[currSequenceElement].duration)
                GamePad.SetVibration((PlayerIndex)player, leftRumble, rightRumble);
            else
                GamePad.SetVibration((PlayerIndex)player, 0, 0);
            sequencePlayTime += Time.fixedDeltaTime;
            if (sequencePlayTime > currSequence[currSequenceElement].duration + 0.5f) {
                currSequenceElement++;
                if (currSequenceElement == currSequenceNumElement) {
                    sequencePlayTime = -1f;
                    responsePlayTime = 0f;
                    currResponseElement = 0;
                }
                else {
                    sequencePlayTime = 0f;
                }
            }
        }
        else if (responsePlayTime >= 0f) {
            responsePlayTime += Time.fixedDeltaTime;

            if(responsePlayTime > 2f && currSequence[currResponseElement].responseType == TransmissionElement.Types.None)
            {
                UltimateSequence();
            }

            bool leftRumble = state.Triggers.Left > RUMBLE_TRIGGER;
            bool rightRumble = state.Triggers.Right > RUMBLE_TRIGGER;
            GamePad.SetVibration((PlayerIndex)player, state.Triggers.Left, state.Triggers.Right);
            if (currSequence[currResponseElement].responseType == TransmissionElement.Types.None) {
                if (leftRumble || rightRumble) {
                    if (delayCheck < 0f && (leftRumble && prevState.Triggers.Left <= RUMBLE_TRIGGER || rightRumble && prevState.Triggers.Right <= RUMBLE_TRIGGER))
                        delayCheck = 0.15f;
                    else if (delayCheck > 0f) {
                        delayCheck -= Time.fixedDeltaTime;
                    }
                    else {
                        currSequence[currResponseElement].responseType = leftRumble && rightRumble ? TransmissionElement.Types.Both : leftRumble ? TransmissionElement.Types.Left : TransmissionElement.Types.Right;
                    }
                }
            }
            else if (currSequence[currResponseElement].responseType == TransmissionElement.Types.Left && (!leftRumble || rightRumble) ||
                currSequence[currResponseElement].responseType == TransmissionElement.Types.Right && (leftRumble || !rightRumble) ||
                currSequence[currResponseElement].responseType == TransmissionElement.Types.Both && (!leftRumble || !rightRumble)) {
                SequenceFeedback.text = string.Format("Punteggio P{0} : {1:P3} - {2:n3}", player, latestScore, totalScore);
                currSequence[currResponseElement].responseDuration = responsePlayTime;
                currResponseElement++;
                if (currResponseElement >= currSequenceNumElement)
                {
                    UltimateSequence();
                }
                else {
                    responsePlayTime = 0f;
                }
            }
        }
        else {
            SequenceFeedback.text = string.Format("Punteggio P{0} : {1:P3} - {2:n3}", player, latestScore, totalScore);
            GamePad.SetVibration((PlayerIndex)player, 0f, 0f);
        }

    }

    public float SequenceRatio() {
        float ratio = 0f;
        for (int i = 0; i < currSequenceNumElement; ++i) {
            ratio += (currSequence[i].type == currSequence[i].responseType ? 1f : 0f) * (1f - Mathf.Abs(currSequence[i].responseDuration - currSequence[i].duration) / currSequence[i].duration) / (float)currSequenceNumElement;
        }

        return ratio;
    }

    private void SetShield() {
        //shield = GameManager.Types.None;
        //if (prevState.DPad.Down == ButtonState.Released && state.DPad.Down == ButtonState.Pressed) { shield = GameManager.Types.Green; }
        //else if(prevState.DPad.Right == ButtonState.Released && state.DPad.Right == ButtonState.Pressed) { shield = GameManager.Types.Red; }
        //else if (prevState.DPad.Left == ButtonState.Released && state.DPad.Left == ButtonState.Pressed) { shield = GameManager.Types.Blue; }
        //else if (prevState.DPad.Up == ButtonState.Released && state.DPad.Up == ButtonState.Pressed) { shield = GameManager.Types.Yellow; }

        if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed) { shield = GameManager.Types.Green; }
        else if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed) { shield = GameManager.Types.Red; }
        else if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed) { shield = GameManager.Types.Blue; }
        else if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed) { shield = GameManager.Types.Yellow; }

        switch (shield) {
            case GameManager.Types.Green:
                spriteRenderer.color = Color.green;
                break;
            case GameManager.Types.Red:
                spriteRenderer.color = Color.red;
                break;
            case GameManager.Types.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case GameManager.Types.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
                //default:
                //    spriteRenderer.color = Color.white;
                //    break;

        }
    }

    private void Fire() {
        //if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed) { type = GameManager.Types.Green; }
        //else if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed) { type = GameManager.Types.Red; }
        //else if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed) { type = GameManager.Types.Blue; }
        //else if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed) { type = GameManager.Types.Yellow; }
        //else { return; }

        if (fireTime <= 0 && prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed) {
            float xValue = state.ThumbSticks.Left.X, yValue = state.ThumbSticks.Left.Y;
            if (Mathf.Abs(xValue) < 0.01 && Mathf.Abs(yValue) < 0.01) { xValue = 1; }
            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(yValue, xValue) * 180 / Mathf.PI)) as Projectile;
            projectile.Initialize(shield);
            fireTime = fireDelay;
        }
        else if (fireTime > 0) {
            fireTime -= Time.deltaTime;
        }
    }

    public void HandleHit(GameManager.Types HitType) {
        if (HitType != shield) {
            // TODO
        }
    }
}
