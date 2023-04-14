using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveText : MonoBehaviour {
    public TextMeshProUGUI text;
    public IntReference objectiveComplete;

    // Update is called once per frame
    void Update()
    {
        if (objectiveComplete.Value == 1) {
            text.text = "";
        }
        else if(ObjectiveManager.Instance != null) {
            text.text = ObjectiveManager.Instance.objectiveText;
        }
        else {
            text.text = "";
        }
    }
}
