using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SpookuleleAudio;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Day1Cutscene : Cutscene {
    public ASoundContainer sheep_bleat;
    public ASoundContainer gate;

    protected override IEnumerator CutsceneCoroutine() {
        dayText.alpha = 0;
        
        // play gate sfx
        gate.Play();

        yield return new WaitForSecondsRealtime(3);
        
        dayText.alpha = 1;
        
        // play sheep baa 
        sheep_bleat.Play();

        yield return new WaitForSecondsRealtime(1.5f);


        yield return null;
    }
}
