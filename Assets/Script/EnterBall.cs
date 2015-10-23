using UnityEngine;
using System.Collections;

public class EnterBall : EnterInteract {

	// Use this for initialization
	void Start () {
		// choose the farest lookat
		lookat = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ChooseLookat ();
		
		aiMoveCamera = new AIMoveCamera ();
		aiMoveCamera.Start (this);
	}
	
	// Update is called once per frame
	void Update () {
		aiMoveCamera.Update ();
		if (aiMoveCamera.IsFinished ()) 
		{
			// ball
			GameObject goBall = Instantiate(Resources.Load("Prefabs/Ball")) as GameObject;
			goBall.GetComponent<Rigidbody>().isKinematic = true;
			goBall.transform.position = Camera.main.transform.position + Camera.main.transform.rotation * (new Vector3(0, 0, 0.4f + 0.08f));
			
			// script
			GameObject goPlay = Instantiate(Resources.Load("Prefabs/BallPlay")) as GameObject;
			goPlay.transform.parent = goBall.transform;

			Destroy(gameObject);
			return;
		}
	}
}
