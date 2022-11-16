using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SpookuleleAudio
{
	public class SoundPlayer : MonoBehaviour
	{
		public AudioSource Source => mLoopFlipper? mLooperSource : mSource;
		
		AudioSource mSource;
		AudioSource mLooperSource;
		bool mLoopFlipper = false;

		int mLoopCount = -1;
		Vector2 mLoopBounds = Vector2.one;

		public void Stop()
		{
			StopAllCoroutines();
			mSource.Stop();
			mLooperSource.Stop();
			mLoopFlipper = false;
		}
		
		public void Initialize()
		{
			mLoopFlipper = false;
			gameObject.SetActive(true);
			
			if(mSource==null)
				mSource = gameObject.AddComponent<AudioSource>();
			if(mLooperSource==null)
				mLooperSource = gameObject.AddComponent<AudioSource>();
			
			gameObject.SetActive(false);
		}
		
		public void Play(AudioClip clip, float volume, float pitch, AudioMixerGroup aMixGroup, float atTime = 0.0f, float duration = float.MaxValue)
		{
			gameObject.SetActive(true);
			Stop();
			StartCoroutine(CR_Play(clip, volume, pitch, false, Vector3.zero, aMixGroup, -1, Vector2.one, atTime, duration));
		}

		public void PlayLooped(AudioClip clip, float volume, float pitch, AudioMixerGroup aMixGroup, int loops,
			Vector2 loopBounds, float atTime = 0.0f)
		{
			mLoopCount = loops;
			mLoopBounds = loopBounds;
			gameObject.SetActive(true);
			Stop();
			StartCoroutine(CR_Play(clip, volume, pitch, false, Vector3.zero, aMixGroup, mLoopCount, mLoopBounds, atTime, float.MaxValue));
		}
		
		public void Play3D(AudioClip clip, float volume, float pitch, Vector3 position, AudioMixerGroup aMixGroup, float atTime = 0.0f, float duration = float.MaxValue)
		{
			gameObject.SetActive(true);
			Stop();
			StartCoroutine(CR_Play(clip, volume, pitch, true, position, aMixGroup, -1, Vector2.one, atTime, duration));
		}

		public void Play3DLooped(AudioClip clip, float volume, float pitch, Vector3 position, AudioMixerGroup aMixGroup, int loops,
			Vector2 loopBounds, float atTime = 0.0f)
		{
			mLoopCount = loops;
			mLoopBounds = loopBounds;
			gameObject.SetActive(true);
			Stop();
			StartCoroutine(CR_Play(clip, volume, pitch, true, position, aMixGroup, mLoopCount, mLoopBounds, atTime, float.MaxValue));
		}


		IEnumerator CR_Play(AudioClip clip, float volume, float pitch, bool is3D, Vector3 position, AudioMixerGroup aMixGroup, int loops, Vector2 loopBounds, float atTime, float duration)
		{
			mSource.time = 0.0f;
			
			mSource.clip = clip;
			mLooperSource.clip = clip;
			
			mSource.timeSamples = Mathf.RoundToInt(clip.frequency * atTime);
			mLooperSource.timeSamples = Mathf.RoundToInt(clip.frequency * atTime);
			
			mSource.volume = volume;
			mLooperSource.volume = volume;
			
			mSource.pitch = pitch;
			mLooperSource.pitch = pitch;
			
			mSource.outputAudioMixerGroup = aMixGroup;
			mLooperSource.outputAudioMixerGroup = aMixGroup;
			
			mSource.spatialBlend = is3D ? 1.0f : 0.0f;
			mLooperSource.spatialBlend = is3D ? 1.0f : 0.0f;
			transform.position = position;

			mLoopCount = loops;
			mLoopBounds = loopBounds;
			
			float waitTime = Mathf.Min(duration, clip.length);
			if (loops < 0)
				yield return CR_PlaySound(waitTime);
			else
				yield return CR_PlayLoopingSound();
		}

		IEnumerator CR_PlaySound(float waitDuration)
		{
			mSource.Play();
			yield return new WaitForSecondsRealtime(waitDuration);
			gameObject.SetActive(false);
		}
		
		IEnumerator CR_PlayLoopingSound()
		{
			mLooperSource.clip = mSource.clip;

			int loops = 0;
			
			while (mLoopCount == 0 || loops < mLoopCount)
			{
				mLoopFlipper = !mLoopFlipper;
				if(loops > 0)
					Source.timeSamples = Mathf.RoundToInt(Source.clip.frequency * mLoopBounds.x);
				float loopMax = Mathf.RoundToInt(Source.clip.frequency * mLoopBounds.y);
				Source.Play();
				
				while (Source.timeSamples <= loopMax)
					yield return null;

				loops++;
				yield return null;
			}

			gameObject.SetActive(false);
		}

	}
}