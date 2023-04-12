using UnityEngine;

[CreateAssetMenu]
public class CoverSet : Set<Cover> {

    public bool PlayerHiddenByCompleteCover() {
        foreach (var cover in Items) {
            if (cover.type == Cover.CoverType.Complete && cover.playerInside && cover.torbalanInside == false) return true;
        }

        return false;
    }
    
    public bool PlayerHiddenBySparseCover() {
        foreach (var cover in Items) {
            if (cover.type == Cover.CoverType.Sparse && cover.playerInside && cover.torbalanInside == false) return true;
        }

        return false;
    }
}
        