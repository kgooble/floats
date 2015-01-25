using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public Text text;
	public Transform player;
	CharacterMovement script;

	float myScore;

	const float speed = 2.0f;

	void Start () 
	{
		text = GetComponent<Text> ();
		script = player.GetComponent<CharacterMovement> ();
		myScore = 0.0f;
	}
	
	void FixedUpdate () 
	{
		myScore = Mathf.Lerp (myScore, script.GetScore (), Time.deltaTime );
		if (Mathf.CeilToInt (myScore) != int.Parse (text.text)) 
		{
			text.text = "" + Mathf.FloorToInt (myScore);
		}
	}
}
