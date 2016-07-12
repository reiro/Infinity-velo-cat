using UnityEngine;
using System.Collections;
using System;

public class CatControl : MonoBehaviour {
	public float maxInitialAcceleration = 20.0f;
	public float maxSpeed = 100.0f;
	public float curSpeed;
	public float speedVector = 0.0f;
	public float drag = 0.08f;
	public float gravity = 9.81f;
	public float jumpPower = 10f;
	public float rotateSpeed = 70.0f;
	public GameObject rotateCat;
	Vector3 previous;

	public CatStats stats;
	public Rigidbody rb;

	CatUnoConnect uno;
	//public GameObject bullet;
	public ParticleEmitter nitro;
	
	float currentSpeed;
	float currentAirSpeed;

	GameManager gameManager;
	GameObject pivotLookAt;
	
	private float currentRotationSunX;
	private float currentRotationSunY;
	public bool canControl;
	
	//public AudioClip bounceClip;
	public Transform rollSound;
	public Transform audioSkid;
	//public AudioClip rollSound;
	//public AudioClip jumpSound;
	//public AudioClip skid;
	
	//[System.NonSerialized]
	public bool brake;
	public bool useNitro;
	
	public int nitroValue = 1000;
	//[System.NonSerialized]
	public Vector3 moveDirection;
	
	// rotation angle
	public float currentAngle; // угол принимаемый с Arduino
	public float middleAngle = 0.0f; // угол отщёта (середина руля)
	private float rudderAngle; // На какой угол нужно повернуть - текущий угол поворота
	
	//acceleration
	public float powerLevel = 1.0f; // передача
	float capacity;
	float totalWork = 0.0f;
	float workA;
	public int turn; // 1-ка с Arduino
	public int firstAction;
	public int secondAction;
	bool acceleration = false; // ускорение если тру
	bool turnOff = false; // 1-ка и потом 0
	float frequency = 0.0f;
	float frequencyTime = 0.0f;
	float lastTurnTime = 0.0f;
	float accelerationTime = 0.0f;

	bool grounded = false;
	float groundedTimer = 0;

	float timer = 0;
	float current_velocity;
	float currentTotalWork = 0.0f;
	int scoreValue = 0;
	
	
	//public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
	
	void Start () {
		rb = GetComponent<Rigidbody>();
		pivotLookAt = GameObject.Find("pivotLookAt");
		gameManager = GameObject.Find ("GameController").GetComponent<GameManager>();
		audioSkid = transform.FindChild("Skid");
		rollSound = transform.FindChild("Rollsound");
		nitro.emit = false;
		currentSpeed = stats.speed;
		currentAirSpeed = stats.airSpeed;
		rotateCat = transform.Find("rotateCat").gameObject;
		lastTurnTime = Time.time;

		uno = GetComponent<CatUnoConnect> ();

		currentSpeed = maxInitialAcceleration;
	}
	
	void FixedUpdate () {
		current_velocity = getSpeed ();
		currentTotalWork = getTotalWork ();
		RotateY ();
		Accelerate ();
		RotateX ();
		PlayAudio ();
		Speedometr ();
		Action ();
		Nitro ();
		
		if (Input.GetMouseButtonDown (1)) {
			uno.PowerChange (1);
			if (gameManager.getTestControll() && powerLevel < 10) powerLevel += 1;
		}
		
		
		if (Input.GetMouseButtonDown (0)) {
		    uno.PowerChange (-1);
			if (gameManager.getTestControll() && powerLevel > 1) powerLevel -= 1;
		}

		if (Input.GetMouseButtonDown (0) && Input.GetMouseButtonDown (1)) {
			uno.CullerOnOff();
		}

		rb.AddForce(Physics.gravity * rb.mass * gravity); // Gravity 

	}
	
	void Update() {
		if (middleAngle == 0.0f) {
			middleAngle = currentAngle;
		}
		
		if (groundedTimer > 0) {
			groundedTimer = Mathf.Max(0,groundedTimer - Time.deltaTime);
			if (groundedTimer <= 0) {
				grounded = false;
			}
		}

		if (timer > 500) {
			//Game
		}
	}
	
