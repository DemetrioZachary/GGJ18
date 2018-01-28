using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour {

    private float[] cameraPosZ = { -25, -25, -30, -40 };

    public float speed = 5;
    public float headOffset = 1;

    private PlayerController[] players;
    private Camera myCamera;
    private float fov = 0;
    private float verticalOffset = 0;

	void Start () {
        players = FindObjectsOfType<PlayerController>();
        myCamera = GetComponent<Camera>();
        fov = myCamera.fieldOfView * Mathf.PI / 180f;
    }
	
	void Update () {

    }

    public void CheckPlayers(int playerNumber) {
        transform.DOMove(new Vector3(0, playerNumber % 2 == 0 ? 0 : 5, cameraPosZ[playerNumber - 1]), 0.5f);
    }
}
