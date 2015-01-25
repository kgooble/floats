using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public Transform background;
	public Transform target;
	public Transform highestPlatform;
	public Transform secondHighestPlatform;

	public Transform leftEdge;
	public Transform rightEdge;

	public Transform normalPlatform;
	public Transform booster;
	public Transform carrot;
	public Transform bean;
	public Transform obstacle;

	public float minY;
	public float maxY;

	public float minXForSpawn;
	public float maxXForSpawn;

	void Start () {
		minY = transform.position.y - Camera.main.rect.height/2;
		maxY = transform.position.y + Camera.main.rect.height/2;

		minXForSpawn = leftEdge.position.x + leftEdge.collider2D.bounds.extents.x;
		maxXForSpawn = rightEdge.position.x - rightEdge.collider2D.bounds.extents.x;
	}
	void FixedUpdate() {
		if (!target.renderer.isVisible) {
			target.GetComponent<CharacterMovement>().Kill();
		}
		if (Input.GetButton ("Restart")) {
			Application.LoadLevel(1);
		}
		if (highestPlatform) {
			GameObject[] objs = GameObject.FindGameObjectsWithTag("Platform");
			float bestY = 0;
			int bestI = 0;
			for (int i = 0; i < objs.Length; i++) {
				if (objs[i].transform.position.y > bestY) {
					bestY = objs[i].transform.position.y;
					bestI = i;
				}
			}
			highestPlatform = objs[bestI].transform;
		}
		if (secondHighestPlatform == null) {
			secondHighestPlatform = highestPlatform;
		}
		Vector3 newPosition = new Vector3 (0, Mathf.Max(minY, target.position.y), transform.position.z);
		transform.position = Vector3.Lerp (transform.position, newPosition, Time.deltaTime);

		if (secondHighestPlatform.renderer.isVisible) {
			maxY = transform.position.y + Camera.main.rect.height/2;
			minY = transform.position.y - Camera.main.rect.height/2;
		}

		if (secondHighestPlatform == null || secondHighestPlatform.renderer.isVisible) {
			// spawn new ones
			float distToNextPlatform = secondHighestPlatform == null? normalPlatform.collider2D.bounds.size.y * 10 : 
				highestPlatform.position.y - secondHighestPlatform.position.y;
			float newY = highestPlatform == null? transform.position.y + distToNextPlatform : highestPlatform.position.y;
			Vector3 nextPos = new Vector3(Random.value * (maxXForSpawn - minXForSpawn) + minXForSpawn,
			                              newY + distToNextPlatform,
			                              0);
			Transform newPlatform = (Transform) Instantiate (normalPlatform, nextPos, Quaternion.identity);

			if (highestPlatform == null)
			{
				secondHighestPlatform = newPlatform;
			} else {
				secondHighestPlatform = highestPlatform;
			}
			highestPlatform = newPlatform;

			float rand = Random.value;

			if (rand < 0.3) {
				Vector3 boosterPos = new Vector3(newPlatform.position.x, 
				                                 booster.collider2D.bounds.extents.y + newPlatform.collider2D.bounds.extents.y + newPlatform.position.y,
				                                 0);
				Instantiate (booster, boosterPos, Quaternion.identity);
			} else if (rand < 0.7) {
				if (rand < 0.4) {
					Vector3 beanPos = new Vector3(Random.value * (maxXForSpawn - minXForSpawn) + minXForSpawn, 
				                                  newPlatform.collider2D.bounds.size.y*(Random.value * 10 + 3) + newPlatform.position.y,
				                                  0);
					Instantiate (bean, beanPos, Quaternion.identity);
				} else {
					int numObstacles = (int) Random.value * 3 + 2; // Between 2 and 5
					float x = newPlatform.position.x - newPlatform.collider2D.bounds.extents.x;
					for (int i = 0; i < numObstacles; i++) {
						Vector3 obstaclePos = new Vector3(x, 
						                                 obstacle.collider2D.bounds.extents.y + newPlatform.collider2D.bounds.extents.y + newPlatform.position.y,
						                                 0);
						Instantiate (obstacle, obstaclePos, Quaternion.identity);
						x += obstacle.collider2D.bounds.size.x + 1.0f;
					}
				}
			} else if (rand < 0.8) {
				float y = Random.value * (highestPlatform.position.y - secondHighestPlatform.position.y) + secondHighestPlatform.position.y;
				int numTimes = 0;
				while (numTimes < 5 && y < highestPlatform.position.y + highestPlatform.collider2D.bounds.extents.x &&
				       y > highestPlatform.position.y - highestPlatform.collider2D.bounds.extents.x) {
					y = Random.value * (highestPlatform.position.y - secondHighestPlatform.position.y) + secondHighestPlatform.position.y;
					numTimes ++;
				}
				if (numTimes < 5){
					// draw carrots in a line
					float xdist = ((CircleCollider2D) carrot.collider2D).radius * 2;
					float x = minXForSpawn + xdist;
					for (int i = 0; i < 4 && x < maxXForSpawn; i++) {
						Instantiate (carrot, new Vector3(x, y, 0), Quaternion.identity);
						x += xdist;
					}
				}
			} else if (rand < 0.9) {
				float x = Random.value * (maxXForSpawn - minXForSpawn) + minXForSpawn;
				float y = secondHighestPlatform.position.y + secondHighestPlatform.collider2D.bounds.extents.y;

				float ydist = ((CircleCollider2D) carrot.collider2D).radius * 2;
				for (int i = 0; i < 4 && y < highestPlatform.position.y - highestPlatform.collider2D.bounds.extents.y; i++) {
					Instantiate(carrot, new Vector3(x, y, 0), Quaternion.identity);
					y += ydist;
				}
			}

		}

	}
}
