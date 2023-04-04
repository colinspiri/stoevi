using UnityEngine;

public class SheepSpriteManager : SpriteManager {
    [Header("Idle Materials")] 
    public Material idleFront;
    public Material idleBack;
    public Material idleLeft;
    public Material idleRight;
    [Header("Eating Materials")] 
    public Material eatingFront;
    public Material eatingBack;
    public Material eatingLeft;
    public Material eatingRight;
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
        ChangeMaterials(eatingFront, eatingBack, eatingLeft, eatingRight);
    }
    public void ShowIdle() {
        ChangeMaterials(idleFront, idleBack, idleLeft, idleRight);
    }
}
