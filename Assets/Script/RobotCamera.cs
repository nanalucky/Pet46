using UnityEngine;
using System.Collections;

public class RobotCamera : MonoBehaviour {

	public Camera mainCamera;
	public float minDistance = 1.0f;
	public float maxDistance = 2.0f;
	public float distanceSmooth = 0.5f;
	public float lookatSmooth = 0.5f;

	public float X_Smooth = 0.5f;
	public float Y_Smooth = 1.0f;
	public float eulerX = 13.8f;

	public float areaBorder = 0.2f;

	private bool inMove = false;
	private float dstDist = 0.0f;
	private float velDist = 0.0f;
	private Vector3 dstEuler = Vector3.zero;
	private float velX = 0.0f;
	private float velY = 0.0f;
	private Vector3 dstLookat = Vector3.zero;
	private Vector3 lastLookat = Vector3.zero;
	private float velLookat = 0.0f;

	private GameObject go;


	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		go = GameObject.FindGameObjectWithTag ("dog");
	}
	
	// Update is called once per frame
	void Update () {
		if (!inMove)
		{
			Vector3 viewPos = mainCamera.WorldToViewportPoint(go.transform.position);
			if( viewPos.x < areaBorder || viewPos.x > 1.0f - areaBorder || viewPos.y < areaBorder || viewPos.y > 1.0f - areaBorder )
				inMove = true;
			if(!inMove)
			{
				float distance = (mainCamera.transform.position - go.transform.position).magnitude;
				if (distance > maxDistance)
					inMove = true;
			}
			if(!inMove)
			{
				Vector3 euler = mainCamera.transform.rotation.eulerAngles;
				if (Mathf.Abs(euler.x - eulerX) > 0.1f)
					inMove = true;
			}
			
			if(inMove)
			{
				velDist = 0.0f;
				velLookat = 0.0f;
				velX = 0.0f;
				velY = 0.0f;
				
				Plane plane = new Plane( new Vector3(0, 1, 0), go.transform.position);
				Vector3 direction = mainCamera.transform.rotation * (new Vector3(0,0,1));
				Ray ray = new Ray(mainCamera.transform.position, direction);
				float rayDist;
				plane.Raycast(ray, out rayDist);
				lastLookat = ray.GetPoint(rayDist);
				dstLookat = go.transform.position;
				
				float curDist = (mainCamera.transform.position - lastLookat).magnitude;
				dstDist = Mathf.Clamp(curDist, minDistance, maxDistance);
				dstDist = Random.Range(minDistance, dstDist);
				if(dstDist == curDist)
					dstDist = 0.0f;
				
				Vector3 curEuler = mainCamera.transform.rotation.eulerAngles;
				Vector3 euler = go.transform.rotation.eulerAngles;
				dstEuler = new Vector3(curEuler.x, euler.y, curEuler.z);
				if(dstEuler == curEuler)
					dstEuler = Vector3.zero;
			}
		}
		
		if (inMove) 
		{
			dstLookat = go.transform.position;
			float dist = (dstLookat - lastLookat).magnitude;
			dist = Mathf.SmoothDamp(0, dist, ref velLookat, lookatSmooth);
			lastLookat = lastLookat + (dstLookat - lastLookat).normalized * dist;

			float distance = (mainCamera.transform.position - lastLookat).magnitude;
			distance = Mathf.SmoothDamp(distance, dstDist, ref velDist, distanceSmooth);
			
			Vector3 euler = mainCamera.transform.rotation.eulerAngles;
			dstEuler = euler;
			dstEuler.x = eulerX;
			dstEuler.y = go.transform.rotation.eulerAngles.y;
			if(dstEuler != Vector3.zero)
			{
				euler.x = Mathf.SmoothDampAngle(euler.x, dstEuler.x, ref velX, X_Smooth);
				euler.y = Mathf.SmoothDampAngle(euler.y, dstEuler.y, ref velY, Y_Smooth);
				euler.z = 0.0f;
			}
			
			Vector3 direction = new Vector3(0, 0, -distance);
			Quaternion rotation = Quaternion.Euler(euler);
			mainCamera.transform.position = lastLookat + rotation * direction;
			mainCamera.transform.rotation = rotation;

			if ((dstLookat - lastLookat).magnitude < 0.1f 
			    && Mathf.Abs(dstDist - distance) < 0.1f
			    && (Mathf.Abs(dstEuler.x - euler.x) < 0.1f && Mathf.Abs(dstEuler.y - euler.y) < 0.1f))
			{
				inMove = false;
			}		

			RaycastHit hit;
			if(Physics.Raycast(lastLookat, (mainCamera.transform.position - lastLookat).normalized, out hit, (mainCamera.transform.position - lastLookat).magnitude))
			{
				mainCamera.transform.position = hit.point;
			}
		}
	}
}
