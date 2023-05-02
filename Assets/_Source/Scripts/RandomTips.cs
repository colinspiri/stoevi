using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomTips : MonoBehaviour {
    // components
    public TextMeshProUGUI text;
    
    // tips
    public AllTips allTips;

    private void OnEnable() {
        DisplayRandomTip();
    }

    private void DisplayRandomTip() {
        int tipsSeen = PlayerPrefs.GetInt("TipsSeen", 0);
        
        // if already seen all tips, get random
        int chosenIndex;
        if (tipsSeen >= allTips.tips.Count) {
            chosenIndex = Random.Range(0, allTips.tips.Count);
        }
        // otherwise go in order
        else {
            chosenIndex = tipsSeen;
        }
        
        // set tip text
        string chosenTip = allTips.tips[chosenIndex];
        text.text = chosenTip;

        // increment number of tips seen
        tipsSeen++;
        PlayerPrefs.SetInt("TipsSeen", tipsSeen);
        Debug.Log("TipsSeen = " + tipsSeen);
    }
}
