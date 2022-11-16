#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace SpookuleleAudio
{
    public class SoundContainerEditor : OdinMenuEditorWindow
    {
        static SoundContainerEditor INSTANCE;
        static string PreviousSoundFolder = "Assets/";
        
        [MenuItem("Tools/Sound Container Editor")]
        static void OpenWindow()
        {
            GetWindow<SoundContainerEditor>().Show();
        }

        protected override void Initialize()
        {
            INSTANCE = this;
            base.Initialize();
        }

        protected override void OnGUI()
        {
            SoundPlayer player = ASoundContainer.CurrentSoundPreview;
            if (player != null && player.Source.isPlaying)
                Repaint();
            base.OnGUI();
        }

        public static void RebuildTree()
        {
            if(INSTANCE)
                INSTANCE.ForceMenuTreeRebuild();
        }

        protected override void DrawMenu()
        {
            base.DrawMenu();

            GUILayout.FlexibleSpace();
            GUILayout.Label("Create new...");
            NewContainerButton<BasicContainer>();
            NewContainerButton<MusicContainer>();
            NewContainerButton<RandomContainer>();
            NewContainerButton<SequenceContainer>();
            GUILayout.Space(20);
        }

        void NewContainerButton<T>() where T : ASoundContainer
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(GetIconFromContainerType(typeof(T)), GUILayout.Width(20));
                if (GUILayout.Button(GetNameFromContainerType(typeof(T)), GUILayout.ExpandWidth(true)))
                {
                    string absolutePath = EditorUtility.SaveFilePanel("New Container", PreviousSoundFolder, "New Container", "asset");

                    if (absolutePath.Length > 0)
                    {
                        string localPath = absolutePath.Substring(Application.dataPath.Length);
                        string path = "Assets" + localPath;
                        PreviousSoundFolder = path;
                        CreateNewContainer<T>(path);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        void CreateNewContainer<T>(string path) where T : ASoundContainer
        {
            T child = ScriptableObject.CreateInstance<T>();
		    
            AssetDatabase.CreateAsset(child, path);
            AssetDatabase.SaveAssets();
		    
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(child);
            RebuildTree();
        }
        
        Texture2D GetRandomContainerIcon() => GetIconFromContainerType(typeof(RandomContainer));
        Texture2D GetSequenceContainerIcon() => GetIconFromContainerType(typeof(SequenceContainer));
        Texture2D GetBasicContainerIcon() => GetIconFromContainerType(typeof(BasicContainer));

        string GetNameFromContainerType(Type type)
        {
            if (type == typeof(RandomContainer))
                return "Random Container";
            if (type == typeof(SequenceContainer))
                return "Sequence Container";
            if (type == typeof(MusicContainer))
                return "Music Container";

            return "Basic Container";
        }
        
        Texture2D GetIconFromContainerType(Type type)
        {
            if (type == typeof(RandomContainer))
                return Resources.Load<Texture2D>("Editor/Sprites/icon_random_container");
            if (type == typeof(SequenceContainer))
                return Resources.Load<Texture2D>("Editor/Sprites/icon_sequence_container");
            if (type == typeof(MusicContainer))
                return Resources.Load<Texture2D>("Editor/Sprites/icon_music_container");
            
            return Resources.Load<Texture2D>("Editor/Sprites/icon_basic_container");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(false, OdinMenuStyle.TreeViewStyle);

            var soundContainers = Resources.LoadAll<ASoundContainer>("");
            
            foreach (var soundContainer in soundContainers)
            {
                var editorIcon = GetIconFromContainerType(soundContainer.GetType());
                tree.Add(soundContainer.Path, soundContainer, editorIcon);
            }

            return tree;
        }
    }

}
#endif