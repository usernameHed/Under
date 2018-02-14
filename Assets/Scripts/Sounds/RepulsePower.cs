using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class RepulsePower : MonoBehaviour {

	[FMODUnity.EventRef]

	public string inputSound = "event:/Repulse";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			FMODUnity.RuntimeManager.PlayOneShot (inputSound);
		}

	}

}
