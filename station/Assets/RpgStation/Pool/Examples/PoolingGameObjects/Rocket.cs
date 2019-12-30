using UnityEngine;

using Station;

public class Rocket : MonoBehaviour
{
	public float MaxSpeed = 12;
	private float _velocity;

	/// <summary>
	/// called automatically by the Pool each time the object is spawned
	/// </summary>
	private void OnSpawn()
	{
		_velocity = 0;
	}

	/// <summary>
	/// called automatically by the Pool each time the object is despawned
	/// </summary>
	private void OnDespawn()
	{
		_velocity = 0;
	}

	private void Update()
	{
		_velocity += MaxSpeed;

		transform.Translate(0, _velocity*Time.deltaTime, 0);

		if(transform.localPosition.y >10)
		{
			PoolSystem.Despawn(gameObject);
		}
	}
}
