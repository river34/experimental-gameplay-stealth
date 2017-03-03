using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightTriggerController : MonoBehaviour {

	private Transform root;
	private GuardController guard;
	private Transform sight;
	public bool is_player;
	public bool is_guard;
	public Transform player_in_range;
	public List <Transform> guard_in_range;
	private GameController game;
	private bool is_notified;

	// Use this for initialization
	void Start ()
	{
		game = GameObject.Find ("Root").GetComponent <GameController> ();
		root = transform.parent.parent;
		is_player = false;
		is_guard = false;
		if (root.gameObject.tag == Tags.PLAYER)
		{
			is_player = true;
		}
		else if (root.gameObject.tag == Tags.GUARD)
		{
			is_guard = true;
			guard = root.gameObject.GetComponent <GuardController> ();
		}
		sight = transform.parent;

		guard_in_range = new List <Transform> ();

		is_notified = false;
	}

	// Update is called once per frame
	void Update ()
	{
		if (is_guard && player_in_range != null)
		{
			if (player_in_range.gameObject.GetComponent <PlayerController> ().GetInLight ()) // is player in light
			{
				if (Vector3.Distance (player_in_range.position, transform.position) <= guard.GetSightRange ()) // is player in sight range
				{
					if (!is_notified) // notify game controller
					{
						game.PlayerIsDiscoveredByGuard (player_in_range);
						guard.SetPlayerInSight (player_in_range);
						is_notified = true;
					}
				}
			}
			// else: is player in dark, nothing happens
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		// print (other.gameObject.name);
		if (is_guard && other.tag == Tags.PLAYER)
		{
			player_in_range = other.transform;
		}
		if (is_player && other.tag == Tags.GUARD)
		{
			guard_in_range.Add (other.transform);
			other.gameObject.GetComponent <GuardController> ().SetInSight (true);
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (is_guard && other.tag == Tags.PLAYER)
		{
			player_in_range = null;
		}
		if (is_player && other.tag == Tags.GUARD)
		{
			guard_in_range.Remove (other.transform);
			other.gameObject.GetComponent <GuardController> ().SetInSight (false);
		}
	}
}
