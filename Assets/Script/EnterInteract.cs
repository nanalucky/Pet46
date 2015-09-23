﻿using UnityEngine;
using System.Collections;

// sit down & move camera & run to camera
public class EnterInteract : MonoBehaviour {

	public class AI
	{
		protected EnterInteract controller;
		
		public virtual void Start(EnterInteract ctrl){}
		public virtual void Update(){}
		public virtual bool IsFinished(){return true;}
		public virtual void Finish(){}
		public virtual AIState GetNextState(){return AIState.None;}
	}

	public class AIMoveCamera : AI
	{
		private Camera mainCamera;
		private GameObject go;

		private bool cameraMove;
		private float velLookat = 0.0f;
		private float velX = 0.0f;
		private float velY = 0.0f;
		private float velDistance;
		private Vector3 curLookat;
		private float dstEulerY;
		private float curDistance;

		private float dstDistance;
		private Vector3 dstLookat;
		private float dstEulerX;

		public override void Start(EnterInteract ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag("dog");
			mainCamera = Camera.main;

			// camera
			cameraMove = true;
			Plane plane = new Plane( new Vector3(0, 1, 0), go.transform.position);
			Vector3 direction = mainCamera.transform.rotation * (new Vector3(0,0,1));
			Ray ray = new Ray(mainCamera.transform.position, direction);
			float rayDist;
			plane.Raycast(ray, out rayDist);
			curLookat = ray.GetPoint(rayDist);
			curDistance = (curLookat - mainCamera.transform.position).magnitude;

			direction = (go.transform.position - ctrl.lookat).normalized;
			if (direction != Vector3.zero)
				dstEulerY = Quaternion.LookRotation (direction).eulerAngles.y;
			else
				dstEulerY = go.transform.rotation.eulerAngles.y + 180;

			Vector3 euler = new Vector3 (controller.eulerX, dstEulerY, mainCamera.transform.rotation.eulerAngles.z);
			Vector3 cameraPos = controller.lookat + Quaternion.Euler (euler) * new Vector3(0, 0, -controller.distance);
			dstDistance = (go.transform.position - cameraPos).magnitude;
			dstLookat = go.transform.position;
			dstEulerX = Quaternion.LookRotation ((go.transform.position - cameraPos).normalized).eulerAngles.x;
		}

		public override void Update()
		{
			if (cameraMove == true) 
			{
				Vector3 direction = (dstLookat - go.transform.position).normalized;
				float curLookatDistance = Mathf.SmoothDamp((curLookat - dstLookat).magnitude,
				                                           0.0f, ref velLookat, controller.lookatSmooth);
				curLookat = dstLookat - direction * curLookatDistance;
				curDistance = Mathf.SmoothDamp(curDistance, dstDistance, ref velDistance, controller.distanceSmooth);

				Vector3 euler = mainCamera.transform.rotation.eulerAngles;
				euler.x = Mathf.SmoothDampAngle(euler.x, dstEulerX, ref velX, controller.eulerXSmooth);
				euler.y = Mathf.SmoothDampAngle(euler.y, dstEulerY, ref velY, controller.eulerYSmooth);

				mainCamera.transform.rotation = Quaternion.Euler(euler);
				mainCamera.transform.position = curLookat + mainCamera.transform.rotation * (new Vector3(0, 0, -curDistance));

				curLookatDistance = (curLookat - dstLookat).magnitude;
				if (curLookatDistance < 0.1f 
				    && Mathf.Abs(euler.y - dstEulerY) < 0.1f 
				    && Mathf.Abs(euler.x - dstEulerX) < 0.1f
				    && Mathf.Abs(curDistance - dstDistance) < 0.1f)
				{
					cameraMove = false;
				}
			}
		}

		public override bool IsFinished()
		{
			return !cameraMove;
		}

		public override AIState GetNextState()
		{
			return AIState.Turn;
		}
	}

	public class AITurn : AI
	{
		private GameObject go;

		private bool inMove = false;
		private float dstEulerY;

		public override void Start(EnterInteract ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");

			inMove = true;
			Vector3 direction = (controller.lookat - go.transform.position).normalized;
			if (direction != Vector3.zero)
				dstEulerY = Quaternion.LookRotation (direction).eulerAngles.y;
			else
				dstEulerY = go.transform.rotation.eulerAngles.y;

			go.GetComponent<Animator> ().Play ("Walk");
		}

