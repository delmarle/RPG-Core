using System;
using System.Collections.Generic;
using Station;
using UnityEngine;

public class PooledEventHandler : MonoBehaviour
{
	public float Delay = 1;

	private ObjectPool<EventPooled> _eventPool;
	private List<EventPooled> _activeEvents;

	private void Awake () 
	{
		_eventPool = new ObjectPool<EventPooled>(()=> new EventPooled(),8);
		_activeEvents = new List<EventPooled>();
	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			SpawnEvent();
		}

		for (int i = 0; i < _activeEvents.Count; i++)
		{
			_activeEvents[i].Update(Time.time);
		}
	}

	private void SpawnEvent()
	{
		var current = _eventPool.Create();
		current.Start(Time.time, Delay);
		current.StartEvent += OnCompleted;
		_activeEvents.Add(current);

	}

	private void OnCompleted(EventPooled ended)
	{
		ended.StartEvent -= OnCompleted;
		_activeEvents.Remove(ended);
		_eventPool.Recycle(ended);
		Debug.Log("pooled event start at " + ended.StartTime + " completed at "+ ended.EndTime);
	}
}

public class EventPooled
{
	public event Action<EventPooled> StartEvent;

	public float StartTime;
	public float EndTime;
	private float _delay;

	public void Start(float time, float delay)
	{
		_delay = delay;
		StartTime = time;
	}

	public void Update(float time)
	{
		if (!(time - StartTime > _delay)) return;
		EndTime = time;
		Trigger();
	}

	private void Trigger()
	{
		if(StartEvent != null)
		{
			StartEvent(this);
		}
	}
}