	public void Accelerate(){

		if (gameManager.getTestControll() == false) {
			if (turn == 1) {
				acceleration = true;

			}
			if ((turn == 0) && (acceleration == true)) {
				accelerationTime += 0.5f;// / frequency;
				acceleration = false;
				
				frequencyTime = Time.time - lastTurnTime; // берём дельту времени между вращениями

				frequency = (1.0f / frequencyTime * 60.0f) / 2.0f; // частота вращения в минуту - 2 магнита
				//Debug.Log ("Частота: "+frequency+ "  " + lastTurnTime +"  "+frequencyTime);
				lastTurnTime = Time.time; // приваиваем время последнего врашения
			}

			if (accelerationTime > 0.0f) {
				accelerationTime -= Time.deltaTime;
				moveDirection = new Vector3 (0f, 0f, 1);
				//Debug.Log(accelerationTime);

				var vertical = 1.0f;
				var accel= maxInitialAcceleration * vertical;
				//Debug.Log(1.0f - (speedVector / maxSpeed));
				accel *= Mathf.Clamp(1.0f - (speedVector / maxSpeed), 0.0f, 1.0f);
				
				speedVector += accel * Time.deltaTime;
				speedVector +=  1 * accel / 1000.0f;
				
				moveDirection = new Vector3 (0f, 0f, speedVector);
			}
			if (accelerationTime <= 0.0f) {
				accelerationTime = 0.0f;
				moveDirection = new Vector3 (0f, 0f, speedVector);
				//moveDirection = new Vector3 (0f, 0f, 0f);
				//Debug.Log (accelerationTime+"4r33");
			}
		}
		if (gameManager.getTestControll() == true){
			var vertical = Input.GetAxis ("Vertical");
			var accel= maxInitialAcceleration * vertical;
			//Debug.Log(1.0f - (speedVector / maxSpeed));
			accel *= Mathf.Clamp(1.0f - (speedVector / maxSpeed), 0.0f, 1.0f);

			speedVector += accel * Time.deltaTime;
			speedVector += Input.GetAxis ("Vertical") * accel / 1000.0f;

			moveDirection = new Vector3 (0f, 0f, speedVector);
		}
		moveDirection = pivotLookAt.transform.TransformDirection (moveDirection);

		if (!grounded) {
			transform.Translate (moveDirection * stats.airSpeed * (1 + (powerLevel-1)/10.0f) * Time.deltaTime, Space.World);
			//rb.AddForce(Quaternion.Euler(0,90,0) *moveDirection*currentAirSpeed * (powerLevel / 5.0f + 1) * accel, ForceMode.Force);
		}
		if (grounded) {
			transform.Translate (moveDirection * stats.speed * (1 + (powerLevel-1)/10.0f) * Time.deltaTime, Space.World);
			//rb.AddForce(Quaternion.Euler(0,90,0) *moveDirection*currentSpeed * (powerLevel / 5.0f + 1) * accel, ForceMode.Force);
		}


		if (speedVector > 0){
			speedVector -= drag;
		}else{
			speedVector = 0;
		}

	}
	
	public void RotateY(){
		var rot = transform.eulerAngles; // Vector3 - rotation in degrees
		rot.x = 0;

		float vecTurn = pivotLookAt.transform.rotation.eulerAngles.y - rot.y;

		rot.y = pivotLookAt.transform.rotation.eulerAngles.y;
		if (rot.y > 60 && rot.y < 180) {
			rot.y = 60.0f;
			pivotLookAt.transform.eulerAngles = rot;
		} else if (rot.y < 300 && rot.y > 180) {
			rot.y = 300.0f;
			pivotLookAt.transform.eulerAngles = rot;
		}
		transform.eulerAngles = rot;
		//rb.velocity = Quaternion.AngleAxis(vecTurn, Vector3.up) * rb.velocity;
		
		//Debug.Log (rb.velocity + " " + transform.eulerAngles);
	}
	
	public void RotateX(){

		currentRotationSunX = rotateSpeed * current_velocity;

		//Debug.Log (currentRotationSunX);
		//transform.Rotate(Vector3.up * rotXko);

		rotateCat.transform.Rotate (Vector3.forward * currentRotationSunX * Time.deltaTime);
	}
	
	public void Jump() {
		if (grounded && canControl) {
			rb.AddForce(Vector3.up*stats.jumpHeight);
			//rb.velocity = new Vector3(0, 10, 0);
			if (transform.FindChild("Jumpsound") != null) {
				transform.FindChild("Jumpsound").GetComponent<AudioSource>().Play();
			} else {
				//Debug.Log("Jump sound failed to play");
			}
			grounded = false;
		}
	}
	
	public void Fire(){
		//var pos = Input.mousePosition;
		//pos.z = transform.position.z - Camera.main.transform.position.z;
		//pos = Camera.main.ScreenToWorldPoint (pos);
		
		//var q = Quaternion.FromToRotation (Vector3.up, pos - transform.position);
		//var go = Instantiate (bullet, transform.position, q);
		//go.rigid
	}

	public void Nitro(){
		if (useNitro && nitroValue > 0) {
			nitro.emit = true;
			//currentSpeed = stats.speed * 2.0f;
			//currentAirSpeed = stats.airSpeed * 2.0f;

			maxInitialAcceleration = currentSpeed * 1.5f;
			nitroValue -= 10;
		} else {
			nitro.emit = false;
			//currentSpeed = stats.speed;
			//currentAirSpeed = stats.airSpeed;
			maxInitialAcceleration = currentSpeed;
		}

		if (nitroValue < 1000 && current_velocity > 5 && ((secondAction != 1) || !Input.GetKey(KeyCode.LeftShift)) ) {
			nitroValue += 3;
		} else if (nitroValue > 1000) {
			nitroValue = 1000;
		}
	}
	
