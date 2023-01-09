using System.Collections;
using System.Collections.Generic;
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
}
