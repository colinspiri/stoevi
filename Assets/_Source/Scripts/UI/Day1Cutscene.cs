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

    protected override IEnumerator CutsceneCoroutine() {
        dayText.alpha = 0;
        
        if(AudioManager.Instance) AudioManager.Instance.PlayWalking();

        yield return new WaitForSecondsRealtime(2);
        
        dayText.alpha = 1;
        
        // play sheep baa 
        sheep_bleat.Play();

        yield return new WaitForSecondsRealtime(2);
        
        // play gate sfx

        yield return null;
    }
}
