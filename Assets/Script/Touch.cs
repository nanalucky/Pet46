using UnityEngine;
using System.Collections;

public class Touch : MonoBehaviour {

	public enum State
	{
		Touch,
		Enjoy,
		NotTouch,
		None,
	}

	public string partName;
	public string animationName;
	public float maxStillTime = 1.0f;
	public float timeIntoEnjoy = 1.0f;
	public float timeOutEnjoy = 1.0f;
	public State state;

	private Camera mainCamera;
	private GameObject goDog;
	private GameObject go;
	private Collider co;
	private SkinnedCollisionHelper skinHelper;

	private float timeInTouch;
	private float timeNotInTouch;
	private Vector3 lastPosition;
	private float lastPositionTime;

	private bool lastMouseDown = false;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		goDog = GameObject.FindGameObjectWithTag ("dog");
		go = GameObject.Find (partName);
		go.AddComponent<MeshCollider> ();
		skinHelper = go.AddComponent<SkinnedCollisionHelper> ();
		skinHelper.updateOncePerFrame = false;
		co = go.GetComponent<MeshCollider> ();

		timeInTouch = 0.0f;
		timeNotInTouch = 0.0f;
		lastPosition = Vector3.zero;
		lastPositionTime = 0.0f;
	}

	bool InTouch()
	{
		if (lastPosition == Vector3.zero || Input.mousePosition != lastPosition) 
		{
			lastPosition = Input.mousePosition;
			lastPositionTime = Time.time;
			return true;
		}

		if (Time.time - lastPositionTime <= maxStillTime)
			return true;

		return false;
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		bool ret;// = co.Raycast (ray, out hit, 100.0f) && Input.GetMouseButton(0);
		switch (state) 
		{
		case State.None:
			ret = false;
			if(!lastMouseDown && Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}
			lastMouseDown = Input.GetMouseButton(0);

			if(ret)
			{
				timeInTouch = Time.time;
				state = State.Touch;
				lastPosition = Input.mousePosition;
				lastPositionTime = Time.time;
			}
			break;
		case State.Touch:
			ret = false;
			if(Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}

			if(ret && InTouch())
			{
				if(Time.time - timeInTouch >= timeIntoEnjoy)
				{
					state = State.Enjoy;
					if(string.Compare(animationName, "TouchHead") == 0)
					{
						if(goDog.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SitIdle"))
							goDog.GetComponent<Animator>().Play("TouchHeadHeadForSit", 1);
						else
							goDog.GetComponent<Animator>().Play ("TouchHeadHead", 1);
					}
					else
					{
						if(goDog.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SitIdle"))
							goDog.GetComponent<Animator>().Play ("TouchBackHeadForSit", 1);
						else
							goDog.GetComponent<Animator>().Play ("TouchBackHead", 1);
					}
				}
			}
			else
			{
				state = State.None;
			}
			break;
		case State.Enjoy:
			ret = false;
			if(Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}

			if(!(ret && InTouch ()))
			{
				state = State.NotTouch;
				timeNotInTouch = Time.time;
			}
			break;
		case State.NotTouch:
			ret = false;
			if(Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}

			if(ret && InTouch())
			{
				state = State.Enjoy;
			}
			else
			{
				if(Time.time - timeNotInTouch > timeOutEnjoy)
				{
					state = State.None;
					if(string.Compare(animationName, "TouchHead") == 0)
					{
						goDog.GetComponent<Animator>().Play("empty", 1);
					}
					else
					{
						goDog.GetComponent<Animator>().Play ("empty", 1);
					}
				}
			}
			break;
		}
	}

	void OnDestroy() {
		go = GameObject.Find (partName);
		if (go != null) {
			Destroy (go.GetComponent<MeshCollider> ());
			Destroy (go.GetComponent<SkinnedCollisionHelper> ());
		}
	}
}
