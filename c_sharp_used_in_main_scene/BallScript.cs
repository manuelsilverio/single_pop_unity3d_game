using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	public GameObject GameManager;
	public bool canPop;
	GameManagerScript gameManagerScript;


	// Use this for initialization
	void Start () {
		GameManager = GameObject.FindWithTag ("GameManager");
		gameManagerScript = GameManager.GetComponent<GameManagerScript> ();
		canPop = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player") {
			canPop = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			canPop = false;
			gameManagerScript.lose();
		}
	}
}
