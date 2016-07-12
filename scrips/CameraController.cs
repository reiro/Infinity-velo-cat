using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform target;

	[System.Serializable]
	public class PositionSettings
	{
		public Vector3 targetPosOffset = new Vector3 (0, 3.4f, 0);
		public float distanceFromTarget = -8;
		public float zoomSmooth = 100;
		public float zoomStep = 2;
		public float maxZoom = -2;
		public float minZoom = -15;
		public bool smoothFollow = true;
		public float smooth = 0.05f;
		
		[HideInInspector]
		public float newDistance = -8;
		[HideInInspector]
		public float adjustmentDistance = -8;
	}

	[System.Serializable]
	public class OrbitSettings{
		public float xRotation = -20;
		public float yRotation = -180;
		public float maxXRotation = -25;
		public float minXRotation = -50;
		public float vOrbitSmooth = 0.5f;
		public float hOrbitSmooth = 0.5f;
	}

	[System.Serializable]
	public class InputSettings{
		public string MOUSE_ORBIT = "MouseOrbit";
		public string MOUSE_ORBIT_VERTICAL = "MouseOrbitVertical";
		public string ORBIT_HORISONTAL_SNAP = "OrbitHorisontalSnap";
		public string ORBIT_HORISONTAL = "OrbitHorisontal";
		public string ORBIT_VERTICAL = "OrbitVertical";
		public string ZOOM = "Mouser ScrolLWheel";
	}

	[System.Serializable]
	public class DebugSettings
	{
		public bool drawDesiredCollisionLines = true;
		public bool drawAdjustedCollisionLines = true;
	}

	public PositionSettings position = new PositionSettings();
	public OrbitSettings orbit = new OrbitSettings();
	//public InputSettings input = new InputSettings();
	public DebugSettings debug = new DebugSettings ();
	public CollisionHandler collision = new CollisionHandler ();

	Vector3 targetPos = Vector3.zero;
	Vector3 destination = Vector3.zero;
	Vector3 adjustedDestination = Vector3.zero;
	Vector3 camVel = Vector3.zero;
	float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput, mouseOrbitInput, vMouseOrbitInput;
	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;
//	public UnoConnect uno;
	public GameObject player;
	/*
	public Camera attachedCamera;
	public Vector3 defaultCameraPosition = new Vector3(0,2.8f,6);
	public float cameraDistance = 1;
	public float cameraDistanceTarget = 1;
	float h;
	float v;



	[SerializeField] public float distance = 3.0f;
	[SerializeField] public float height = 1.0f;
	[SerializeField] public float damping = 5.0f;
	[SerializeField] public bool smoothRotation = true;
	[SerializeField] public float rotationDamping = 10.0f;

	[SerializeField] public Vector3 targetLookAtOffset; // allows offsetting of camera lookAt, very useful for low bumper heights

	[SerializeField] public float bumperDistanceCheck = 2.5f; // length of bumper ray
	[SerializeField] public float bumperCameraHeight = 1.0f; // adjust camera height while bumping
	[SerializeField] public Vector3 bumperRayOffset; // allows offset of the bumper ray from target origin

	*/

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		//uno = player.GetComponent<UnoConnect> ();
		
		SetCameraTarget (target);
		
		vOrbitInput = hOrbitInput = zoomInput = hOrbitSnapInput = mouseOrbitInput = vMouseOrbitInput = 0;
		
		MoveToTarget ();
		
		previousMousePos = currentMousePos = Input.mousePosition;
		
		collision.Ititialize (Camera.main);
		collision.UpdateCameraClipPositions (transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPositions (destination, transform.rotation, ref collision.desiredCameraClipPoints);
	}

	void SetCameraTarget(Transform t){
		target = t;
	}
/*
	void GetInput(){
		vOrbitInput = Input.GetAxisRaw (input.ORBIT_VERTICAL);
		hOrbitInput = Input.GetAxisRaw (input.ORBIT_HORISONTAL);
		hOrbitSnapInput = Input.GetAxisRaw(input.ORBIT_HORISONTAL_SNAP);
		zoomInput = Input.GetAxisRaw (input.ZOOM);
		mouseOrbitInput = Input.GetAxisRaw (input.MOUSE_ORBIT);
		vMouseOrbitInput = Input.GetAxisRaw (input.MOUSE_ORBIT_VERTICAL);
	}
*/
	void Update(){
		//GetInput ();
		//ZoomOnTarget ();
	}

	void FixedUpdate()
	{
		MoveToTarget ();
		LookAtTarget ();
		OrbitTarget ();
		MouseOrbitTarget ();
		
		collision.UpdateCameraClipPositions (transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPositions (destination, transform.rotation, ref collision.desiredCameraClipPoints);
		
		// draw debug lines
		for (int i = 0; i < 5; i++) {
			if (debug.drawDesiredCollisionLines){
				Debug.Log("asdsa" + collision.desiredCameraClipPoints[i]);
				Debug.Log("12312" + collision.adjustedCameraClipPoints[i]);
				Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
			}
			
			if(debug.drawAdjustedCollisionLines){
				Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
			}
		}
		
		collision.CheckColliding (targetPos);//using raycasts here
		position.adjustmentDistance = collision.GetAdjastedDistanceWithRayFrom (targetPos);
	}

	void MoveToTarget()
	{
		targetPos = target.position + position.targetPosOffset;
		//targetPos = target.position + Vector3.up * position.targetPosOffset.y + Vector3.forward * position.targetPosOffset.z + transform.TransformDirection
		destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distanceFromTarget;
		destination += targetPos;
		
		if (collision.colliding){
			adjustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * Vector3.forward * position.adjustmentDistance;
			adjustedDestination += targetPos;
			
			if(position.smoothFollow){
				//user smooth damp
				transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref camVel, position.smooth);
			}
			else{
				transform.position = adjustedDestination;
			}
		}
		else {
			if(position.smoothFollow){
				//user smooth damp
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, position.smooth);
			}
			else{
				transform.position = destination;
			}
		}
	}

	void LookAtTarget(){
		Quaternion targetRotation = Quaternion.LookRotation (targetPos - transform.position);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, 100 * Time.deltaTime);
		//transform.rotation = Quaternion.Euler (0,target.transform.rotation.eulerAngles.y + 180, 0);
	}

	void OrbitTarget(){
		if(hOrbitSnapInput > 0){
			orbit.yRotation = -180;
		}

		orbit.xRotation += -vOrbitInput * orbit.vOrbitSmooth;
		orbit.xRotation += -hOrbitInput * orbit.hOrbitSmooth;
	}

