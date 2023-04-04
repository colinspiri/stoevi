using System.Collections;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    // components
    private InputActions inputActions;
    private Light light;
    public GameObject followObject;
    public ASoundContainer sfx_flashlight_on;
    public ASoundContainer sfx_flashlight_off;

    // constants
    public float lerpSpeed;
    public float minFlickerTime;
    public float maxFlickerTime;
    public float minFlickerDelay;
    public float maxFlickerDelay;
    public float flickerOnTime;

    // state
    private bool flashlightOn;
    private float nextFlickerTime;
    private float flickerTimer;
    private Coroutine flickerCoroutine;


    private void Awake() {
        light = GetComponent<Light>();
        
        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Gameplay.Flashlight.performed += ToggleFlashlight;
    }

    // Start is called before the first frame update
    void Start() {
        light.enabled = false;

        nextFlickerTime = Random.Range(minFlickerTime, maxFlickerTime);
    }

    // Update is called once per frame
    void Update() {
        // update position & rotation
        transform.position = followObject.transform.position;
        transform.rotation =
            Quaternion.Lerp(transform.rotation, followObject.transform.rotation, lerpSpeed * Time.deltaTime);
        
        // flicker
        if (flashlightOn) {
            flickerTimer += Time.deltaTime;
            if (flickerTimer >= nextFlickerTime) {
                flickerCoroutine = StartCoroutine(Flicker());
                flickerTimer = 0;
                nextFlickerTime = Random.Range(minFlickerTime, maxFlickerTime);
            }
        }
        
    }

    private void ToggleFlashlight(InputAction.CallbackContext context) {
        if (flashlightOn) {
            // turn off
            flashlightOn = false;
            light.enabled = false;
            if (flickerCoroutine != null) {
                StopCoroutine(flickerCoroutine);
            }
            sfx_flashlight_off.Play();
        }
        else {
            // turn on
            flashlightOn = true;
            light.enabled = true;
            sfx_flashlight_on.Play();
        }
    }

    private IEnumerator Flicker() {
        int reps = Random.Range(1, 3);

        light.enabled = false;
        
        yield return new WaitForSeconds(Random.Range(minFlickerDelay, maxFlickerDelay));

        for (int i = 0; i < reps; i++) {
            light.enabled = true;
            yield return new WaitForSeconds(flickerOnTime);
            light.enabled = false;

            var flickerDelay = Random.Range(minFlickerDelay, maxFlickerDelay);
            yield return new WaitForSeconds(flickerDelay);
        }
        
        // fade up
        light.enabled = true;
    }

    private void OnDestroy() {
        inputActions.Gameplay.Flashlight.performed -= ToggleFlashlight;
    }
}
