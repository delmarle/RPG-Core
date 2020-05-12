using UnityEngine;
using Station;

public class RocketLauncher : MonoBehaviour
{
	public PooledItem RocketPrefab;


	private void Start()
	{
		//Optionally you can start by preloading objects to avoid hiccups later
		PoolSystem.PopulatePool(RocketPrefab, 32);
	}

	private void Update()
	{
		if(Input.GetMouseButton(0))
		{
			Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
			SpawnBullet(Camera.main.ScreenToWorldPoint(pos), Quaternion.identity);	
		}
	}

	private void SpawnBullet(Vector3 position, Quaternion rotation)
	{
		//Rocket rocket = PoolManager.Spawn(RocketPrefab, position, rotation).GetComponent<Rocket>();
		var item = PoolSystem.Spawn(RocketPrefab, position, rotation);
		var component = item.GetComponent(typeof(UiPopup));
		var casted = (UiPanel) component;
	}
}
