using UnityEngine;
using System.Collections.Generic;

namespace Station
{
	[CreateAssetMenu(fileName = "SoundConfig", menuName = "Foundation/Audio/Create_SoundConfig", order = 1)]
	public class SoundConfig : ScriptableObject 
	{
		[Header("Parameters:")]
		[Range(0f, 1f)]public float Volume = 1f;
		public float MinPitch = 1f;
		public float MaxPitch = 1f;
		public float DelayAtStart;
		public float FadeInTime;
		public float FadeOutTime;
		public bool Looping;
	 
		[Header("Components:")]
		public List<AudioClip> Clips = new List<AudioClip>();
		public SourcePoolConfig SourceConfig;

		public AudioClip RandomizedClip()
		{
			return Clips.Count == 0 ? null : Clips.RandomItem();
		}
	    public float RandomizedPitch(){ return Random.Range(MinPitch, MaxPitch); }
	}
}