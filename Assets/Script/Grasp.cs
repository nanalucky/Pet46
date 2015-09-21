using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class Grasp : MonoBehaviour {

	public enum State
	{
		Touch,
		Grasp,
		None,
	}


	public State state;
	public string partName;
	public string[] boneNames;
	public string rootName;
	public float touchTime = 1.0f;
	public Vector3 minOffset = new Vector3 (0.0f, 0.0f, 0.0f);
	public Vector3 maxOffset = new Vector3 (0.0f, 0.15f, 0.15f);

	private Camera mainCamera;
	private GameObject goDog;
	private GameObject go;
	private Collider co;
	private SkinnedCollisionHelper skinHelper;

	private LimbIK limbIK;
	private float lastTouchTime;
	private Vector3 firstPosition;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		goDog = GameObject.FindGameObjectWithTag ("dog");
		go = GameObject.Find (partName);
		go.AddComponent<MeshCollider> ();
		skinHelper = go.AddComponent<SkinnedCollisionHelper> ();
		co = go.GetComponent<MeshCollider> ();

		state = State.None;
		limbIK = go.AddComponent<LimbIK> ();
		limbIK.solver.SetChain (GameObject.Find (boneNames [0]).transform, 
		                        GameObject.Find (boneNames [1]).transform, 
		                        GameObject.Find (boneNames [2]).transform, 
		                        GameObject.Find (rootName).transform);
		limbIK.solver.IKPositionWeight = 0.0f;
		limbIK.solver.IKRotationWeight = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		skinHelper.forceUpdate = true;

		Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		bool ret = co.Raycast (ray, out hit, 100.0f) && Input.GetMouseButton(0);
		switch (state) {
		case State.None:
			if(ret)
			{
				lastTouchTime = Time.time;
				state = State.Touch;
			}
			break;
		case State.Touch:
			if(ret)
			{
				if(Time.time - lastTouchTime >= touchTime)
				{
					state = State.Grasp;
					limbIK.solver.IKPositionWeight = 1.0f;
					limbIK.solver.IKRotationWeight = 0.5f;
					limbIK.solver.IKPosition = hit.point;
					firstPosition = hit.point;
				}
			}
			else
			{
				state = State.None;
			}
			break;
		case State.Grasp:
			if(!Input.GetMouseButton(0))
			{
				state = State.None;
				limbIK.solver.IKPositionWeight = 0.0f;
				limbIK.solver.IKRotationWeight = 0.0f;
			}
			else
			{
				Ray rayCur = mainCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 posCur = PetHelper.ProjectPointLine(limbIK.solver.IKPosition, rayCur.GetPoint(0), rayCur.GetPoint(100));
				Vector3 firstInLocal = Quaternion.Inverse(goDog.transform.rotation) * firstPosition;
				Vector3 curInLocal = Quaternion.Inverse(goDog.transform.rotation) * posCur;
				curInLocal.x = Mathf.Clamp(curInLocal.x, firstInLocal.x + minOffset.x, firstInLocal.x + maxOffset.x);
				curInLocal.y = Mathf.Clamp(curInLocal.y, firstInLocal.y + minOffset.y, firstInLocal.y + maxOffset.y);
				curInLocal.z = Mathf.Clamp(curInLocal.z, firstInLocal.z + minOffset.z, firstInLocal.z + maxOffset.z);
				Vector3 curInWorld = goDog.transform.rotation * curInLocal;
				limbIK.solver.IKPosition = curInWorld;
			}
			break;
		}
	}

	void OnDestroy() {
		go = GameObject.Find (partName);
		if (go != null) {
			Destroy (go.GetComponent<MeshCollider> ());
			Destroy (go.GetComponent<SkinnedCollisionHelper> ());
			Destroy (go.GetComponent<LimbIK> ());
		}
	}
}
