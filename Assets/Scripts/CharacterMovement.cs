using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CharacterMovement : MonoBehaviour {
	public enum State {
		STILL,
		GOING_UP,
		FALLING_DOWN,
		DEAD
	};
	public Text scoreText;
	public Text gameOverText;
	float myScore;
	public const int CARROT_SCORE = 10;
	public const int BEAN_SCORE = 1;

	public bool facingRight;
	public int score;
	public bool hasHitPlatformOnce;
	public State state;
	public float lastPushTime;

	public Transform powers;
	Animator anim;

	public int powersSpawned = 0;

	public const int maxPowers = 6;
	public const float maxJumpForce = 50.0f;
	public const float jumpPowerScale = 50.0f;
	public const float boosterJumpForce = 60.0f;
	public const float speed = 3.0f;

	public float startJumpingTime;
	public float jumpForce;
	// Use this for initialization
	void Start () {
		facingRight = true;
		score = 0;
		myScore = 0.0f;
		anim = GetComponent<Animator> ();
		hasHitPlatformOnce = false;
		state = State.STILL;
		lastPushTime = 0.0f;
		startJumpingTime = 0.0f;
		jumpForce = 0.0f;
	}

	void Flip () {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void Update () {
		// Can't do anything when dead!
		if (state == State.DEAD) {
			return;
		}
		if (Input.GetButton ("Jump") && (state == State.STILL)) {
			jumpForce = Mathf.Min ((Time.time - startJumpingTime) * jumpPowerScale, maxJumpForce);
			SetState (State.GOING_UP);
		} else if (state == State.GOING_UP) {
			if (rigidbody2D.velocity.y <= 0) {
				SetState (State.FALLING_DOWN);
			}
		}
		UsePowersIfClicking();

		float xMovement = Input.GetAxis ("Horizontal");
		
		if (Mathf.Abs (xMovement) > 0) {
			if (xMovement < 0 && facingRight) {
				Flip ();
			} else if (xMovement > 0 && !facingRight) {
				Flip ();
			}
			Vector3 newPosition = new Vector3(transform.position.x + Input.GetAxis ("Horizontal") * speed, transform.position.y, transform.position.z);
			transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
			anim.SetBool("Running", true);
		} else {
			anim.SetBool("Running", false);
		}
		if (Input.GetAxis ("Vertical") < 0) {
			rigidbody2D.AddForce(new Vector2(0, Input.GetAxis ("Vertical") * 2));
		}
	}

	void UsePowersIfClicking() {
		if (Input.GetButtonDown ("Fire1") && powersSpawned < maxPowers) {
			powersSpawned++;
			Vector3 pos = Input.mousePosition;
			pos.z = -Camera.main.transform.position.z;
			Vector3 point = Camera.main.ScreenToWorldPoint(pos);
			Vector3 direction = point - transform.position;
			Vector3 offset = new Vector3(Mathf.Sign(direction.x) * collider2D.bounds.extents.x,
			                             Mathf.Sign (direction.y) * collider2D.bounds.extents.y,
			                             0);
			Vector3 spawnPosition = transform.position + offset;

			Vector2 forceDir = new Vector2(direction.x * 100, direction.y * 100);
			Transform obj = (Transform) Instantiate (powers, spawnPosition, transform.rotation);
			//obj.rotation = Quaternion.AngleAxis (Vector2.Angle (Vector2.right, forceDir), Vector3.back);

			obj.rigidbody2D.AddForce(forceDir);
		}
	}

	void FixedUpdate () 
	{
		myScore = Mathf.Lerp (myScore, GetScore(), Time.deltaTime );
		if (Mathf.CeilToInt (myScore) != int.Parse (scoreText.text)) 
		{
			scoreText.text = "" + Mathf.FloorToInt (myScore);
		}
	}
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Carrot") {
			score += CARROT_SCORE;
		} else if (coll.gameObject.tag == "Bean") {
			score += BEAN_SCORE;
			if (rigidbody2D.velocity.y < 20) {
				if (rigidbody2D.velocity.y < 1) {
					jumpForce = 35.0f;
				} else {
					jumpForce = 10.0f/Mathf.Abs (rigidbody2D.velocity.y);
				}
				SetState (State.GOING_UP);
			}
		}
	}

	public void Kill() {
		if (hasHitPlatformOnce) {
			SetState(State.DEAD);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Platform") {
			// Only set state still if colliding with top of platform
			// else, probably falling or something
			Vector3 platTop = (coll.gameObject.transform.position + 
					new Vector3(0, coll.gameObject.collider2D.bounds.extents.y, 0));
			float platformY = platTop.y;
			float platformMinX = coll.gameObject.transform.position.x - coll.gameObject.collider2D.bounds.extents.x;
			float platformMaxX = coll.gameObject.transform.position.x + coll.gameObject.collider2D.bounds.extents.x;
			for (int i = 0; i < coll.contacts.Length; i++ ){
				if (coll.contacts[i].point.y > platformY && coll.contacts[i].point.x < platformMaxX && coll.contacts[i].point.x > platformMinX) {
					SetState (State.STILL);
					hasHitPlatformOnce = true;
					break;
				}
			}
		} else if (coll.gameObject.tag == "Ground") {
			if (hasHitPlatformOnce) {
				SetState (State.DEAD);
			} else {
				SetState(State.STILL);
			}
		} else if (coll.gameObject.tag == "Booster" && state != State.GOING_UP) {
			for (int i = 0; i < coll.contacts.Length; i++) {
				if (coll.contacts[i].point.y > coll.gameObject.transform.position.y && 
				    coll.contacts[i].point.x < (coll.gameObject.transform.position.x + coll.gameObject.collider2D.bounds.extents.x) &&
				    coll.contacts[i].point.x > (coll.gameObject.transform.position.x - coll.gameObject.collider2D.bounds.extents.x)) {
					jumpForce = boosterJumpForce;
					coll.gameObject.GetComponent <Animator>().SetTrigger("Bounce");
					SetState (State.GOING_UP);
					break;
				}
			}
		}
	}

	void SetState(State newState) {
		switch (newState) {
		case State.STILL:
			anim.SetInteger ("StateInt", 0);
			powersSpawned = 0;
			break;
		case State.FALLING_DOWN:
			anim.SetInteger ("StateInt", 2);
			powersSpawned = 0;
			break;
		case State.GOING_UP:
			anim.SetInteger ("StateInt", 1);
			anim.SetTrigger("Jumping");
			rigidbody2D.AddForce(Vector2.up * jumpForce);
			break;
		case State.DEAD:
			anim.SetInteger ("StateInt", 3);
			gameOverText.gameObject.SetActive(true);
			scoreText.gameObject.SetActive (false);
			break;
		}
		state = newState;
	}

	public int GetScore() {
		return score;
	}

}
