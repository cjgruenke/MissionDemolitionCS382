using System.Collections.Generic;
using UnityEngine;

// Attached to the ProjectileLine prefab. Draws a trail behind one projectile.
[RequireComponent(typeof(LineRenderer))]
public class ProjectileLine : MonoBehaviour {
    static private List<ProjectileLine> LINES = new List<ProjectileLine>();

    public float minDist = 0.1f;          // Minimum distance between trail points

    private LineRenderer line;
    private Transform target;
    private List<Vector3> points = new List<Vector3>();

    void Awake() {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = 0.15f;
        line.endWidth = 0.15f;
        Color dark = new Color(0.15f, 0.15f, 0.15f, 1f);
        line.startColor = dark;
        line.endColor = dark;

        // Fade every older trail
        Color faded = new Color(0.15f, 0.15f, 0.15f, 0.3f);
        foreach (ProjectileLine pl in LINES) {
            if (pl != null) {
                pl.line.startColor = faded;
                pl.line.endColor = faded;
            }
        }
        LINES.Add(this);
    }

    void OnDestroy() {
        LINES.Remove(this);
    }

    // Called by Slingshot right after launch
    public void Track(Transform t) {
        target = t;
        AddPoint();
    }

    void FixedUpdate() {
        if (target == null) return;   // Projectile was destroyed (exploded or fell off)

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null && rb.IsSleeping()) {
            target = null;            // Stop drawing once the projectile stops
            return;
        }
        AddPoint();
    }

    void AddPoint() {
        Vector3 p = target.position;
        if (points.Count > 0 && (p - points[points.Count - 1]).magnitude < minDist) return;
        points.Add(p);
        line.positionCount = points.Count;
        line.SetPosition(points.Count - 1, p);
    }

    // Called by MissionDemolition at the start of each level
    static public void CLEAR() {
        foreach (ProjectileLine pl in LINES.ToArray()) {
            if (pl != null) Destroy(pl.gameObject);
        }
        LINES.Clear();
    }
}