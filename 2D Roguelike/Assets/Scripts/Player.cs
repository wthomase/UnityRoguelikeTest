using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartDelay = 1f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator; 
	private int food;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator> ();
		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food: " + food;

		base.Start ();
	}

	private void onDisable() {
		GameManager.instance.playerFoodPoints = food;
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.playersTurn) { 
			return;
		} else {
			int horizontal = 0;
			int vertical = 0;

			horizontal = (int) Input.GetAxisRaw ("Horizontal");
			vertical = (int) Input.GetAxisRaw ("Vertical");

			if (horizontal != 0) {
				vertical = 0;
			}

			if (horizontal != 0 || vertical != 0) {
				AttemptMove<Wall> (horizontal, vertical);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Exit") {
			Invoke ("restart", restartDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		}
	}

	protected override void OnCantMove<T> (T component) {
		Wall hitWall = component as Wall;
		hitWall.damageWall (wallDamage);
		animator.SetTrigger ("playerSwipe");
	}

	private void restart() {
		Application.LoadLevel (Application.loadedLevel);
	}

	public void loseFood(int loss) {
		animator.SetTrigger ("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver ();
	}

	protected override void AttemptMove<T> (int xdir, int ydir)
	{
		food--;
		foodText.text = "Food: " + food; 
		base.AttemptMove <T> (xdir, ydir);

		RaycastHit2D hit;
		if (Move (xdir, ydir, out hit)) {
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void CheckIfGameOver() {
		if (food <= 0) {
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.musicSource.Stop ();
			GameManager.instance.GameOver ();
		}
	}
}
