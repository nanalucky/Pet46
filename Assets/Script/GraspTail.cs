using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class GraspTail : MonoBehaviour {

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

	private Camera mainCamera;
	private GameObject go;
	private Collider co;
	private SkinnedCollisionHelper skinHelper;
	
	private CCDIK ccdIK;
	private RotationLimit[] rotationLimits;
	private float lastTouchTime;


	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		go = GameObject.Find (partName);
		go.AddComponent<MeshCollider> ();
		skinHelper = go.AddComponent<SkinnedCollisionHelper> ();
		co = go.GetComponent<MeshCollider> ();
		
		state = State.None;
		ccdIK = go.AddComponent<CCDIK> ();
		ccdIK.solver.SetChain (new Transform[]{GameObject.Find (boneNames [0]).transform, 
		                        GameObject.Find (boneNames [1]).transform, 
								GameObject.Find (boneNames [2]).transform}, 
		                        GameObject.Find (rootName).transform);
		ccdIK.solver.IKPositionWeight = 0.0f;
		rotationLimits = GameObject.Find (boneNames [0]).GetComponentsInChildren<RotationLimit> ();
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
					ccdIK.solver.IKPositionWeight = 1.0f;
					ccdIK.solver.IKPosition = ccdIK.solver.bones[ccdIK.solver.bones.Length - 1].transform.position;
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
				ccdIK.solver.IKPositionWeight = 0.0f;
			}
			else
			{
				Ray rayCur = mainCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 posCur = PetHelper.ProjectPointLine(ccdIK.solver.IKPosition, rayCur.GetPoint(0), rayCur.GetPoint(100));
				ccdIK.solver.IKPosition = posCur;
			}
			break;
		}
	}

	void LateUpdate()
	{
		foreach (RotationLimit rl in rotationLimits) {
			rl.Apply();
		}
	}
	
	void OnDestroy() {
		go = GameObject.Find (partName);
		if (go != null) {
			Destroy (go.GetComponent<MeshCollider> ());
			Destroy (go.GetComponent<SkinnedCollisionHelper> ());
			Destroy (go.GetComponent<CCDIK> ());
		}
	}

}
