using BehaviorDesigner.Runtime;

[System.Serializable]
public class SharedCrop : SharedVariable<Crop>
{
    public static implicit operator SharedCrop(Crop value) { return new SharedCrop { Value = value }; }
}
