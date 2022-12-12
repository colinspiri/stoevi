using UnityEngine;

namespace SpookuleleAudio
{
    
    [CreateAssetMenu(menuName = "AudioSystem/New Basic Container", fileName = "basic_new")]
    public class BasicContainer : ASoundContainer
    {
        public AudioClip AudioClip;
        
        public override SoundClip GetSoundClip()
        {
            SoundClip s = new SoundClip {AudioClip = AudioClip, Pitch = 1.0f, Volume = 1.0f, Loops = -1, LoopBounds = Vector2.zero};

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
            if (IgnoreListenerPause) 
                s.IgnoreListenerPause = true;

            return s;
        }
    }
}