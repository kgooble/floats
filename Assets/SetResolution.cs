using UnityEngine;
using System.Collections;

public class SetResolution : MonoBehaviour {

	void Awake()
	{
		Screen.SetResolution(410,656,false);
	}
}
