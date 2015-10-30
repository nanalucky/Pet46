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
	public Button btnOrder;
	public Button[] btnRecords;
	public Button[] btnPlays;

	private float lookatBodyWeight;
	private float lookatHeadWeight;
	private float lookatEyeWeight;

	private Animator animator;

	public Vector3[] lookats = new Vector3[]{new Vector3(7f,0f,5f),new Vector3(5f,0f,6f),new Vector3(3.5f,0f,4f),new Vector3(5f,0f,4f)};

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		lookatBodyWeight = gameObject.GetComponent<LookAtIK> ().solver.bodyWeight;
		lookatHeadWeight = gameObject.GetComponent<LookAtIK> ().solver.headWeight;
		lookatEyeWeight = gameObject.GetComponent<LookAtIK> ().solver.eyesWeight;
	}

	void Update(){
		if (Input.GetKeyUp(KeyCode.A)) {
			gameObject.GetComponent<Animator>().Play("Blink", 2);	
			gameObject.GetComponent<Animator>().Play("Blink", 5);	
		}
	}

	public void EnableLookatIK(bool enabled)
	{
		LookAtIK lookatik = GameObject.FindGameObjectWithTag ("dog").GetComponent<LookAtIK> ();
		if (enabled) {
			lookatik.enabled = true;
			//Debug.LogWarning(string.Format("lookatid enable:{0}", Time.time));
		} else {
			lookatik.solver.bodyWeight = lookatBodyWeight;
			lookatik.solver.headWeight = lookatHeadWeight;
			lookatik.solver.eyesWeight = lookatEyeWeight;
			lookatik.Disable();
			StartCoroutine(PlayAnimInterval(2, 0.5f));
		}
	}

	IEnumerator PlayAnimInterval(int n, float time)
	{
		while (n > 0)
		{
			var anim = GameObject.FindGameObjectWithTag ("dog").GetComponent<Animator> ();
			if(anim.GetCurrentAnimatorStateInfo(2).IsName("Blink")){
				yield return new WaitForSeconds(0);
			} else {
				//Debug.LogWarning(string.Format("Blink:{0}", Time.time));
				anim.Play ("Blink", 2);
				anim.Play ("Blink", 5);
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

	public bool IsStand()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Stand") 
		    || animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle1") 
		    || animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle2")) {
			return true;
		}
		
		return false;
	}
	
	public void SitDown()
	{
		if (IsStand ()) {
			animator.Play ("SitDown");
		}
	}
	
	public void FallDown()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("SitIdle")) {
			animator.Play ("SitToFall");
		}
		else
		{
			if (IsStand ()) {
				animator.Play ("StandToFall");
			}
		}
	}
	
	public void StandUp()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("SitIdle")) {
			animator.Play ("SitToStand");
		}
		else if (animator.GetCurrentAnimatorStateInfo (0).IsName ("FallIdle")) {
			animator.Play ("FallToSit");
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("SleepIdle")) {
			animator.Play("Wake");
		}
	}
	
	public void RightRawUp()
	{
		if (IsStand ()) {
			animator.Play("RightRawUp");
		}else{
            StandUp();
            StartCoroutine("PlayRightRawUp");
		}
	}
    IEnumerator PlayRightRawUp()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
        {
			yield return null;
		}
        animator.Play("RightRawUp");
    }
	
	public void LeftRawUp()
	{
		if (IsStand ()) {
			animator.Play("LeftRawUp");
		}else{
            StandUp();
            StartCoroutine("PlayLeftRawUp");
		}
	}

    IEnumerator PlayLeftRawUp()
    {
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
		{
			yield return null;
		}
		animator.Play("LeftRawUp");
    }

	void ClearAll()
	{
		Time.fixedDeltaTime = 0.02f;

		// robot
		Destroy(GameObject.FindGameObjectWithTag("RobotScript"));

		// interact
		Destroy(GameObject.FindGameObjectWithTag("EnterInteract"));
		Destroy(GameObject.FindGameObjectWithTag("Interact"));


		// ball
		Destroy(GameObject.FindGameObjectWithTag("EnterBall"));
		Destroy(GameObject.FindGameObjectWithTag("Ball"));
		Destroy (GameObject.FindGameObjectWithTag ("Mouth").GetComponent<BallCollideMouth> ());
		EnableLookatIK (false);

		// exercise
		Destroy(GameObject.FindGameObjectWithTag("EnterExercise"));
		Destroy(GameObject.FindGameObjectWithTag("Exercise"));
		record.SetActive (false);

		// order
		Destroy(GameObject.FindGameObjectWithTag("EnterOrder"));
		Destroy(GameObject.FindGameObjectWithTag("Order"));
		record.SetActive (false);
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

	public void ToExercise()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/EnterExercise"));
	}

	public void ToOrder()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/EnterOrder"));
	}
}
