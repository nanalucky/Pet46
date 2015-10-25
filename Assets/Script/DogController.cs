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
	public Button btnRecord;
	public Button btnPlay;
	public Button[] btnRecords;
	public Button[] btnPlays;

	public Vector3[] lookats = new Vector3[]{new Vector3(7f,0f,5f),new Vector3(5f,0f,6f),new Vector3(3.5f,0f,4f),new Vector3(5f,0f,4f)};

	// Use this for initialization
	void Start () {
	}

	void Update(){
		if (Input.GetKeyUp(KeyCode.A)) {
			gameObject.GetComponent<Animator>().Play("Blink", 2);	
		}
	}

	public void EnableLookatIK(bool enabled)
	{
		LookAtIK lookatik = GameObject.FindGameObjectWithTag ("dog").GetComponent<LookAtIK> ();
		if (enabled) {
			lookatik.enabled = true;
			Debug.LogWarning(string.Format("lookatid enable:{0}", Time.time));
		} else {
			lookatik.Disable();
			StartCoroutine(PlayAnimInterval(2, 0.5f));
		}
	}

	IEnumerator PlayAnimInterval(int n, float time)
	{
		while (n > 0)
		{
			var anim = GameObject.FindGameObjectWithTag ("dog").GetComponent<Animator> ();
			if(anim.GetCurrentAnimatorStateInfo(1).IsName("Blink")){
				yield return new WaitForSeconds(0);
			} else {
				Debug.LogWarning(string.Format("Blink:{0}", Time.time));
				anim.Play ("Blink", 2);
				--n;
				yield return new WaitForSeconds(time);
			}
		}
	}

	public Vector3 ChooseLookat()
	{
		var goDog = GameObject.FindGameObjectWithTag ("dog");
		Vector3 nearest = lookats [0];
		float distance = (nearest - goDog.transform.position).magnitude;
		foreach (Vector3 pt in lookats) {
			float dis = (pt - goDog.transform.position).magnitude;
			if(dis > distance)
			{
				distance = dis;
				nearest = pt;
			}
		}
		ArrayList validLookats = new ArrayList();
		foreach (Vector3 pt in lookats) {
			if(pt != nearest)
			{
				validLookats.Add(pt);
			}
		}

		return (Vector3)(validLookats[(int)(Random.value * validLookats.Count)]);
	}

	void ClearAll()
	{
		Destroy(GameObject.FindGameObjectWithTag("RobotScript"));


		Destroy(GameObject.FindGameObjectWithTag("EnterInteract"));
		Destroy(GameObject.FindGameObjectWithTag("Interact"));
		btnRecord.interactable = false;
		btnPlay.interactable = false;
		foreach (Button btn in btnRecords) {
			btn.gameObject.SetActive(false);
		}		
		foreach (Button btn in btnPlays) {
			btn.gameObject.SetActive(false);
		}


		Destroy(GameObject.FindGameObjectWithTag("EnterBall"));
		Destroy(GameObject.FindGameObjectWithTag("Ball"));
		Destroy (GameObject.FindGameObjectWithTag ("Mouth").GetComponent<BallCollideMouth> ());
		EnableLookatIK (false);
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
		Instantiate(Resources.Load("Prefabs/EnterBall"));
	}
}