	public void Action(){
		//Debug.Log (secondAction);
		if (firstAction == 1 || Input.GetKey(KeyCode.Space)) {
			Jump();
		}
		if (secondAction == 1 || Input.GetKey(KeyCode.LeftShift)) {
			useNitro = true;
		} else {
			useNitro = false;
		}
	}
	
	public void PlayAudio(){
		if (rollSound != null) {
			if (grounded) {
				float rollSpd = Mathf.Clamp (GetComponent<Rigidbody> ().angularVelocity.magnitude / 3, 0, 16);
				//Debug.Log (rollSpd);
				rollSound.GetComponent<AudioSource> ().pitch = Mathf.Max (rollSpd, 0.8f);
				rollSound.GetComponent<AudioSource> ().volume = Mathf.Min (rollSpd, 1);
				rollSound.GetComponent<AudioSource>().Play();
			} else {
				if (rollSound.GetComponent<AudioSource> ().volume > 0) {
					rollSound.GetComponent<AudioSource> ().volume = Mathf.Max (0, rollSound.GetComponent<AudioSource> ().volume - 0.2f);
					rollSound.GetComponent<AudioSource>().Play();
				}
			}
		}
		
		if (audioSkid != null) {
			//	udio.PlayOneShot();;
			audioSkid.GetComponent<AudioSource>().PlayOneShot(audioSkid.GetComponent<AudioClip>(), 1.0F);
		}
		
	}

	// ======================== TRAPS ===================================

	public void decreaseSpeed(){
		rb.AddForce(Vector3.forward * (-2), ForceMode.Impulse);
	}

	public void increaseSpeed(){
		rb.AddForce(Vector3.forward * 2, ForceMode.Impulse);
	}

	public void springJump(){
		rb.AddForce(Vector3.up * 2, ForceMode.Impulse);
	}
	
// ================================================================

	public void Speedometr () {
		//var mph = getVSpeed() * 2.237;
		//var kph = rb.velocity.magnitude * 3.6;
		//speedometr.speed = Convert.ToSingle(mph);
		//var res = ((int)(kph * 100)) / 100f;
		//string spd = res.ToString() + " Км/ч";
	}
	
	
	
	// -------------------------- COLLISIONS ----------------------------------------------------------------------------
	
	void OnCollisionStay(Collision c) {
		if (c.gameObject.layer == 10) {
			grounded = true;
			groundedTimer = 0;
		}
	}
	
	void OnCollisionExit(Collision c) {
		if (c.gameObject.layer == 10) {
			groundedTimer = 0.08f;
		}
	}
	
	
	// -------------------------- GETTERs ----------------------------------------------------------------------------
	public float getAngle(){
		if (Mathf.Abs(middleAngle - currentAngle) > 5.0f) {
			rudderAngle = middleAngle - currentAngle;
		} else {
			rudderAngle = 0;
		}
		rudderAngle = rudderAngle / stats.angleKo;
		return rudderAngle;
	}
	
	public float getSpeed(){
		float velocity = (((transform.position - previous).magnitude) / Time.deltaTime) ;
		previous = transform.position;
		if (velocity < 0.01) velocity = 0.0f;
		return velocity;
		//return speedVector;
	}

	public float getTotalWork(){
		//frequency = 50;
		//capacity = (0.3f * (powerLevel / 10.0f + 1)  * frequency) / 965.0f;
		//workA = capacity * Time.deltaTime;
		//totalWork += workA / 4184.0f;
		//workA = 0.013f * 70.0f * (0.12f * 95.0f - 7.0f);
		if (gameManager.getTestControll()) frequency = 78.0f;
		totalWork += 0.3f * (frequency/60.0f) * (powerLevel/10.0f) * Time.deltaTime;
		Debug.Log (totalWork);
		return totalWork;

		//Э = 0,014 × М × (0,12 × П - 7) (ккал).
	}

	public Vector3 getMoveDirection(){
		return moveDirection; 
	}
	
	//------------------------- UI ---------------------------------------------------------------------------
	
	void OnGUI(){

		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 60;
		style.normal.textColor = new Color (0f, 219f, 219f, 1.0f);
		
		string powerString = "Передача: " + powerLevel.ToString ();
		GUI.Label (new Rect (5, 155,200,200), powerString, style);
		

		GUI.Label (new Rect(5, 5, Screen.width, 30), "Ускорение: " + nitroValue/10 + "%", style);
		GUI.Label (new Rect(5, 55, Screen.width, 30), "Частота: " + Math.Round(frequency) + "об/мин", style);
		GUI.Label (new Rect(5, 105, Screen.width, 30), "Калории: " + Math.Round(currentTotalWork) + "ккал", style);

		GUI.Label (new Rect(50, 225, Screen.width, 30), "Скорость: " + Math.Round(current_velocity, 2) + "км/ч", style);
	
	}
	
}
