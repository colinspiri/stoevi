using SpookuleleAudio;

public class Well : Interactable {
    // components
    public ASoundContainer well;
    
    // shared state
    public IntVariable maxWater;
    public IntVariable currentWater;
    
    public override void InteractPrimary() {
        currentWater.SetValue(maxWater);
    }

    public override void InteractSecondary() {
        throw new System.NotImplementedException();
    }

    public override void OnStartInteractingPrimary() {
        base.OnStartInteractingPrimary();
        well.Play3D(transform);
    }

    public override string GetObjectName() {
        return "well";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPromptPrimary() {
        return (currentWater.Value == maxWater.Value) ? "water is already full" : "hold " + GetInteractPrimaryButton() + " to refill water";
    }

    public override bool IsInteractablePrimary() {
        return currentWater.Value < maxWater.Value;
    }
}
