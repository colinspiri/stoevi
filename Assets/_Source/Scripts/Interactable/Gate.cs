
public class Gate : Interactable {
    
    public override void InteractPrimary() {
        GameManager.Instance.GameOver(true);
    }

    public override string GetObjectName() {
        return "gate";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPromptPrimary() {
        return GetInteractPrimaryButton() + " go home";
    }
}
