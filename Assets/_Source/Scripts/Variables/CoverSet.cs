using UnityEngine;

[CreateAssetMenu]
public class CoverSet : Set<Cover> {

    public bool PlayerInCompleteCover() {
        foreach (var cover in Items) {
            if (cover.playerInside && cover.type == Cover.CoverType.Complete) return true;
        }

        return false;
    }
    
    public bool PlayerInSparseCover() {
        foreach (var cover in Items) {
            if (cover.playerInside && cover.type == Cover.CoverType.Sparse) return true;
        }

        return false;
    }
}
        