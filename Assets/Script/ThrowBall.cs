using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class ThrowBall : MonoBehaviour {

	public enum State
	{
		Grasp,
		None,
	}

	public State state = State.None;
	public Vector3 forceOrient = new Vector3(0, 1, 1);
	public float forceMultiplier = 0.05f;
	public float forceThreshold = 0;

	private GameObject go;
	private Collider co;
	private Rigidbody rb;
	private Vector3 startPosition;
	private Vector3 lastPosition;

	private GameObject goDog;
	private LookAtIK lookatIK;

	private bool firstFrame = true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (firstFrame) {
			firstFrame = false;
			go = GameObject.FindGameObjectWithTag ("Ball");
			co = go.GetComponent<SphereCollider> ();
			rb = go.GetComponent<Rigidbody> ();
			startPosition = go.transform.position;

			goDog = GameObject.FindGameObjectWithTag("dog");
			lookatIK = goDog.GetComponent<LookAtIK>();
			lookatIK.enabled = true;
		}

		lookatIK.solver.IKPosition = go.transform.position;

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		bool ret = co.Raycast (ray, out hit, 100.0f) && Input.GetMouseButton(0);

		switch(state)
		{
		case State.None:
			if(ret)
			{
				state = State.Grasp;
				go.transform.position = PetHelper.ProjectPointLine(go.transform.position, ray.GetPoint(0), ray.GetPoint(100.0f));
				lastPosition = Input.mousePosition;
			}
			break;
		case State.Grasp:
			if(Input.GetMouseButton(0))
			{
				go.transform.position = PetHelper.ProjectPointLine(go.transform.position, ray.GetPoint(0), ray.GetPoint(100.0f));
				lastPosition = Input.mousePosition;
			}
			else
			{
				Vector3 curPosition = Input.mousePosition;
				Vector3 delta = curPosition - lastPosition;
				float forceLen = delta.magnitude / Time.deltaTime * forceMultiplier;
				if(forceLen < forceThreshold)
				{
					state = State.None;
					go.transform.position = startPosition;
				}
				else
				{
					Vector3 force = (delta.x * (Quaternion.Inverse(Camera.main.transform.rotation) * (new Vector3(1, 0, 0))) + delta.y * (Quaternion.Inverse(Camera.main.transform.rotation) * forceOrient)).normalized * forceLen;
					rb.isKinematic = false;
					rb.AddForce(force);
					this.gameObject.GetComponent<BallCamera>().enabled = true;
					this.gameObject.GetComponent<BallDogController>().enabled = true;					
					this.enabled = false;
				}
			}
			break;
		}
	}
}
