using UnityEngine;
using System.Collections;

public class SetResolution : MonoBehaviour {

	void Awake()
	{
		Screen.SetResolution(615,984,false);
	}
}
