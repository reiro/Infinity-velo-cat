using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private List<GameObject> allocatedTrackList;
	private float timer = 0.0f;
	public bool testControll;
	public float finishTime = 30.0f;
	public float restartDelay = 5f;

    void Awake()
    {
        instance = this;
        allocatedTrackList = new List<GameObject>();
    }

	void Update(){
		timer += Time.deltaTime;

		if (timer >= finishTime) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void OnGUI(){
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 30;
		style.normal.textColor = new Color (255f, 0.0f, 90f, 1.0f);

		//string timerString = timer.ToString ();
		GUI.Label (new Rect (w/2 - w/20, 0,0,0), "Время: " + Math.Round(timer, 2), style);
	}

	public bool getTestControll(){
		return testControll;
	}

	public void ChangeTestControll(){
		Debug.Log (testControll);
		if (testControll == true) {
			testControll = false;
		} else {
			testControll = true;
		}
	}

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public List<GameObject> AllocatedTrackList
    {
        get
        {
            return allocatedTrackList;
        }
    }
}


