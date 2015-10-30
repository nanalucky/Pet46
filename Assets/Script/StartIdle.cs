using UnityEngine;
using System.Collections;

public class StartIdle : MonoBehaviour {

	public int minInterval = 1;
	public int maxInterval = 3;
	public string[] animations = new string[]{"Idle3", "Idle2"};
	public string stand = "Idle1";

	private float endTime;

	// Use this for initialization
	void Start () {
		endTime = Time.time + Random.Range (minInterval, maxInterval);
	}
	
	// Update is called once per frame
	void Update () {
		if(endTime > 0)
		{
			if(Time.time > endTime)
			{
				endTime = 0;
				GameObject.FindGameObjectWithTag("dog").GetComponent<Animator>().Play(animations[(int)(Random.value * 10.0f) % animations.Length], 0);
			}
		}
		else
		{
			if(GameObject.FindGameObjectWithTag("dog").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(stand))
			{
				endTime = Time.time + Random.Range (minInterval, maxInterval);
			}
		}
	}
}
