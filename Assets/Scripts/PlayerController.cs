using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private GameController game;
	private Animator animator;
	private SpriteRenderer render;
	private Color original_color;
	private Collider2D collider;
	private Vector2 original_collider_offset;
	private Transform sight;
	private Vector3 original_sight_position;
	private float speed;
	public bool is_in_light;
	private List<bool> is_in_light_buffer;
	public bool is_dead;

	// particles
	private ParticleSystem particle;
	private Color particle_color_in_light;
	private Color particle_color_in_dark;
	private ParticleSystem.EmissionModule emission;

	// movement
	public bool is_up;
	public bool is_in_sky;
	public bool is_on_ground;
	private float gravity;
	private float up_speed;
	private float down_speed;
	private float max_up_speed;
	private float min_x;
	private float min_y;
	private Vector3 original_position;

	// Use this for initialization
	void Start ()
	{
		game = GameObject.Find ("Root").GetComponent <GameController> ();
		animator = GetComponent <Animator> ();
		render = GetComponent <SpriteRenderer> ();
		original_color = render.material.GetColor ("_Color");
		collider = GetComponent <Collider2D> ();
		original_collider_offset = collider.offset;
		sight = transform.Find ("Sight");
		original_sight_position = sight.position;
		speed = 3f;
		is_in_light = false;
		is_in_light_buffer = new List<bool> ();
		for (int i = 0; i < 2; i++)
		{
			is_in_light_buffer.Add (is_in_light);
		}
		is_dead = false;

		// particles
		particle = transform.Find ("Particle System").gameObject.GetComponent <ParticleSystem> ();
		particle_color_in_light = Color.black;
		particle_color_in_dark = Color.white;
		emission = particle.emission;

		// movement
		is_up = false;
		is_in_sky = false;
		is_on_ground = true;
		gravity = 16f;
		up_speed = 0f;
		down_speed = 0f;
		max_up_speed = 8f;
		min_x = -6f;
		min_y = -5f;
		original_position = transform.position;
	}

	// Update is called once per frame
	void Update ()
	{
		UpdateMovement ();
		UpdateState ();
	}

	void LateUpdate ()
	{
		is_in_light_buffer.Add (is_in_light);
		is_in_light_buffer.Remove (is_in_light_buffer[0]);
	}

	public void Move (float x)
	{
		transform.position += Vector3.right * x * Time.deltaTime * speed;
	}

	void UpdateState ()
	{
		if (is_in_light_buffer[0] && is_in_light_buffer[1] && is_in_light)
		{
			render.material.SetColor ("_Color", original_color);
			particle.startColor = particle_color_in_light;
		}
		else
		{
			render.material.SetColor ("_Color", Color.black);
			particle.startColor = particle_color_in_dark;
		}

		if (is_dead)
		{
			particle.startColor = Color.black;
			// particle.startSize = 3f;
			particle.startLifetime = 3f;
			particle.startSpeed = 1f;
			emission.rate = 200f;
			render.material.SetColor ("_Color", new Color32 (0, 0, 0, 0));
		}
	}

	void UpdateMovement ()
	{
		if (is_up && !is_in_sky)
		{
			up_speed -= gravity * Time.deltaTime;
			if (up_speed < 0)
			{
				up_speed = 0;
			}
			transform.position += Vector3.up * Time.deltaTime * up_speed;
		}
		else if (!is_up && !is_on_ground)
		{
			down_speed += gravity * Time.deltaTime;
			transform.position += Vector3.down * Time.deltaTime * down_speed;
		}

		if (is_on_ground || is_in_sky)
		{
			down_speed = 0;
			up_speed = max_up_speed;
		}

		if (transform.position.x < min_x - 0.5f)
		{
			transform.position = new Vector3 (min_x, transform.position.y, transform.position.z);
		}
		if (transform.position.y < min_y - 0.5f)
		{
			transform.position = new Vector3 (transform.position.x, min_y, transform.position.z);
		}
	}

	public void SetInLight (bool _is_in_light)
	{
		is_in_light = _is_in_light;
		// visual clue that the player is in light or in dark
		// print (Time.time + " " + is_in_light);
	}

	public bool GetInLight ()
	{
		return is_in_light;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.CompareTag (Tags.DARK))
		{
			// print ("OnTriggerEnter2D DARK");
			if (GetInLight())
			{
				SetInLight (false);
			}
		}
		if (other.gameObject.CompareTag (Tags.OBJECTIVE_SMALL))
		{
			game.AddScore (Tags.OBJECTIVE_SMALL);
			Destroy (other.gameObject);
		}
		if (other.gameObject.CompareTag (Tags.OBJECTIVE))
		{
			game.AddScore (Tags.OBJECTIVE);
			Destroy (other.gameObject);
		}
		if (other.gameObject.name == "Ground")
		{
			is_on_ground = true;
		}
		if (other.gameObject.name == "Sky")
		{
			is_in_sky = true;
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.CompareTag (Tags.DARK))
		{
			// print ("OnTriggerExit2D DARK");
			if (!GetInLight())
			{
				SetInLight (true);
			}
		}
		if (other.gameObject.name == "Ground")
		{
			is_on_ground = false;
		}
		if (other.gameObject.name == "Sky")
		{
			is_in_sky = false;
		}
	}

	void OnTriggerStay2D (Collider2D other)
	{
		// initial check
		if (other.CompareTag (Tags.DARK))
		{
			if (GetInLight())
			{
				SetInLight (false);
			}
		}

		// recorrect
		if (other.CompareTag (Tags.LIGHT))
		{
			if (!GetInLight())
			{
				SetInLight (true);
			}
		}
	}

	public void Run ()
	{
		if (!animator.GetBool ("IsRunning"))
		{
			animator.SetBool ("IsRunning", true);
		}
	}

	public void Stop ()
	{
		if (animator.GetBool ("IsRunning"))
		{
			animator.SetBool ("IsRunning", false);
		}
	}

	public void Hide ()
	{
		if (!animator.GetBool ("IsHiding"))
		{
			animator.SetBool ("IsHiding", true);
			// collider.offset += Vector2.down;
			// sight.position += Vector3.down;
		}
		if (collider.offset.y > original_collider_offset.y - 1f)
		{
			collider.offset += Vector2.down * Time.deltaTime * 8f;
		}
		if (sight.position.y > original_sight_position.y - 1f)
		{
			sight.position += Vector3.down * Time.deltaTime * 8f;
		}
	}

	public void GetUp ()
	{
		if (animator.GetBool ("IsHiding"))
		{
			animator.SetBool ("IsHiding", false);
			// collider.offset = original_collider_offset;
			// sight.position = original_sight_position;
		}
		if (collider.offset.y < original_collider_offset.y)
		{
			collider.offset += Vector2.up * Time.deltaTime * 5f;
		}
		if (sight.position.y < original_sight_position.y)
		{
			sight.position += Vector3.up * Time.deltaTime * 5f;
		}
	}

	public void Flip (bool is_right)
	{
		Vector3 scale = gameObject.transform.localScale;
		scale.x = - scale.x;
		if (is_right)
		{
			scale.x = Mathf.Abs (scale.x);
		}
		else
		{
			scale.x = -1 * Mathf.Abs (scale.x);
		}
		gameObject.transform.localScale = scale;
	}

	public void SetUp ()
	{
		is_up = true;
	}

	public void SetDown ()
	{
		is_up = false;
	}

	public void SetDie ()
	{
		is_dead = true;
	}

	public void Init ()
	{
		is_in_light = false;
		for (int i = 0; i < 2; i++)
		{
			is_in_light_buffer.Remove (is_in_light_buffer[0]);
			is_in_light_buffer.Add (is_in_light);
		}
		is_dead = false;

		is_up = false;
		is_in_sky = false;
		is_on_ground = true;
		gravity = 16f;
		up_speed = 0f;
		down_speed = 0f;
		transform.position = original_position;

		particle.startColor = Color.white;
		// particle.startSize = 3f;
		particle.startLifetime = 1f;
		particle.startSpeed = 0.1f;
		emission.rate = 10f;
		render.material.SetColor ("_Color", Color.black);
	}
}
