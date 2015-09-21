﻿using UnityEngine;
using System.Collections;

public class BallDogCamera : MonoBehaviour {

	public float minDistance = 1.0f;
	public float maxDistance = 3.0f;
	public float speedEulerX = 1.0f;

	private Camera mainCamera;
	private GameObject go;

	private float cameraHeight;
	private Vector3 lastLookat;
	private bool firstFrame = true;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		go = GameObject.FindGameObjectWithTag ("dog");
	}
	
	// Update is called once per frame
	void Update () {
		if (firstFrame) {
			firstFrame = false;

			Plane plane = new Plane( new Vector3(0, 1, 0), go.transform.position);
			Vector3 direction = mainCamera.transform.rotation * (new Vector3(0,0,1));
			Ray ray = new Ray(mainCamera.transform.position, direction);
			float rayDist;
			plane.Raycast(ray, out rayDist);
			lastLookat = ray.GetPoint(rayDist);
			
			cameraHeight = mainCamera.transform.position.y - go.transform.position.y;
		}

		bool inMove = false;
		if (lastLookat != go.transform.position) {
			lastLookat = go.transform.position;
			inMove = true;
		}
		
		float distance = (lastLookat - mainCamera.transform.position).magnitude;
		Vector3 euler = Quaternion.LookRotation((lastLookat - mainCamera.transform.position).normalized).eulerAngles;
		if (distance < minDistance || distance > maxDistance) {
			inMove = true;
			distance = minDistance;
			float eulerX = Mathf.Rad2Deg * Mathf.Asin(cameraHeight / distance);
			euler.x = Mathf.MoveTowardsAngle(mainCamera.transform.rotation.eulerAngles.x, eulerX, speedEulerX * Time.deltaTime);
			distance = cameraHeight / Mathf.Sin(Mathf.Deg2Rad * euler.x);
		}
		
		if (inMove) {
			mainCamera.transform.rotation = Quaternion.Euler(euler);
			mainCamera.transform.position = go.transform.position + mainCamera.transform.rotation * (new Vector3(0, 0, -distance));
		}
	}
}
