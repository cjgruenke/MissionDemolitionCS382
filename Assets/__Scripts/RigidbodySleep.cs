using UnityEngine;

// Attached to Wall and Slab. Keeps castle pieces asleep for the first few
// physics steps so the castle doesn't wobble or collapse when it appears.
[RequireComponent(typeof(Rigidbody))]
public class RigidbodySleep : MonoBehaviour {
    private int sleepCountdown = 4;
    private Rigidbody rigid;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (sleepCountdown > 0) {
            rigid.Sleep();
            sleepCountdown--;
        }
    }
}