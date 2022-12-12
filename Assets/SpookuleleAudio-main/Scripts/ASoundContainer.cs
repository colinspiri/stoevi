using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SpookuleleAudio
{
    public abstract class ASoundContainer : ScriptableObject
    {
         
#if UNITY_EDITOR

        public void InitializeAsChild(AListContainer container)
        {
            ParentContainer = container;
        }
        
        [TitleGroup("Editor", Order = -1), ShowIf("HasParentContainer"), Button]
        public void RemoveFromParent()
        {
            if (EditorUtility.DisplayDialog("Remove from List " + ParentContainer.name,
                "Are you sure you want to delete this object?", "yes"))
            {
                ParentContainer.SoundContainers.Remove(this);
                EditorUtility.SetDirty(ParentContainer);
                DestroyImmediate(this, true);
                AssetDatabase.SaveAssets();
            }
            
            SoundContainerEditor.RebuildTree();
        }
        
#endif
        
        [HorizontalGroup("Pitch", 20), HideLabel] public bool PitchRangeEnabled = false; 
        [HorizontalGroup("Pitch"), LabelWidth(100), EnableIf("PitchRangeEnabled"), MinMaxSlider(0.0f, 1.5f, true)] public Vector2 PitchRange = Vector2.one;
        
        [HorizontalGroup("Volume", 20), HideLabel] public bool VolumeRangeEnabled = false;
        [HorizontalGroup("Volume"), LabelWidth(100), EnableIf("VolumeRangeEnabled"), MinMaxSlider(0.0f, 1.5f, true)] public Vector2 VolumeRange = Vector2.one;
        
        [HorizontalGroup("Mixer", 20), HideLabel] public bool AudioGroupEnabled = false;
        [HorizontalGroup("Mixer"), LabelWidth(100), EnableIf("AudioGroupEnabled")] public AudioMixerGroup AudioGroup;

        [ToggleGroup("LoopingEnabled", "Looping")] public bool LoopingEnabled;
        [ToggleGroup("LoopingEnabled", "Looping"), MinMaxSlider(0.0f, "@SoundClipMax", true)]
        public Vector2 LoopBounds;
        [ToggleGroup("LoopingEnabled", "Looping"), Tooltip("0 means infinite")]
        public int LoopCount;
        
        public bool IgnoreListenerPause;

        float SoundClipMax => GetSoundClip().AudioClip != null ? GetSoundClip().AudioClip.length : 1.0f;

        [Space(25), SerializeField, Sirenix.OdinInspector.ReadOnly] public AListContainer ParentContainer;

        bool HasParentContainer => ParentContainer != null;

        public string Path => HasParentContainer ? (ParentContainer.Path + "/" + name) : name;


        public abstract SoundClip GetSoundClip();

        public void Play() => AudioPlayer.PlaySound(this);
        public void Play3D(Transform source) => AudioPlayer.PlaySound3D(this, source.position);
        public void Play3D(Vector3 pos) => AudioPlayer.PlaySound3D(this, pos);

        
#if UNITY_EDITOR

        public static SoundPlayer CurrentSoundPreview;
        void OnEnable()
        {
            CurrentSoundPreview = EditorUtility.CreateGameObjectWithHideFlags("Audio Preview", HideFlags.HideAndDontSave,
                typeof(SoundPlayer)).GetComponent<SoundPlayer>();
            CurrentSoundPreview.Initialize();
            
        }

        void OnDisable()
        {
            DestroyImmediate(CurrentSoundPreview);
        }

        public bool PreviewPlaying => CurrentSoundPreview.Source.isPlaying;
        string PreviewPrompt => !PreviewPlaying ? "Play Preview" : "Stop Preview";

        [TitleGroup("Editor"), Button("@PreviewPrompt", ButtonSizes.Large)]
        void PlayOrStopPreview()
        {
            if(CurrentSoundPreview.Source.isPlaying)
                CurrentSoundPreview.Stop();
            else
            {
                SoundClip clip = GetSoundClip();
                if (clip.AudioClip == null)
                    Debug.LogError("Null Clip! @ Container " + name);
                else if(clip.Loops == -1)
                    CurrentSoundPreview.Play(clip.AudioClip, clip.Volume, clip.Pitch, clip.AudioGroup, true, PreviewStartTime);
                else
                    CurrentSoundPreview.PlayLooped(clip.AudioClip, clip.Volume, clip.Pitch, clip.AudioGroup, true, clip.Loops, clip.LoopBounds, PreviewStartTime);
            }
        }

        [TitleGroup("Editor"), ShowInInspector, PropertyRange(0.0f, "@SoundClipMax")] public float PreviewStartTime = 0.0f;

        float ProgressMax => PreviewPlaying ? CurrentSoundPreview.Source.clip.length : 1.0f;
        
        [TitleGroup("Editor"), ShowInInspector, ProgressBar(0.0f, "@ProgressMax")] float PreviewProgress => PreviewPlaying ? CurrentSoundPreview.Source.time : 0.0f;


#endif
    }
}