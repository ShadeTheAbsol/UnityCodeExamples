using UnityEngine;
using System.Collections;

public class Meltable : MonoBehaviour 
{

	public float meltSpeed;

	void OnTriggerStay2D (Collider2D col)
	{
		if (col.tag == "Breath" && PlayerMovement.activeBreath == 1 && Input.GetKey(KeyCode.X)) 
		{
			if (transform.localScale.y > 0) 
			{
				transform.Translate (Vector2.down * Time.deltaTime * meltSpeed);
				transform.localScale -= new Vector3 (0, Time.deltaTime * meltSpeed, 0);
			}

			if (transform.localScale.y <= 0 || transform.localScale.y <= 0.2f)
				transform.localScale = new Vector3 (0,0,0);
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
			if (transform.localScale.y <= 0.5f)
			{
			while (transform.localScale.y > 0) 
				{
					transform.Translate (Vector2.down * Time.deltaTime * meltSpeed);
					transform.localScale -= new Vector3 (0, Time.deltaTime * meltSpeed, 0);
				}
			}

			if (transform.localScale.y <= 0)
				transform.localScale = new Vector3 (0, 0, 0);
	}

}
