using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.UI;

public class DogController : MonoBehaviour {

	public Vector3 zoneMin = new Vector3(-3, 0, 0);
	public Vector3 zoneMax = new Vector3(3, 0, 7);
	public float walkSpeed = 0.85f;
	public float runSpeed = 1.8f;
	public float rushSpeed = 1.5f;

	public GameObject record;
	public Button[] btnRecords;

	private Quaternion eye1Rotation;
	private Vector3 eye1Position;
	private Quaternion eye2Rotation;
	private Vector3 eye2Position;

	// Use this for initialization
	void Start () {
		eye1Rotation = GameObject.Find ("Bone10").transform.rotation;
		eye1Position = GameObject.Find ("Bone10").transform.localPosition;
		eye2Rotation = GameObject.Find ("Bone11_").transform.rotation;
		eye2Position = GameObject.Find ("Bone11_").transform.localPosition;
	}

	public void ResetLookatIK()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<LookAtIK> ().Disable();
		//GameObject.Find ("Bone10").transform.rotation = eye1Rotation;
		//GameObject.Find ("Bone10").transform.localPosition = eye1Position;
		//GameObject.Find ("Bone11_").transform.rotation = eye2Rotation;
		//GameObject.Find ("Bone11_").transform.localPosition = eye2Position;
	}

	void ClearAll()
	{
		Destroy(GameObject.FindGameObjectWithTag("RobotScript"));

		Destroy(GameObject.FindGameObjectWithTag("EnterInteract"));
		Destroy(GameObject.FindGameObjectWithTag("Interact"));

		Destroy(GameObject.FindGameObjectWithTag("Ball"));
		Destroy (GameObject.FindGameObjectWithTag ("Mouth").GetComponent<BallCollideMouth> ());
		GameObject.FindGameObjectWithTag ("dog").GetComponent<LookAtIK> ().enabled = false;
	}

	public void ToRobot()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/RobotScript"));
	}

	public void ToInteract()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/EnterInteract"));
	}

	public void ToBall()
	{
		ClearAll ();

		GameObject go = Instantiate(Resources.Load("Prefabs/Ball")) as GameObject;
		go.GetComponent<Rigidbody>().isKinematic = true;
		go.transform.position = Camera.main.transform.position + Camera.main.transform.rotation * (new Vector3(0, 0, 0.4f + 0.08f));

		GameObject goPlay = Instantiate(Resources.Load("Prefabs/BallPlay")) as GameObject;
		goPlay.transform.parent = go.transform;

	}
	

}
