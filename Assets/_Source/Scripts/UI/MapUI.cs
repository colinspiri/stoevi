using UnityEngine;
using UnityEngine.InputSystem;

public class MapUI : MonoBehaviour {
    // components
    private InputActions inputActions;
    public CanvasGroup canvasGroup;
    
    // state
    public IntVariable mapEnabled;

    private void Awake() {
        inputActions = new InputActions();
        inputActions.Enable();
    }

    // Start is called before the first frame update
    void Start() {
        SetMapEnabled(false);

        inputActions.Gameplay.Map.started += EnableMap;
        inputActions.Gameplay.Map.canceled += DisableMap;
    }

    public void SetMapEnabled(bool value) {
        mapEnabled.SetValue(value ? 1 : 0);

        canvasGroup.alpha = mapEnabled.Value;
    }
    
    private void EnableMap(InputAction.CallbackContext context) {
        SetMapEnabled(true);
    }
    private void DisableMap(InputAction.CallbackContext context) {
        SetMapEnabled(false);
    }

    private void OnDestroy() {
        inputActions.Gameplay.Map.started -= EnableMap;
        inputActions.Gameplay.Map.canceled -= DisableMap;
    }
}
