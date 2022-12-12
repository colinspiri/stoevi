using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SpookuleleAudio
{
	[Serializable]
	public struct SoundClip
	{
		public AudioClip AudioClip;
		public AudioMixerGroup AudioGroup;
		public float Pitch;
		public float Volume;
		public Vector2 LoopBounds;
		public int Loops;
		public bool IgnoreListenerPause;
	}
}