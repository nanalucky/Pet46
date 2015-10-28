using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonChangeSprite : MonoBehaviour {

	public string word;
	public Sprite saved;
	private bool bSaved = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectWithTag ("Record")) {
			Record record = GameObject.FindGameObjectWithTag ("Record").GetComponent<Record> ();
			if (record.HasProfile (word) != bSaved) {
				gameObject.GetComponent<Button>().image.sprite = saved;
				bSaved = !bSaved;
			}
		}
	}
}
