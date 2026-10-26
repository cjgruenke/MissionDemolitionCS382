using UnityEngine;

// Attached to the Cloud prefab. Builds a random cloud out of CloudSphere copies.
public class Cloud : MonoBehaviour {
    [Header("Set in Inspector")]
    public GameObject cloudSphere;
    public int sphereCountMin = 3;
    public int sphereCountMax = 6;
    public Vector3 sphereOffsetScale = new Vector3(5, 2, 1);
    public Vector2 sphereScaleRangeX = new Vector2(4, 8);
    public Vector2 sphereScaleRangeY = new Vector2(3, 4);
    public Vector2 sphereScaleRangeZ = new Vector2(2, 4);
    public float scaleYMin = 2f;

    void Start() {
        int count = Random.Range(sphereCountMin, sphereCountMax + 1);
        for (int i = 0; i < count; i++) {
            GameObject sp = Instantiate(cloudSphere, transform);

            // Random position within an ellipsoid
            Vector3 offset = Random.insideUnitSphere;
            offset.x *= sphereOffsetScale.x;
            offset.y *= sphereOffsetScale.y;
            offset.z *= sphereOffsetScale.z;
            sp.transform.localPosition = offset;

            // Random size; spheres near the edges are flatter
            Vector3 scale = new Vector3(
                Random.Range(sphereScaleRangeX.x, sphereScaleRangeX.y),
                Random.Range(sphereScaleRangeY.x, sphereScaleRangeY.y),
                Random.Range(sphereScaleRangeZ.x, sphereScaleRangeZ.y));
            scale.y *= 1 - (Mathf.Abs(offset.x) / sphereOffsetScale.x);
            scale.y = Mathf.Max(scale.y, scaleYMin);
            sp.transform.localScale = scale;
        }
    }
}