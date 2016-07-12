using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class CatUnoConnect : MonoBehaviour {
	
	SerialPort serial;
	Thread myThread;
	CatControl catControl;
	public string num;
	public string[] vec5;
	public string powerControl = "0";
	public int power;
	public bool sendS;
	public bool cullerControl = false;
	int n = 0;
	
	void Start () {
		catControl = GetComponent<CatControl>();
		
		try{
			serial = new SerialPort("COM"+num, 115200);
			serial.Open();
		}
		catch{
			Debug.Log("Could not open serial port: ");
		}
		myThread = new Thread (new ThreadStart (GetArduino));
		myThread.IsBackground = true;
		myThread.Start ();
	}
	
	private void GetArduino(){
		while(myThread.IsAlive){
			string value = serial.ReadLine();
			vec5 = value.Split(' ');
			string newString = "Connected: "+vec5[0]+", "+vec5[1]+", "+vec5[2]+", "+vec5[3]+", "+vec5[4]+", "+vec5[5];
			//Debug.Log (newString);
			catControl.currentAngle = float.Parse(vec5[0]);
			catControl.powerLevel = int.Parse(vec5[1]) + 1;
			power = int.Parse(vec5[1]);
			catControl.turn = int.Parse(vec5[5]);
			
			catControl.firstAction = int.Parse(vec5[3]);
			catControl.secondAction = int.Parse(vec5[4]);
			//Debug.Log (int.Parse(vec5[4]));
			
			//powerControl = (int.Parse(powerLevel).ToString();
			if (sendS == true){
				serial.Write(powerControl);
				//Debug.Log(newString);
				
				sendS = false;
			}
			//			PowerChange();
		}
	} 
	
	void OnApplicationQuit()
	{
		powerControl = "V";
		cullerControl = false;
		sendS = true;
		try{
			serial.Close();
			myThread.Abort ();
			Debug.Log (myThread.IsAlive);
		}
		catch(UnityException e){
			Debug.Log(e);
		}
		
	}
	
	public void PowerChange(float powerDelta){
		powerControl = (powerDelta + power).ToString();
		sendS = true;
		Debug.Log("Power Change "+ powerControl);
	}
	
	public void FixedPowerChange(float power){
		powerControl = (power).ToString();
		sendS = true;
		//Debug.Log(powerControl);
	}

	public void CullerOnOff(){
		if (cullerControl == true) {
			powerControl = "V";
			cullerControl = false;
		} else if (cullerControl == false) {
			powerControl = "v";
			cullerControl = true;
		}

		sendS = true;
	}

	public void QuitGame(){
		Debug.Log ("EXIT");
		powerControl = "V";
		cullerControl = false;
		sendS = true;
		try{
			serial.Close();
			myThread.Abort ();
			Debug.Log (myThread.IsAlive);
		}
		catch(UnityException e){
			Debug.Log(e);
		}
		Application.Quit();
	}
	//void OnGUI()
	//{
	//string value = serial.ReadLine();
	//string[] vec5 = value.Split(' ');
	//string newString = "Connected: " + vec5[0] + ", " + vec5[1] + ", " + vec5[2] + ", " + vec5[3] + ", " + vec5[4];
	//		GUI.Label(new Rect(10,10,300,100), newString); //Display new values
	//}
	
}
