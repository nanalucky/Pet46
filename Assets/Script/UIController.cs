using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIController : MonoBehaviour {

	public void OnClickRobot()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToRobot ();
	}

	public void OnClickInteract()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToInteract ();
	}

	public void OnClickBall()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToBall ();
	}

	public void OnClickRecord()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToExercise ();
	}

	public void OnClickPlay()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToOrder ();
	}

	public void OnClickOrder()
	{

	}
	
	public void OnClickSitDownPlay()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().PlayProfile (Record.WORD_SITDOWN);
	}

	public void OnClickFallDownPlay()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().PlayProfile (Record.WORD_FALLDOWN);
	}

	public void OnClickStandUpPlay()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().PlayProfile (Record.WORD_STANDUP);
	}

	public void OnClickRightRawUpPlay()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().PlayProfile (Record.WORD_RIGHTRAWUP);
	}

	public void OnClickLeftRawUpPlay()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().PlayProfile (Record.WORD_LEFTRAWUP);
	}

	public void OnClickNoisePlay()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().PlayProfile (Record.WORD_NOISE);
	}

	public void OnClickLoad()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().ProfileLoad();
	}

	public void OnClickSave()
	{
		GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ().ProfileSave();
	}

	public void OnClickVolume()
	{
		Camera.main.GetComponent<AudioSource> ().mute = !Camera.main.GetComponent<AudioSource> ().mute; 
		//GameObject.FindGameObjectWithTag ("dog").GetComponent<AudioSource> ().enabled = !GameObject.FindGameObjectWithTag ("dog").GetComponent<AudioSource> ().enabled;
		Button btn = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().btnVolume.GetComponent<Button> ();
		if(!Camera.main.GetComponent<AudioSource>().mute)
			btn.image.sprite = Resources.Load<Sprite>("UI/volume");
		else
			btn.image.sprite = Resources.Load<Sprite>("UI/mute");
	}

	/*
	public void OnClick()
	{
		if (GameObject.FindGameObjectWithTag ("RobotScript") != null) {
			Destroy(GameObject.FindGameObjectWithTag("RobotScript"));
			Instantiate(Resources.Load("Prefabs/EnterInteract"));
		} else {
			Destroy(GameObject.FindGameObjectWithTag("EnterInteract"));
			Destroy(GameObject.FindGameObjectWithTag("Interact"));
			Instantiate(Resources.Load("Prefabs/RobotScript"));
		}
	}

	public void onClickBall()
	{
		GameObject go = GameObject.FindGameObjectWithTag ("Ball");
		if (go != null) {
			Destroy(go);
		} else {
			go = Instantiate(Resources.Load("Prefabs/Ball")) as GameObject;
			go.GetComponent<Rigidbody>().isKinematic = true;
			go.transform.position = Camera.main.transform.position + Camera.main.transform.rotation * (new Vector3(0, 0, 0.4f + 0.08f));
			GameObject goPlay = Instantiate(Resources.Load("Prefabs/BallPlay")) as GameObject;
		}
	}
	*/
}
