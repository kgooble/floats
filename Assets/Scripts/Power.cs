using UnityEngine;
using System.Collections;

public class Power : MonoBehaviour {

	public float timeToLive = 5.0f;
	public float lifeTime;

	public float maxY;
	// Use this for initialization
	void Start () {
		lifeTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		lifeTime += Time.deltaTime;
		if (lifeTime > timeToLive) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Obstacle" || coll.gameObject.tag == "Edge") {
			Destroy (gameObject);
		}
	}
}
