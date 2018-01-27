using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour {
    public int player;
    public float speed = 1;
    public float railLength = 10;
    public float angle = 0;
    public float fireDelay = 1;
    public Projectile projectilePrefab;

    public TransmissionElement[] currSequence = new TransmissionElement[10];
    public int currSequenceNumElement;
    public int currSequenceElement;
    public int currResponseElement;

    private GameManager.Types shield = GameManager.Types.Green;
    private string inputStr = "";
    private SpriteRenderer spriteRenderer;
    private float fireTime = 0;

    private float sequencePlayTime = -1f;
    private float responsePlayTime = -1f;

    public Text SequenceFeedback;

    GamePadState state;
    GamePadState prevState;

    private void Awake()
    {
        for(int i=0; i<10;++i)
        {
            currSequence[i] = new TransmissionElement();
        }
    }

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.green;
        if (player == 2) { speed *= -1; }
        inputStr = "P" + player;
    }

    void Update() {

        prevState = state;
        state = GamePad.GetState((PlayerIndex)player);

        //Move();
        SetShield();
        Fire();
    }

    void FixedUpdate()
    {
        // SetVibration should be sent in a slower rate.
        // Set vibration according to triggers
        if (sequencePlayTime >= 0f)
        {
            float leftRumble = currSequence[currSequenceElement].type == TransmissionElement.Types.Left || currSequence[currSequenceElement].type == TransmissionElement.Types.Both ? 1.0f : 0f;
            float rightRumble = currSequence[currSequenceElement].type == TransmissionElement.Types.Right || currSequence[currSequenceElement].type == TransmissionElement.Types.Both ? 1.0f : 0f;
            GamePad.SetVibration((PlayerIndex)player, leftRumble, rightRumble);
            sequencePlayTime += Time.fixedDeltaTime;
            if (sequencePlayTime > currSequence[currSequenceElement].duration)
            {
                currSequenceElement++;
                if (currSequenceElement == currSequenceNumElement)
                {
                    sequencePlayTime = -1f;
                    responsePlayTime = 0f;
                    currResponseElement = -1;
                }
                else
                {
                    sequencePlayTime = 0f;
                }
            }
        } 
        else if(responsePlayTime >= 0f)
        {
            responsePlayTime += Time.fixedDeltaTime;

            bool leftRumble = state.Triggers.Left > 0.5f;
            bool rightRumble = state.Triggers.Right > 0.5f;
            GamePad.SetVibration((PlayerIndex)player, state.Triggers.Left, state.Triggers.Right);
            if (currResponseElement == -1 || currSequence[currResponseElement].responseType == TransmissionElement.Types.None)
            {
                if (currResponseElement == currSequenceNumElement)
                {
                    responsePlayTime = -1f;
                }
                else
                {
                    responsePlayTime = 0f;
                }

                if (leftRumble || rightRumble)
                {
                    if(currResponseElement == -1)
                        currResponseElement = 0;
                    responsePlayTime = 0;
                    currSequence[currResponseElement].responseType = leftRumble && rightRumble ? TransmissionElement.Types.Both : leftRumble ? TransmissionElement.Types.Left : TransmissionElement.Types.Right;
                }
            }
            else if(currSequence[currResponseElement].responseType == TransmissionElement.Types.Left && (!leftRumble || rightRumble) ||
                currSequence[currResponseElement].responseType == TransmissionElement.Types.Right && (leftRumble || !rightRumble) ||
                currSequence[currResponseElement].responseType == TransmissionElement.Types.Both && (!leftRumble || !rightRumble))
            {
                currSequence[currResponseElement].responseDuration = responsePlayTime;
                currResponseElement++;
                if (currResponseElement == currSequenceNumElement)
                {
                    responsePlayTime = -1f;
                }
                else
                {
                    responsePlayTime = 0f;
                }
            }
        }
        else
        {
            SequenceFeedback.text = string.Format("{0:P3}", SequenceRatio());
            GamePad.SetVibration((PlayerIndex)player, 0f, 0f);
        }

    }

    public void StartSequence()
    {
        sequencePlayTime = 0f;
        currSequenceElement = 0;

        currSequenceNumElement = 3;
        currSequence[0].Set(TransmissionElement.Types.Left, 2f);
        currSequence[1].Set(TransmissionElement.Types.Right, 1.5f);
        currSequence[2].Set(TransmissionElement.Types.Left, 2f);
        //currSequence[3].Set(TransmissionElement.Types.Right, 1f);
        //currSequence[4].Set(TransmissionElement.Types.Left, 1.5f);
    }

    public float SequenceRatio()
    {
        float ratio = 0f;
        for(int i= 0; i< currSequenceNumElement; ++i)
        {
            ratio += (currSequence[i].type == currSequence[i].responseType ? 1f : 0f) * (1f - Mathf.Abs(currSequence[i].responseDuration - currSequence[i].duration) / currSequence[i].duration) / (float)currSequenceNumElement;
        }

        return ratio;
    }

    //private void Move() {
    //    float yPos = Mathf.Sin(angle) * railLength / 2f;
    //    transform.position = new Vector3(transform.position.x, yPos, 0);
    //    angle += Time.deltaTime * speed;
    //    if (Mathf.Abs(angle) >= Mathf.PI * 2) { angle = 0; }
    //}

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
        //GameManager.Types type;
        //if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
        //{
        //    StartSequence();
        //}

        //if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed) { type = GameManager.Types.Green; }
        //else if (prevState[0].Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed) { type = GameManager.Types.Red; }
        //else if (prevState[0].Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed) { type = GameManager.Types.Blue; }
        //else if (prevState[0].Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed) { type = GameManager.Types.Yellow; }
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
