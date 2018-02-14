using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriggerFmod : MonoBehaviour {


	[FMODUnity.EventRef]

	public string Music = "event:/Music/Clonage CenterCity 170bpm";
	public FMOD.Studio.EventInstance Zik;
	public FMOD.Studio.ParameterInstance Eggs;


	// Use this for initialization
	void Start () {
		Zik = FMODUnity.RuntimeManager.CreateInstance(Music);
		Zik.getParameter("Eggs", out Eggs);
		Zik.start();
	}

//	void OnTriggerEnter(Collider other)
//	{
	//	Eggs.setValue(1.0);
//	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			Eggs.setValue(1);
	}
}


//[FMODUnity.EventRef]
//public string NAME EVENT = “event:/EVENT”;
//public FMOD.Studio.EventInstance AUDIO EVENT;
//public FMOD.Studio.ParameterInstance PARAMETER EVENT;

//void Start () {
//	AUDIO EVENT = FMODUnity.RuntimeManager.CreateInstance(NAME EVENT);
//	AUDIO EVENT.getParameter(“NAME PARAMETER FMOD”, out PARAMETER EVENT);
//	AUDIO EVENT.start();
//}

}
