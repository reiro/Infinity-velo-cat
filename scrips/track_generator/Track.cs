using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour
{

	public GameObject startPoint;
	public GameObject endPoint;

	public float get_track_size(){
		float length = 0.0f;
		//foreach (Transform child in transform)
		//{
		//	if (child.tag == "Track")
		//	{
		//		length += child.Find("floor").GetComponent<Renderer>().bounds.size.z;
		//	}
		//}
		length = endPoint.transform.position.z - startPoint.transform.position.z;
		return length;
	}

	public float get_track_width(){
		float width = transform.GetComponent<Renderer>().bounds.size.x;

		return width;
	}
}
