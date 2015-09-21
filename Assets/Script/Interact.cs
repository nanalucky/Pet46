using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.UI;

public class Interact : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject goHead = Instantiate (Resources.Load ("Prefabs/TouchHead")) as GameObject;
		goHead.transform.parent = gameObject.transform;

		GameObject goBack = Instantiate (Resources.Load ("Prefabs/TouchBack")) as GameObject;
		goBack.transform.parent = gameObject.transform;

		GameObject goLeftArm = Instantiate (Resources.Load ("Prefabs/GraspLeftArm")) as GameObject;
		goLeftArm.transform.parent = gameObject.transform;

		GameObject goRightArm = Instantiate (Resources.Load ("Prefabs/GraspRightArm")) as GameObject;
		goRightArm.transform.parent = gameObject.transform;

		GameObject goTail = Instantiate (Resources.Load ("Prefabs/GraspTail")) as GameObject;
		goTail.transform.parent = gameObject.transform;

		GameObject goLookCamera = Instantiate (Resources.Load ("Prefabs/LookCamera")) as GameObject;
		goLookCamera.transform.parent = gameObject.transform;

		gameObject.AddComponent<Gesture> ();

		DogController dogController = GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>();
		dogController.record.SetActive(true);
		foreach (Button btn in dogController.btnRecords) {
			btn.gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}


	void OnDestroy() {
		DogController dogController = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ();
		if (dogController != null) {
			dogController.record.SetActive(false);
			foreach (Button btn in dogController.btnRecords) {
				btn.gameObject.SetActive(false);
			}		
		}
	}

}
