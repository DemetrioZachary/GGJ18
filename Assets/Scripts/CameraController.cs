using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private float[] cameraPosZ = { -25, -25, -30, -40 };

    public float speed = 5;
    public float headOffset = 1;

    private PlayerController[] players;
    private Camera myCamera;
    private Vector3 targetPosition;
    private float fov = 0;
    private float verticalOffset = 0;

	void Start () {
        players = FindObjectsOfType<PlayerController>();
        myCamera = GetComponent<Camera>();
        fov = myCamera.fieldOfView * Mathf.PI / 180f;
        targetPosition = transform.position;
    }
	
	void Update () {
        float magnitude = (targetPosition - transform.position).magnitude;
        if (Mathf.Abs(magnitude) > 0.001f) {
            transform.Translate((targetPosition - transform.position).normalized * magnitude * speed * Time.deltaTime);
        }
    }

    //private void FindPositionX() {
    //    float minX = players[0].transform.position.x, maxX = minX;
    //    foreach (PlayerController player in players) {
    //        float playerX = player.transform.position.x;
    //        if (playerX < minX) {
    //            minX = playerX;
    //        }
    //        else if (playerX > maxX) {
    //            maxX = playerX;
    //        }
    //    }
    //    float range = Mathf.Abs(transform.position.z * Mathf.Tan(fov));
    //    if (maxX - minX < range - headOffset) {
    //        targetPosition = new Vector3(minX + range / 2f - 2 * headOffset, targetPosition.y, targetPosition.z);
    //    }
    //    else {
    //        targetPosition = new Vector3(maxX - range / 2f + headOffset, targetPosition.y, targetPosition.z);
    //    }
    //}

    public void CheckPlayers(int playerNumber) {
        targetPosition = new Vector3(targetPosition.x, playerNumber % 2 == 0 ? 0 : 5, cameraPosZ[playerNumber - 1]);
    }
}
