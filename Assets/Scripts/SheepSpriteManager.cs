using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpriteManager : SpriteManager {
    [Header("Idle Sprites")]
    public Sprite idleFrontSprite;
    public Sprite idleBackSprite;
    public Sprite idleLeftSprite;
    public Sprite idleRightSprite;
    [Header("Eating Sprites")]
    public Sprite eatingFrontSprite;
    public Sprite eatingBackSprite;
    public Sprite eatingLeftSprite;
    public Sprite eatingRightSprite;

    private void Start() {
        ShowIdle();
    }

    public void ShowEating() {
        ChangeSprites(eatingFrontSprite, eatingBackSprite, eatingLeftSprite, eatingRightSprite);
    }
    public void ShowIdle() {
        ChangeSprites(idleFrontSprite, idleBackSprite, idleLeftSprite, idleRightSprite);
    }
}
