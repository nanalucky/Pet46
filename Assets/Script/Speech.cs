using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Speech : MonoBehaviour {

	private Button btn;
	private Word word;

	void Start () {
		GameObject goLookCamera = Instantiate (Resources.Load ("Prefabs/LookCamera")) as GameObject;
		goLookCamera.transform.parent = gameObject.transform;
		
		DogController dogController = GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>();
		dogController.btnPlay.gameObject.SetActive (false);
		dogController.btnSpeech.gameObject.SetActive (true);
		btn = dogController.btnSpeech;
	}
	
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnDestroy() {
		DogController dogController = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ();
		if (dogController != null) {
			dogController.btnPlay.gameObject.SetActive(true);
			dogController.btnSpeech.gameObject.SetActive (false);
		}
	}
}
