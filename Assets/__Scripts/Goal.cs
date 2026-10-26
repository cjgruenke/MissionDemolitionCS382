using UnityEngine;

// Attached to the Goal prefab. Detects when a projectile enters it.
public class Goal : MonoBehaviour {
    static public bool goalMet = false;

    void OnTriggerEnter(Collider other) {
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null) {
            goalMet = true;

            // Make the goal more see-through to show it was hit
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 0.75f;
            mat.color = c;
        }
    }
}