using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConstants", menuName = "UIConstants")]
public class UIConstants : ScriptableObject
{
    [Header("Menu Option Animation")] 
    public float scaleOnSelect;
    public float selectTime;

    [Header("Menu Screen Animation")] 
    public float offscreenDistance;
    public float menuScreenTransitionTime;

    [Header("Foldout List Animation")]
    public float foldoutTransitionTime;

    // TODO: add colors here but make it an editor function so that the UI designer can edit colors just from the SO and the UI elements automatically update on editor refresh
}