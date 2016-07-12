using UnityEngine;
using System.Collections;

public class TrackCollection : MonoBehaviour
{
	public GameObject[] tracks;
	private BonusGeneration bonusGenerationScript;
	public int scene_tracks_count = 3;
	public int generate_count = 3;
	private float summ_length = 0.0f;

	void Awake ()
	{
		bonusGenerationScript = GameObject.Find ("GameController").GetComponent<BonusGeneration> ();
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		CreateTracksPool ();
		AllocateTracks (Vector3.zero);
	}

	private void CreateTracksPool ()
	{
		foreach (GameObject eachTrack in tracks) {
			eachTrack.GetComponent<Track> ().CreatePool ();
		}
	}

	public void AllocateTracks (Vector3 position)
	{
		for (int i = 0; i < generate_count; i++) {
			int trackId = i;
			//Debug.Log (tracks [trackId].GetComponent<Track>().get_track_size());
			summ_length += tracks [trackId].GetComponent<Track>().get_track_size();

			Track trackScript = tracks [trackId].GetComponent<Track> ().Spawn (position);
			GameManager.Instance.AllocatedTrackList.Add (trackScript.gameObject);
			position = new Vector3(position.x, position.y, position.z + tracks [trackId].GetComponent<Track>().get_track_size());
		}
	}


	public void AllocateTrack (Vector3 position)
	{
		int trackId = Random.Range (0, tracks.Length);
		position = new Vector3 (position.x, position.y, summ_length);
		Track trackScript = tracks [trackId].GetComponent<Track> ().Spawn (position);
		bonusGenerationScript.Generate (tracks [trackId], position);
		GameManager.Instance.AllocatedTrackList.Add (trackScript.gameObject);

		summ_length += tracks [trackId].GetComponent<Track>().get_track_size();
	}

	public void RecycleTrack (GameObject trackObject)
	{
		if (GameManager.Instance.AllocatedTrackList.Count > scene_tracks_count) {
			RemoveTrack();
		}
		GenerateTrack (trackObject);
	}

	private void RemoveTrack ()
	{
		GameObject trackGroupObject = GameManager.Instance.AllocatedTrackList [0];
		trackGroupObject.GetComponent<Track> ().Recycle ();
		GameManager.Instance.AllocatedTrackList.Remove (trackGroupObject);
	}

	private void GenerateTrack (GameObject trackObject)
	{
		Vector3 trackPosition = trackObject.transform.position;
		AllocateTrack (trackPosition);
	}
}
