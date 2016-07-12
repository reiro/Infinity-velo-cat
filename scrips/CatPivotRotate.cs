using UnityEngine;
using System.Collections;

public class CatPivotRotate : MonoBehaviour {

	GameObject player;
	GameManager gameManager;
	CatControl catControl;
	Vector3 rot;
	Quaternion rotation;
	float yRotation;


	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		gameManager = GameObject.Find ("GameController").GetComponent<GameManager>();
		catControl = player.GetComponent<CatControl> ();
	}
	
	void FixedUpdate () {
		if (gameManager.getTestControll() == true){
			//Debug.Log(Input.GetAxis("Vertical"));
			if (Input.GetAxis("Vertical") > 0.2f && Input.GetKey ("w")){
				yRotation = Input.GetAxis("Horizontal") / 15.0f;
			}else {
				yRotation = 0.0f;
			}

			Vector3 moveDirection = new Vector3 (yRotation, 0, Input.GetAxis("Vertical"));
			//Debug.Log(moveDirection);
			moveDirection = transform.TransformDirection (moveDirection);
			transform.position = player.transform.position;
			Vector3 lookDirection = moveDirection + player.transform.position;
			
			transform.LookAt (lookDirection);
			
		}
		if (gameManager.getTestControll() == false) {
			yRotation = -1 * catControl.getAngle();
			Vector3 moveDirection = new Vector3 (yRotation, 0, 1);
			moveDirection = transform.TransformDirection (moveDirection);
			transform.position = player.transform.position;
			Vector3 lookDirection = moveDirection + player.transform.position;
			
			transform.LookAt (lookDirection);
		}
	}

}
