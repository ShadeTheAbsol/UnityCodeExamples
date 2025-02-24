using UnityEngine;
using System.Collections;

public class CeilingSpiritTrail : MonoBehaviour
{

	void OnCollisionEnter2D (Collision2D col)
	{
		//Message Not Sending Fast Enough. player Rotates 180 Degrees Before jumpToCeiling Boolean Gets Set To True In Time.

		if (col.transform.CompareTag ("Player"))
		{
			col.gameObject.SendMessage ("MoveToCeiling",SendMessageOptions.DontRequireReceiver);
		}

	}
}
