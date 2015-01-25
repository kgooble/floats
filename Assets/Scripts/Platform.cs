using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

	public bool jumpedOn;
	// Use this for initialization
	void Start () {
		jumpedOn = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
			for (int i = 0; i < coll.contacts.Length; i++) {
				// Multiplying by constants to prevent platforms disappearing when the player
				// Collides with them on the side
				if (coll.contacts[i].point.y > transform.position.y + (collider2D.bounds.extents.y * 0.75)&&
					coll.contacts[i].point.x < (transform.position.x + (collider2D.bounds.extents.x * 0.9)) &&
					coll.contacts[i].point.x > (transform.position.x - (collider2D.bounds.extents.x * 0.9))) {
					jumpedOn = true;
				}
			}
		}
	}

	void OnCollisionExit2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
			if (jumpedOn) {
				Destroy(gameObject);
			}
		}
	}
}
