using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class MenuOptionAnimator : MonoBehaviour {
    // components
    private Selectable selectable;
    public TextMeshProUGUI text;
    
    // constants
    public UIConstants uiConstants;
    
    // state
    private Color previousTextColor;

    private void Awake() {
        selectable = GetComponent<Selectable>();
    }

    private void Start() {
        if (text != null) previousTextColor = text.color;
    }

    public void Submit() {
        ExecuteEvents.Execute(selectable.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    public void DeselectAnimation() {
        selectable.transform.DOScale(1f, uiConstants.selectTime).SetUpdate(true);
        if(text != null) text.DOColor(previousTextColor, uiConstants.selectTime).SetUpdate(true);
    }

    public void SelectAnimation() {
        selectable.transform.DOScale(uiConstants.scaleOnSelect, uiConstants.selectTime).SetUpdate(true);
        if(text != null) text.DOColor(Color.red, uiConstants.selectTime).SetUpdate(true);
        if(AudioManager.Instance) AudioManager.Instance.PlaySelectSound();
    }

    public void SubmitAnimation()
    {
        // to be implemented
    }

    public void PlaySubmitSound()
    {
        if(AudioManager.Instance) AudioManager.Instance.PlaySubmitSound();
    }
    public void PlayBackSound()
    {
        if(AudioManager.Instance) AudioManager.Instance.PlayBackSound();
    }
}