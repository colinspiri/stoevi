using System.Collections;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    // misc components
    private InputActions inputActions;
    private Light light;
    public ASoundContainer sfx_flashlight_on;
    public ASoundContainer sfx_flashlight_off;
    
    [Header("Follow")]
    public GameObject followObject;
    public float lerpSpeed;
    
    [Header("Battery")] 
    public FloatReference fullBatteryTime;
    public FloatVariable batteryPercent;
    public IntVariable additionalBatteries;
    // state
    private float currentBatteryTime;

    [Header("Flicker")] 
    public float flickerBatteryPercent;
    public float minFlickerTime;
    public float maxFlickerTime;
    public float lowBatteryPercent;
    public float lowBatteryMultiplier;
    public float minFlickerDelay;
    public float maxFlickerDelay;
    public float flickerOnTime;
    // state
    private bool flashlightOn;
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
        TurnOff(false);
        
        // get battery time from percent SO
        if (batteryPercent.Value <= 0) {
            if (additionalBatteries.Value >= 1) {
                batteryPercent.Value = 1;
            }
        }
        currentBatteryTime = Mathf.Lerp(0, fullBatteryTime.Value, batteryPercent.Value);
    }

    // Update is called once per frame
    void Update() {
        // update position & rotation
        transform.position = followObject.transform.position;
        transform.rotation =
            Quaternion.Lerp(transform.rotation, followObject.transform.rotation, lerpSpeed * Time.deltaTime);

        // decrease battery
        if (flashlightOn) {
            currentBatteryTime -= Time.deltaTime;
            
            batteryPercent.SetValue(currentBatteryTime / fullBatteryTime);

            if (batteryPercent.Value <= 0) {
                batteryPercent.SetValue(0);
                additionalBatteries.ApplyChange(-1);
                TurnOff();
            }

            // Debug.Log("battery = " + batteryPercent.Value*100f + " with " + additionalBatteries.Value + " additional batteries");
        }
        
        // flicker
        if (flashlightOn && batteryPercent.Value <= flickerBatteryPercent) {
            // count flicker timer
            flickerTimer -= Time.deltaTime;
            
            // low battery
            if (batteryPercent.Value <= lowBatteryPercent && flickerTimer >= maxFlickerTime*lowBatteryMultiplier) {
                flickerTimer = 0;
            } 

            // flicker
            if (flickerTimer <= 0) {
                flickerCoroutine = StartCoroutine(Flicker());
                
                // reset timer
                flickerTimer = Random.Range(minFlickerTime, maxFlickerTime);
                if (batteryPercent.Value <= lowBatteryPercent) {
                    flickerTimer *= lowBatteryMultiplier;
                }
            }
        }
    }
    
    private void ToggleFlashlight(InputAction.CallbackContext context) {
        if (flashlightOn) {
            TurnOff();
        }
        else {
            // check battery percent
            if (batteryPercent.Value <= 0) {
                // check if have batteries
                if (additionalBatteries.Value >= 1) {
                    currentBatteryTime = fullBatteryTime;
                }
                // no more batteries
                else return;
            }
            
            TurnOn();
        }
    }
    
    private void TurnOn() {
        flashlightOn = true;
        light.enabled = true;
        sfx_flashlight_on.Play();
    }
    private void TurnOff(bool playSound = true) {
        flashlightOn = false;
        light.enabled = false;
        if (flickerCoroutine != null) {
            StopCoroutine(flickerCoroutine);
        }
        sfx_flashlight_off.Play();
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
