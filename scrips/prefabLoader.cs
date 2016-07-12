using UnityEngine;
using System.Collections;

public class prefabLoader : MonoBehaviour {

	public GameObject startPrefab;
	public GameObject finishPrefab;
	public GameObject[] prefabs;

	GameObject[] instantiatedRoads;
	GameObject clone;
	int currentRoad = 0;

	int deleteId = 0;

	// Use this for initialization
	void Start () {
		//instantiatedRoads = new GameObject[100];
		//Debug.Log (instantiatedRoads.Length);
		for (int i = 0; i < 2; i++) {
			var z = i * 100;
			Instantiate(prefabs[i].transform, new Vector3(0, 0, z), prefabs[i].transform.rotation);
			currentRoad += 1;
		}
		//instaniatedRoads =  GameObject.FindGameObjectsWithTag("road");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void instantiateNext(Vector3 pos){
		if (currentRoad + 1 < prefabs.Length) {
			//int prefabId = Random.Range(0,1);
			Debug.Log (pos);
			//Instantiate (prefabs [currentRoad].transform, new Vector3 (0, 0, pos.z + 200), prefabs [prefabId].transform.rotation);
			Instantiate (prefabs [currentRoad].transform, new Vector3 (0, 0, pos.z + 150), prefabs [currentRoad].transform.rotation);
			currentRoad += 1;
			Debug.Log (currentRoad);
		} else {
			currentRoad = 0;
			Debug.Log (currentRoad);
			Debug.Log (pos.z + 150);
			Instantiate (prefabs [currentRoad].transform, new Vector3 (0, 0, pos.z + 150), prefabs [currentRoad].transform.rotation);
		}

		instantiatedRoads = GameObject.FindGameObjectsWithTag("road");
		if (instantiatedRoads.Length > 3) {
			Destroy(instantiatedRoads[deleteId]);
		}



		//deleteId += 1;

	}

	public void deleteLast(int id){



		//instantiatedRoads =  GameObject.FindGameObjectsWithTag("road");

	}
}
