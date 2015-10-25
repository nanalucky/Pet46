using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class LookCamera : MonoBehaviour {

	private LookAtIK lookatIK;

	// Use this for initialization
	void Start () {
		GameObject goDog = GameObject.FindGameObjectWithTag ("dog");
		lookatIK = goDog.GetComponent<LookAtIK> ();
		goDog.GetComponent<DogController> ().EnableLookatIK (true);
	}
	
	// Update is called once per frame
	void Update () {
		lookatIK.solver.IKPosition = Camera.main.transform.position;
	}

	void OnDestroy () {
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().EnableLookatIK (false);
	}
}
