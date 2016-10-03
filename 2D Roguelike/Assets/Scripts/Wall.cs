using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public Sprite damageSprite;
	public int hp = 4;
	public AudioClip chopSound1;
	public AudioClip chopSound2;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	public void damageWall(int dmgLoss) {
		SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
		spriteRenderer.sprite = damageSprite;
		hp -= dmgLoss;

		if (hp <= 0) {
			gameObject.SetActive(false);
		}
	}

}
