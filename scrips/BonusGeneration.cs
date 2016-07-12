using UnityEngine;
using System.Collections;

public class BonusGeneration : MonoBehaviour {

	public GameObject[] objects;
	private TrackCollection trackCollectionScript;

	void Start(){
		trackCollectionScript = GameObject.Find ("GameController").GetComponent<TrackCollection> ();
	}

	public void Generate (GameObject track, Vector3 position)
	{
		float track_length = track.GetComponent<Track> ().get_track_size ();
		float from_dist = position.z;
		float to_dist = position.z + track_length - 50;

		float track_width = track.GetComponent<Track> ().get_track_width ();
		float[] widths = new float[] {-10.0f, -15.0f, 10.0f, 15.0f};
		int widthId = Random.Range (0, 3);

		int lengthId = Random.Range(1, 3);

		Vector3 bonus_pos = new Vector3 (widths [widthId], 2, to_dist - track_length / lengthId);

		int bonusId = Random.Range (0, objects.Length);
		Instantiate (objects [bonusId], bonus_pos, Quaternion.identity);
		//(Instantiate(objects [bonusId], bonus_pos, Quaternion.identity) as GameObject).transform.parent = GameObject.Find("NewTrack(Clone)").transform;
		//Debug.Log (track);
		//(Instantiate (m_Prefab, position, rotation) as GameObject).transform.parent = parentGameObject.transform;
	}

}
