using UnityEngine;

namespace SpookuleleAudio
{
    
    [CreateAssetMenu(menuName = "AudioSystem/New Sequence Container", fileName = "sequence_new")]
    public class SequenceContainer : AListContainer
    {
        int mLastPlayedIndex = -1;
        
        public override SoundClip GetSoundClip()
        {
            int index = mLastPlayedIndex + 1;

            if (index >= SoundContainers.Count || index < 0)
                index = 0;
            
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