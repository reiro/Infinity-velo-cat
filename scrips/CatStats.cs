using UnityEngine;
using System.Collections;

[System.Serializable]
public class CatStats {
	public float rollSpeed = 100;
	public float airSpeed = 15;
	public float jumpHeight = 30;
	public float speed = 100;
	public float gravity = 1;
	public float angleKo = 5000.0f; // Коэффициент на который делится угол поворота от рули (1042 / AnngleKo)
}
