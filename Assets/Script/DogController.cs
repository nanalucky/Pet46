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

	// Use this for initialization
	void Start () {
	}

	public void ResetLookatIK()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<LookAtIK> ().Disable();
		StartCoroutine(PlayAnimInterval(2, 0.5f));
	}

	IEnumerator PlayAnimInterval(int n, float time)
	{
		while (n > 0)
		{
			var anim = GameObject.FindGameObjectWithTag ("dog").GetComponent<Animator> ();
			if(anim.GetCurrentAnimatorStateInfo(1).IsName("Blink")){
				yield return new WaitForSeconds(0);
			} else {
				anim.Play ("Blink", 1);
				--n;
				yield return new WaitForSeconds(time);
			}
		}
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
