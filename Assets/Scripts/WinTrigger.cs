using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinTrigger : MonoBehaviour {

    public Text winnerText;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            FindObjectOfType<GameManager>().DeclareWinner();
            winnerText.text = collision.gameObject.name.ToUpper() + "WINS";
        }
    }
}
