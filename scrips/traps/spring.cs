using UnityEngine;
using System.Collections;

public class spring : MonoBehaviour {
	
	public GameObject player;
	public CatControl catControl;
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		catControl = player.GetComponent<CatControl> ();
	}
	
	// Update is called once per frame
	void Update () {
	//	transform.Rotate (100 * Time.deltaTime, 100 * Time.deltaTime, 100 * Time.deltaTime);
	}
	
	
	void OnTriggerEnter(Collider collision){;
		catControl.springJump ();
		//	ContactPoint contact = collision.contacts [0];
		//Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
		//Vector3 pos = contact.point;
		//Destroy game object
		
		//player.GetComponent<prefabLoader> ().instantiateNext (transform.position);
		//trackCollectionScript.RecycleTrack(track);
		GameObject.Destroy (gameObject);
	}
}