		public override void Update()
		{
			if (inMove) 
			{
				Vector3 euler = go.transform.rotation.eulerAngles;
				euler.y = Mathf.MoveTowardsAngle(euler.y, dstEulerY, Time.deltaTime * controller.turnEulerYSpeed);
				if(Mathf.Abs(dstEulerY - euler.y) < 0.1f)
				{
					inMove = false;
					euler.y = dstEulerY;
				}
				go.transform.rotation = Quaternion.Euler(euler);
			}
		}

		public override bool IsFinished ()
		{
			return !inMove;
		}

		public override AIState GetNextState()
		{
			return AIState.Stay;
		}
	}

	public class AIStay : AI
	{
		private GameObject go;
		private float endTime;

		public override void Start(EnterInteract ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			go.GetComponent<Animator> ().Play ("WalkToStand");
			endTime = Time.time + ctrl.stayTime;
		}

		public override bool IsFinished()
		{
			if (!controller.aiMoveCamera.IsFinished ())
				return false;

			return endTime <= Time.time;
		}

		public override AIState GetNextState()
		{
			if (go.transform.position == controller.lookat)
				return AIState.Sit;

			return AIState.Run;
		}
	}

	public class AIRun : AI
	{
		private Camera mainCamera;
		private GameObject go;

		private bool inMove;
		private Vector3 startPosition;
		private float time;

		public override void Start(EnterInteract ctrl)
		{
			controller = ctrl;
			mainCamera = Camera.main;
			go = GameObject.FindGameObjectWithTag ("dog");

			inMove = true;
			go.GetComponent<Animator> ().Play ("Run");
			startPosition = go.transform.position;
			time = 0.0f;
		}

		public override void Update()
		{
			if (!inMove)
				return;

			time = time + Time.deltaTime;
			go.transform.position = startPosition + go.transform.rotation * (new Vector3 (0, 0, time * go.GetComponent<DogController> ().runSpeed));
			if ((go.transform.position - startPosition).magnitude > (controller.lookat - startPosition).magnitude) 
			{
				inMove = false;
				go.transform.position = controller.lookat;
			}

			mainCamera.transform.rotation = Quaternion.LookRotation ((go.transform.position - mainCamera.transform.position).normalized);
		}

		public override bool IsFinished()
		{
			return !inMove;
		}

		public override AIState GetNextState()
		{
			return AIState.Sit;
		}
	}

	public class AISit : AI
	{
		private GameObject go;

		public override void Start(EnterInteract ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			go.GetComponent<Animator> ().Play ("SitDown");
		}

		public override bool IsFinished()
		{
			if (go.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("SitIdle"))
				return true;

			return false;
		}

		public override AIState GetNextState()
		{
			return AIState.None;
		}
	}

	public enum AIState
	{
		MoveCamera,
		Turn,
		Stay,
		Run,
		Sit,
		None,
	}

	public Vector3 lookat = new Vector3 (0, 0, 3.5f);
	public float lookatSmooth = 0.5f;
	public float distance = 1.0f;
	public float distanceSmooth = 0.5f;
	public float eulerYSmooth = 0.5f;
	public float eulerX = 30.0f;
	public float eulerXSmooth = 0.5f;

	public float turnEulerYSpeed = 480.0f;
	public float stayTime = 1.0f;

	public AI aiMoveCamera;
	private AI lastAI;
	
	// Use this for initialization
	void Start () {
		aiMoveCamera = new AIMoveCamera ();
		aiMoveCamera.Start (this);
		lastAI = new AITurn ();
		lastAI.Start (this);
	}
	
	// Update is called once per frame
	void Update () {
		aiMoveCamera.Update ();
		lastAI.Update ();
		if (lastAI.IsFinished ()) 
		{
			ChangeAI();
		}
	}
	
	void ChangeAI()
	{
		AIState state = lastAI.GetNextState ();
		lastAI.Finish ();

		if (state == AIState.None)
		{
			Instantiate(Resources.Load("Prefabs/Interact"));
			Destroy(gameObject);
			return;
		}

		AI ai;
		switch (state) {
		case AIState.Stay:
			ai = new AIStay();
			break;
		case AIState.Run:
			ai = new AIRun();
		break;
		case AIState.Sit:
			ai = new AISit();
			break;
		default:
			ai = new AISit();
			break;
		}

		ai.Start(this);
		lastAI = ai;
	}


}