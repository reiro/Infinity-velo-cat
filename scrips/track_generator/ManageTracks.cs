using UnityEngine;
using System.Collections;

public class ManageTracks : MonoBehaviour
{
	private TrackCollection trackCollectionScript;
	public GameObject track;

	void Start(){
		trackCollectionScript = GameObject.Find ("GameController").GetComponent<TrackCollection> ();
	}

	void OnTriggerEnter(Collider collision){
		trackCollectionScript.RecycleTrack(track);
	}


}
