using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Edge" || coll.gameObject.tag == "Power") {
			collider2D.isTrigger = true;
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Edge" || coll.gameObject.tag == "Power") {
			collider2D.isTrigger = true;
		}
	}
}
