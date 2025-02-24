using UnityEngine;
using System.Collections;

public class DashObstacle : MonoBehaviour 
{
	private Animator anim;

	void Start ()
	{
		anim = GetComponent<Animator> ();
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.transform.CompareTag ("SpiritRushOrb"))
		{
			this.GetComponent<Collider2D> ().enabled = false;
			anim.SetBool ("break", true);
			Invoke ("BreakObstacle", 0.3f);
		}
	}

	private void BreakObstacle ()
	{
		gameObject.SetActive (false);
	}
}
