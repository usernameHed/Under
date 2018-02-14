using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

	public class FMODController : MonoBehaviour {

	public FMOD.Studio.EventInstance MusicTest;
	public FMOD.Studio.EventDescription Equal;
	public float EqualVal;
	// Use this for initialization
	void Start () {

		Equal = FMODUnity.RuntimeManager.GetEventDescription("event:/Music/MenuSolo125bpm");
		MusicTest = FMODUnity.RuntimeManager.CreateInstance("event:/Music/MenuSolo125bpm");
		Equal.createInstance (out MusicTest);

		//if (MusicTest.Get("Eq",out Equal) != FMOD.RESULT.OK)
		//{
		//	Debug.LogError ("Le parametre Eq n'existe pas sur cet event");
		//}
		//MusicTest.getValue (out EqualVal);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Keypad1)) 	//seulement une fois
		{			
			FMODUnity.RuntimeManager.PlayOneShot ("event:/SFX/Bouton");
		}
		if (Input.GetKey (KeyCode.Keypad2)) 		//en répétition
		{			
			FMODUnity.RuntimeManager.PlayOneShot ("event:/SFX/Egg");
		}
		if (Input.GetKeyDown (KeyCode.Keypad3)) 		
		{			
			MusicTest.start ();
		}
		if (Input.GetKeyDown (KeyCode.Keypad4)) 		
		{			
			EqualVal += Time.deltaTime;
			MusicTest.setParameterValue ("Equal",EqualVal);
			print (EqualVal);
		}
		if (Input.GetKeyDown (KeyCode.Keypad5)) 		
		{			
			EqualVal -= Time.deltaTime;
			MusicTest.setParameterValue ("Equal",EqualVal);
			print (EqualVal);
		}
		if (Input.GetKeyDown (KeyCode.Keypad6)) 		
		{			
			MusicTest.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}
}