/*
	void LateUpdate () {
		if (target == null) {
			Destroy(this.gameObject);
			return;
		}
		transform.position = target.transform.position;
		transform.rotation =  Quaternion.Euler (0,target.transform.rotation.eulerAngles.y + 180, 0);
		
		Vector3 targetPoint = defaultCameraPosition * cameraDistance;
		targetPoint = new Vector3 (targetPoint.x, targetPoint.y, targetPoint.z);
		
		attachedCamera.transform.position = transform.TransformPoint(targetPoint);
		h = player.GetComponent<BallControl>().getHSpeed();
		v = player.GetComponent<BallControl>().getVSpeed();
		
		var rot = attachedCamera.transform.eulerAngles; // Vector3 - rotation in degrees
		if (Mathf.Abs (h) > 0.2) {
			rot.x -= h / 10;
			rot.x = Mathf.Clamp(rot.x, 0, 40); 
		} else {
			if(rot.x < 19 ){
				rot.x += 0.4f;
			}
			if(rot.x > 21){
				rot.x -= 0.4f;
			}
			//rot.x = 15.0f;
			//rot.x--;
			rot.x = Mathf.Clamp(rot.x, 0, 40); 
		}
		//var a = 0;
		//a = ((rot.x - 10.0f) / 100.0f) * 10;
		//uno.PowerChange (int.Parse(a));
		
		//Debug.Log (rot);
		attachedCamera.transform.eulerAngles = rot;
		//}
	}
*/

	void MouseOrbitTarget(){
		previousMousePos = currentMousePos;
		currentMousePos = Input.mousePosition;

		Vector3.Normalize (previousMousePos);
		Vector3.Normalize (currentMousePos);

		if (mouseOrbitInput > 0) {
			orbit.xRotation += (currentMousePos.y - previousMousePos.y) * orbit.vOrbitSmooth;
			orbit.yRotation += (currentMousePos.x - previousMousePos.x) * orbit.hOrbitSmooth;
		}

		if (vMouseOrbitInput > 0) {
			orbit.xRotation += (currentMousePos.y - previousMousePos.y) * (orbit.vOrbitSmooth /2);
		}

		CheckVerticalRotation ();
	}

	void CheckVerticalRotation(){
		if (orbit.xRotation > orbit.maxXRotation) {
			orbit.xRotation = orbit.maxXRotation;
		}
		if (orbit.xRotation < orbit.minXRotation) {
			orbit.xRotation = orbit.minXRotation;
		}
	}

	void ZoomOnTarget(){
		position.newDistance += position.zoomStep * zoomInput;

		position.distanceFromTarget = Mathf.Lerp (position.distanceFromTarget, position.newDistance, position.zoomSmooth * Time.deltaTime);

		if (position.distanceFromTarget > position.maxZoom) {
			position.distanceFromTarget = position.maxZoom;
			position.newDistance = position.maxZoom;
		}
		if (position.distanceFromTarget < position.minZoom) {
			position.distanceFromTarget = position.minZoom;
			position.newDistance = position.minZoom;
		}
	}

	[System.Serializable]
	public class CollisionHandler{
		public LayerMask collisionLayer;
		
		[HideInInspector]
		public bool colliding = false;
		[HideInInspector]
		public Vector3[] adjustedCameraClipPoints;
		[HideInInspector]
		public Vector3[] desiredCameraClipPoints;
		
		Camera camera;
		
		public void Ititialize(Camera cam)
		{
			camera = cam;
			adjustedCameraClipPoints = new Vector3[5];
			desiredCameraClipPoints = new Vector3[5];
		}
		public void UpdateCameraClipPositions(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray){
			if (!camera)
				return;
			intoArray = new Vector3[5];
			float z = camera.nearClipPlane;
			float x = Mathf.Tan (camera.fieldOfView / 3.41f) * z;
			float y = x / camera.aspect;
			
			intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;
			intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
			intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
			intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
			intoArray[4] = cameraPosition - camera.transform.forward;
		}
		
		bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition){
			for (int i = 0; i < clipPoints.Length; i++) {
				Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
				float distance = Vector3.Distance(clipPoints[i], fromPosition);
				if(Physics.Raycast (ray, distance, collisionLayer)){
					return true;
				}
			}
			return false;
		}
		
		public float GetAdjastedDistanceWithRayFrom(Vector3 from){
			float distance = -1;
			
			for(int i = 0; i < desiredCameraClipPoints.Length; i++){
				Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)){
					if(distance == -1)
						distance = hit.distance;
					else{
						if(hit.distance < distance)
							distance = hit.distance;
					}
				}
			}
			
			if (distance == -1)
				return 0;
			else
				return distance;
		}
		
		public void CheckColliding(Vector3 targetPosition){
			if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition)){
				colliding = true;
			}
			else{
				colliding = false;
			}
		}
	}
}
