using UnityEngine;

public static class Util {
    
    public static string FormatTimer(float timer) {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public static Vector3 RandomPointInRadius(Vector3 center, float maxRadius, float minRadius = 0f) {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float radius = Random.Range(minRadius, maxRadius);
        Vector3 randomPoint = center + new Vector3(randomDirection.x, 0, randomDirection.y) * radius;

        return randomPoint;
    }

    public static void PlaceGameObjectOnTerrain(GameObject go, string method) {
        // Cast a ray and get all hits
        RaycastHit[] rgHits = Physics.RaycastAll( go.transform.position, -Vector3.up, Mathf.Infinity );

        // We can assume we did not hit the current game object, since a ray cast from within the collider will implicitly ignore that collision
        int iBestHit = -1;
        float flDistanceToClosestCollider = Mathf.Infinity;
        for( int iHit = 0; iHit < rgHits.Length; ++iHit )
        {
            RaycastHit CurHit = rgHits[ iHit ];

            // Assume we want the closest hit
            if ( CurHit.distance > flDistanceToClosestCollider )
                continue;

            // Cache off the best hit
            iBestHit = iHit;
            flDistanceToClosestCollider = CurHit.distance;
        }

        // Did we find something?
        if ( iBestHit < 0 )
        {
            Debug.LogWarning( "Failed to find an object on which to place the game object " + go.name + "." );
            return;
        }

        // Grab the best hit
        RaycastHit BestHit = rgHits[ iBestHit ];
        
        // calculate offset based on settings
        float yOffset = 0f;
        Bounds bounds = GetBoundsForGameObjectHierarchy(go);
        switch (method)
        {
            case "Bottom":
                yOffset = go.transform.position.y - bounds.min.y;
                break;
            case "Origin":
                yOffset = 0f;
                break;
            case "Center":
                yOffset = bounds.center.y - go.transform.position.y;
                break;
        }

        // Set position
        go.transform.position = new Vector3( BestHit.point.x, BestHit.point.y + yOffset, BestHit.point.z );
        
        // Set rotation
        go.transform.rotation = Quaternion.FromToRotation( Vector3.up, BestHit.normal );
    }
    
    public static Bounds GetBoundsForGameObjectHierarchy( GameObject go )
    {
        Bounds bounds = new Bounds();
        bounds.center = go.transform.position;
        bounds.extents = Vector3.zero;
 
        // Deal with parent and all descendents (GetComponentsInChildren() gets everything)
        Renderer[] rgDescendentRenderers = go.GetComponentsInChildren< Renderer >();
        foreach( Renderer renderer in rgDescendentRenderers  )
        {
            if ( !renderer )
                continue;
 
            bounds.Encapsulate( renderer.bounds );
        }
 
        return bounds;
    }
}
