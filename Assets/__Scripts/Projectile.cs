using System.Collections.Generic;
using UnityEngine;

// Attached to the Projectile prefab. Tracks all projectiles and handles detonation.
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    static private List<Projectile> PROJECTILES = new List<Projectile>();
    static public Projectile CURRENT;    // The most recently launched projectile

    [Header("Explosion (Set in Inspector)")]
    public float explosionRadius = 6f;
    public float explosionForce = 25f;
    public float upwardsModifier = 1f;
    public GameObject explosionPrefab;   // Particle effect
    public AudioClip explosionClip;

    private Rigidbody rigid;
    private bool launched = false;
    private bool exploded = false;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        PROJECTILES.Add(this);
    }

    void OnDestroy() {
        PROJECTILES.Remove(this);
    }

    // Called by Slingshot when the player lets go
    public void Launch(Vector3 velocity) {
        rigid.isKinematic = false;
        rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigid.velocity = velocity;
        launched = true;
        CURRENT = this;
    }

    void Update() {
        // Clean up projectiles that fall off the world
        if (transform.position.y < -50f) {
            Destroy(gameObject);
            return;
        }

        // Only the newest projectile can be detonated, only once, and only while it is moving
        if (!launched || exploded || CURRENT != this) return;
        if (rigid.IsSleeping()) return;

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
            Explode();
        }
    }

    void Explode() {
        exploded = true;
        Vector3 pos = transform.position;

        // Push every rigidbody within the blast radius
        Collider[] hits = Physics.OverlapSphere(pos, explosionRadius);
        foreach (Collider hit in hits) {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && rb != rigid) {
                rb.WakeUp();
                rb.AddExplosionForce(explosionForce, pos, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }

        // Particle effect; the camera watches it until it finishes
        if (explosionPrefab != null) {
            GameObject fx = Instantiate(explosionPrefab, pos, Quaternion.identity);
            if (FollowCam.POI == gameObject) FollowCam.POI = fx;
        }

        // Sound, played at the camera so it is always audible
        if (explosionClip != null) {
            AudioSource.PlayClipAtPoint(explosionClip, Camera.main.transform.position);
        }

        Destroy(gameObject);
    }

    // Called by MissionDemolition at the start of each level
    static public void DESTROY_PROJECTILES() {
        foreach (Projectile p in PROJECTILES.ToArray()) {
            if (p != null) Destroy(p.gameObject);
        }
        CURRENT = null;
    }
}