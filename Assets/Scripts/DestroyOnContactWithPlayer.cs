using UnityEngine;
using System.Collections;

public class DestroyOnContactWithPlayer : MonoBehaviour {

	public AudioSource playOnDestroy;

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player") {
			if (playOnDestroy != null) {
				AudioSource.PlayClipAtPoint(playOnDestroy.clip, transform.position);
			}
			Destroy (gameObject);
		}
	}
}
