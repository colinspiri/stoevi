using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

namespace SpookuleleAudio
{
    public abstract class AListContainer : ASoundContainer
    {
        [TitleGroup("Container List"), ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)] public List<ASoundContainer> SoundContainers = new List<ASoundContainer>();
        
#if UNITY_EDITOR

	    void AddContainer<T>() where T : ASoundContainer
	    {
		    T child = ScriptableObject.CreateInstance<T>();
		    child.name = "New Container " + (Random.Range(10000,99999));
		    child.InitializeAsChild(this);
		    SoundContainers.Add(child);
		    
		    AssetDatabase.AddObjectToAsset(child, this);
		    AssetDatabase.SaveAssets();
		    
		    EditorUtility.SetDirty(this);
		    EditorUtility.SetDirty(child);
		    SoundContainerEditor.RebuildTree();
	    }
	    
	    [TitleGroup("Container List"), Button] public void AddBasicContainer() => AddContainer<BasicContainer>();
	    [TitleGroup("Container List"), Button] public void AddRandomContainer() => AddContainer<RandomContainer>();
	    [TitleGroup("Container List"), Button] public void AddSequenceContainer() => AddContainer<SequenceContainer>();
	    
#endif
    }
    
    #if UNITY_EDITOR
	
	[CustomEditor(typeof(ASoundContainer))]
	public class ASoundContainerEditor : OdinEditor {
		public override void OnInspectorGUI ()
		{
			ASoundContainer container = (ASoundContainer) target;

			EditorGUILayout.BeginHorizontal();
			{
				target.name = EditorGUILayout.TextField("Name", target.name);
				
				if (GUILayout.Button("Rename"))
				{
					EditorUtility.SetDirty(container);
					if (container.ParentContainer)
						EditorUtility.SetDirty(container.ParentContainer);
					else
						AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(container), target.name);
					SoundContainerEditor.RebuildTree();
				}
				
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();
			base.OnInspectorGUI();
		}
	}
	
	[CustomEditor(typeof(RandomContainer))]
	public class RandomContainerEditor : ASoundContainerEditor { }

	[CustomEditor(typeof(BasicContainer))]
	public class BasicContainerEditor : ASoundContainerEditor { }

	[CustomEditor(typeof(SequenceContainer))]
	public class SequenceContainerEditor : ASoundContainerEditor { }

#endif
    
}
