using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpriteManager : MonoBehaviour {
    // components
    public SpriteRenderer spriteRenderer;
    private Camera cam;
    
    // state
    private Sprite frontSprite;
    private Sprite backSprite;
    private Sprite leftSprite;
    private Sprite rightSprite;

    protected void Awake() {
        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    protected void Update() {
        Vector3 toTarget = cam.transform.position - transform.position;
        toTarget.Normalize();
        float dot = Vector3.Dot(toTarget, transform.forward);

        if (dot < -0.75) {
            spriteRenderer.sprite = backSprite;
        }
        else if (dot < 0.75) {
            float angle = Vector3.SignedAngle(toTarget, transform.forward, Vector3.up);
            spriteRenderer.sprite = angle < 0 ? leftSprite : rightSprite;
        }
        else {
            spriteRenderer.sprite = frontSprite;
        }
    }

    protected void ChangeSprites(Sprite newFront, Sprite newBack, Sprite newLeft, Sprite newRight) {
        frontSprite = newFront;
        backSprite = newBack;
        leftSprite = newLeft;
        rightSprite = newRight;
    }
}
