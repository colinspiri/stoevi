
public class Gate : Interactable {
    public IntReference objectiveComplete;

    public override bool IsInteractablePrimary() {
        return objectiveComplete.Value == 1;
    }

    public override void InteractPrimary() {
        if (objectiveComplete.Value == 1) {
            GameManager.Instance.EndDay();
        }
    }

    public override string GetObjectName() {
        return "gate";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPromptPrimary() {
        if (objectiveComplete.Value == 1) {
            return GetInteractPrimaryButton() + " go home";
        }
        else {
            return "must complete objective";
        }
    }
}
