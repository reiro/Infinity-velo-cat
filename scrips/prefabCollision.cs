using UnityEngine;
using System.Collections;

public class prefabCollision : MonoBehaviour {

	GameObject player;

	void Start () {
		player = GameObject.Find ("Player");
	}

	void OnTriggerEnter(Collider collision){
	//	ContactPoint contact = collision.contacts [0];
		//Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
		//Vector3 pos = contact.point;
		//Destroy game object
		//Instantiae new
		player.GetComponent<prefabLoader> ().instantiateNext (transform.position);
	}

}
