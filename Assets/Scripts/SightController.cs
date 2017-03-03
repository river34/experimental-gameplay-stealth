using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightController : MonoBehaviour {

	// private Transform sigh;

	// Use this for initialization
	void Start ()
	{
		// sigh = transform.Find ("Sight");
	}

	// Update is called once per frame
	void Update ()
	{
		//
	}

	public void Rotate (float angle)
	{
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	public void Flip ()
	{
		Vector3 scale = gameObject.transform.localScale;
		scale.x = - scale.x;
		gameObject.transform.localScale = scale;
	}

	public Vector3 GetPosition ()
	{
		return transform.position;
	}

	public void Hide ()
	{
		if (gameObject.activeSelf)
		{
			gameObject.SetActive (false);
		}
	}

	public void Show ()
	{
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive (true);
		}
	}
}
