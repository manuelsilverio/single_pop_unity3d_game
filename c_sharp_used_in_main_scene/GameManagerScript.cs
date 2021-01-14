using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManagerScript : MonoBehaviour {

	public int gameStatus;		//0 standby //1 playing // 2 lose  // 3 win
	public GameObject ballCenter;
	public GameObject playerCenter;
	public GameObject ball;
	public GameObject circle;
	public GameObject lockTop;
	public GameObject background;
	public GameObject menuSettings;
	public Transform levelTextSettings;
	public Color[] colorBackground;
	public Color colorBackgroundLose;
	public Transform TextMiddle;
	public Transform TextTop;
	public AudioClip soundTap;
	public AudioClip soundLose;
	public AudioClip soundWin;
	public AudioClip soundWoosh;
	public AudioClip soundMenuTap;


	private RandomRotScript scriptRandomRot;
	private PlayerScript scriptPlayer;
	private BallScript scriptBall;
	private int score;
	private int currentLevel;
	private bool canTap;
	private bool canResetAfterLose;

	private Animator animText;
	private Animator animCircle;
	private Animator animPlayer;
	private Animator animBall;
	private Animator animLockTop;

	private AudioSource audioSpeaker;

	void Awake(){
		//SETTING THE FONT SIZE OF TEXT RELATIVE TO SCREEN SIZE
		/*int textSizeTop = Screen.height / 19;		
		int textSizeMid = Screen.height / 8;
		TextMiddle.fontSize = textSizeMid;
		TextTop.fontSize = textSizeTop;
		float posY = (float) Screen.height / 14.5f;
		TextMiddle.rectTransform.localPosition = new Vector3(0, -posY, 0);*/

		//ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;	//MAKES THE NAVIGATION BAR VISIBLE

		animText = TextMiddle.GetComponent<Animator> ();
		animCircle = circle.GetComponent<Animator> ();
		animPlayer = playerCenter.GetComponent<Animator> ();
		animBall = ballCenter.GetComponent<Animator> ();
		animLockTop = lockTop.GetComponent<Animator> ();
		audioSpeaker = this.GetComponent<AudioSource> ();
		menuSettings.SetActive (false);
		canTap = true;
		canResetAfterLose = true;

	}

	// Use this for initialization
	void Start () {
		//GETTING REFERENCES TO SCRIPTS
		gameStatus = (int) EnumHolder.gamestatuslist.standby;
		scriptRandomRot = ballCenter.GetComponent<RandomRotScript> ();
		scriptPlayer = playerCenter.GetComponent<PlayerScript> ();
		scriptBall = ball.GetComponent<BallScript> ();
		//PlayerPrefs.DeleteAll ();
		score = PlayerPrefs.GetInt("level", 1);
		currentLevel = score;
		updateLevelText ();
		if (PlayerPrefs.GetInt ("level", 1) == 1) {
			performTapSettings();
		}
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit = new RaycastHit();
		#if UNITY_ANDROID
		if(Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			if(Physics.Raycast(ray,out hit, 100f) && hit.transform.tag == "tap screen" && canTap)
			{
				Debug.Log("screen just tapped in android");
				performTap();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.tag == "button settings"){
				performTapSettings();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "ArrowLeft"){
				levelDown();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "ArrowRight"){
				levelUp();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "button Close"){
				performTapSettings();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "Button Exit"){
				Application.Quit();
			}
		}		
		#endif
		#if UNITY_EDITOR
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit, 100f) && hit.transform.tag == "tap screen" && canTap)
			{
				//Debug.Log("screen just tapped in editor");
				performTap();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.tag == "button settings"){
				performTapSettings();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "ArrowLeft"){
				levelDown();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "ArrowRight"){
				levelUp();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "button Close"){
				performTapSettings();
			}else if(Physics.Raycast(ray,out hit, 100f) && hit.transform.name == "Button Exit"){
				Debug.Log("button exit pressed");
			}
			
		}				
		#endif


	}

	void FixedUpdate(){
		TextMiddle.GetComponent<TextMesh>().text = ""+score;

	}

	private void playSound(AudioClip clip, float soundVolume){
		audioSpeaker.clip = clip;
		audioSpeaker.volume = soundVolume;
		audioSpeaker.Play();
	}

	private void performTapSettings(){
		playSound (soundMenuTap, 0.5f);
		if (canTap) {
			canTap = false;
			menuSettings.SetActive(true);
			levelTextSettings.GetComponent<TextMesh>().text = "Level: "+currentLevel;
		} else {
			menuSettings.SetActive(false);
			canTap = true;
			updateLevelText();
			resetGame();
		}
	}

	private void levelUp(){
		if (currentLevel < PlayerPrefs.GetInt ("level", 1)) {
			playSound (soundMenuTap, 0.5f);
			currentLevel++;
			levelTextSettings.GetComponent<TextMesh>().text = "Level: "+currentLevel;
		}
	}

	private void levelDown(){
		if (currentLevel > 1) {
			playSound (soundMenuTap, 0.5f);
			currentLevel--;
			levelTextSettings.GetComponent<TextMesh>().text = "Level: "+currentLevel;
		}
	}

	private void performTap()
	{
		switch(gameStatus)
		{
		case (int) EnumHolder.gamestatuslist.standby:
			//scriptRandomRot.resetRotation();
			scriptPlayer.rotationDirection = -scriptRandomRot.side;
			scriptPlayer.canRotate = true;
			scriptBall.canPop = false;
			gameStatus = (int) EnumHolder.gamestatuslist.playing;
			break;
			
		case (int) EnumHolder.gamestatuslist.playing:
			if(scriptBall.canPop){
				Debug.Log("rotation direction: "+scriptPlayer.rotationDirection);

				scriptBall.GetComponent<CircleCollider2D>().enabled = false;
				float newAngle = 0f;
				if(scriptPlayer.rotationDirection>0){
					newAngle = Random.Range(-40f, -90f); 
				}else{
					newAngle = Random.Range(40f, 90f);
				}
				scriptRandomRot.replaceRotation(newAngle);
				Debug.Log ("new angle: "+newAngle);
				scriptBall.GetComponent<CircleCollider2D>().enabled = true;
				scriptPlayer.rotationDirection*= -1;
				scriptBall.canPop = false;
				score--;
				if(score<=0){
					playSound (soundWin, 0.05f);
					win ();
				}else{
					playSound (soundTap, 0.4f);
				}

			}else{
				lose();
			}

			break;
			
		case (int) EnumHolder.gamestatuslist.lose:
			if(canResetAfterLose){
				resetGame();
				gameStatus = (int) EnumHolder.gamestatuslist.standby;
				background.GetComponent<SpriteRenderer> ().color = colorBackground[0];
			}

			break;
			
		case (int) EnumHolder.gamestatuslist.win:

			break;
			
			
		}
	}

	private void win(){
		ball.SetActive (false);
		gameStatus = (int)EnumHolder.gamestatuslist.win;
		scriptPlayer.canRotate = false;

		firstAnimationRound ();
		Invoke ("secondAnimationRound", 1f);
		Invoke ("resetGame", 1.15f);
		Invoke ("thirdAnimationRound", 2f);
		Invoke ("finishWinning", 2.4f);
	}

	public void lose(){
		playSound (soundLose, 1f);
		gameStatus = (int)EnumHolder.gamestatuslist.lose;
		scriptPlayer.canRotate = false;
		background.GetComponent<SpriteRenderer> ().color = colorBackgroundLose;
		if (scriptPlayer.rotationDirection > 0) {
			animLockTop.SetTrigger ("canSpinLeft");
			animText.SetTrigger("canSpinLeft");
		} else {
			animLockTop.SetTrigger ("canSpinRight");
			animText.SetTrigger("canSpinRight");
		}
		canResetAfterLose = false;
		Invoke ("setCanResetAfterLose", 0.5f);
	}

	private void setCanResetAfterLose(){
		canResetAfterLose = true;
	}

	private void resetGame(){
		scriptPlayer.canRotate = false;
		scriptRandomRot.resetRotation ();
		scriptPlayer.resetRotation ();
		scriptPlayer.resetRotationSpeed ();
		score = currentLevel;
	}

	private void updateLevelText(){
		TextTop.GetComponent<TextMesh>().text = "Level: " + currentLevel;
	}

	//METHODS FOR LOSING



	//METHODS FOR WINING

	private void firstAnimationRound(){
		animText.SetTrigger ("canFadeOut");
		animLockTop.SetTrigger ("canSlideUp");
	}

	private void secondAnimationRound(){
		currentLevel++;
		animBall.SetTrigger ("canSlide");
		animCircle.SetTrigger ("canSlide");
		animPlayer.SetTrigger ("canSlide");
		playSound (soundWoosh, 0.5f);
	}

	private void thirdAnimationRound(){

		updateLevelText ();
		if (currentLevel > PlayerPrefs.GetInt ("level", 1)) {
			PlayerPrefs.SetInt ("level", currentLevel);
		}
		animText.SetTrigger ("canFadeIn");
		animLockTop.SetTrigger ("canSlideDown");
		ball.SetActive(true);
	}

	private void finishWinning(){
		scriptPlayer.rotationDirection = -scriptRandomRot.side;
		gameStatus = (int) EnumHolder.gamestatuslist.standby;
	}

}
