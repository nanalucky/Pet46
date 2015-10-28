using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Exercise : MonoBehaviour {

	public float timeDetect = 3.0f;
	private bool enable = false;
	private float timeStart = 0f;
	private Record record;

	void Start () {
		GameObject goLookCamera = Instantiate (Resources.Load ("Prefabs/LookCamera")) as GameObject;
		goLookCamera.transform.parent = gameObject.transform;
		
		DogController dogController = GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>();
		//dogController.record.SetActive(true);
		//dogController.record.GetComponent<Record> ().EnableDetectWords (false);
		foreach (Button btn in dogController.btnRecords) {
			btn.gameObject.SetActive (true);
		}
		
		record = GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ();
	}


	// Update is called once per frame
	void Update () {
		if (!enable && record.IsEnableDetectWords ()) {
			timeStart = Time.time;
			enable = true;
		}

		if (enable) {
			if(Time.time - timeStart >= timeDetect) {
				record.EnableDetectWords(false);
				timeStart = 0f;
				enable = false;
			}
		}
	}

	void OnDestroy() {
		DogController dogController = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ();
		if (dogController != null) {
			//dogController.record.SetActive(false);
			foreach (Button btn in dogController.btnRecords) {
				btn.gameObject.SetActive(false);
			}		
		}
	}
}
