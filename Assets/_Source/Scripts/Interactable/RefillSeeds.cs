
public class RefillSeeds : Interactable
{
    public override bool IsInteractablePrimary() {
        return false;
    }

    public override void InteractPrimary() {
        
    }

    public override string GetObjectName() {
        return "seed crate";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPromptPrimary() {
        return "temporarily disabled";
    }
}
