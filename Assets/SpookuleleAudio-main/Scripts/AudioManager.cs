using System.Collections.Generic;
using UnityEngine;

namespace SpookuleleAudio
{
	public class AudioManager : MonoBehaviour
	{
		[SerializeField] int SoundPlayerCount = 100;
		static AudioManager INSTANCE;

		GameObject mPoolParent;
		Queue<SoundPlayer> mSoundPlayers;
		

		void Awake()
		{
			INSTANCE = this;
			mSoundPlayers = new Queue<SoundPlayer>();
			for (var i = 0; i < SoundPlayerCount; i++)
			{
				SoundPlayer player = new GameObject("player").AddComponent<SoundPlayer>();
				player.transform.SetParent(transform);
				player.Initialize();
				mSoundPlayers.Enqueue(player);
			}
		}

		public static void PlaySound(ASoundContainer container)
		{
			SoundPlayer player = INSTANCE.mSoundPlayers.Dequeue();
			
			SoundClip clip = container.GetSoundClip();
			if (clip.AudioClip == null)
				Debug.LogError("Null Clip! @ Container " + container.name);
			else if(clip.Loops == -1)
				player.Play(clip.AudioClip, clip.Volume, clip.Pitch, clip.AudioGroup, 0f);
			else
				player.PlayLooped(clip.AudioClip, clip.Volume, clip.Pitch, clip.AudioGroup, clip.Loops, clip.LoopBounds, 0f);
			
			INSTANCE.mSoundPlayers.Enqueue(player);
		}
		
		public static void PlaySound3D(ASoundContainer container, Vector3 position)
		{
			SoundPlayer player = INSTANCE.mSoundPlayers.Dequeue();
			
			
			SoundClip clip = container.GetSoundClip();
			if (clip.AudioClip == null)
				Debug.LogError("Null Clip! @ Container " + container.name);
			else if(clip.Loops == -1)
				player.Play3D(clip.AudioClip, clip.Volume, clip.Pitch, position, clip.AudioGroup, 0f);
			else
				player.Play3DLooped(clip.AudioClip, clip.Volume, clip.Pitch, position, clip.AudioGroup, clip.Loops, clip.LoopBounds, 0f);
			
			INSTANCE.mSoundPlayers.Enqueue(player);
		}
	}
}