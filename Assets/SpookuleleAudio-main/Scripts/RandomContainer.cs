using UnityEngine;

namespace SpookuleleAudio
{
	[CreateAssetMenu(menuName = "AudioSystem/New Random Container", fileName = "random_new")]
	public class RandomContainer : AListContainer
	{
		public bool NoRepeats;
		int mLastPlayedIndex = -1;

		public override SoundClip GetSoundClip()
		{
			int index = 0;
			do
				index = Random.Range(0, SoundContainers.Count);
			while (NoRepeats && index == mLastPlayedIndex && SoundContainers.Count > 1);

			mLastPlayedIndex = index;

			SoundClip s = SoundContainers[index].GetSoundClip();
			if (PitchRangeEnabled)
				s.Pitch = Random.Range(PitchRange.x, PitchRange.y);
			if (VolumeRangeEnabled)
				s.Volume = Random.Range(VolumeRange.x, VolumeRange.y);
			if (AudioGroupEnabled)
				s.AudioGroup = AudioGroup;
			if (LoopingEnabled)
			{
				s.Loops = LoopCount;
				s.LoopBounds = LoopBounds;
			}

			return s;
		}
	}
}
