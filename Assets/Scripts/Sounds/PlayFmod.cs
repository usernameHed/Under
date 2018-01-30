using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;



public class EggsTEST : MonoBehaviour {

	[FMODUnity.EventRef]


	public string inputSound = "event:/Egg";
	 

	void Start ()
	{
		
	}

	void OnCollisionEnter() 
	{
		FMODUnity.RuntimeManager.PlayOneShot (inputSound);

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			FMODUnity.RuntimeManager.PlayOneShot (inputSound);
		}
	
	}



}
