using UnityEngine;

// Attached to CloudCrafter. Spawns clouds and drifts them across the sky.
public class CloudCrafter : MonoBehaviour {
    [Header("Set in Inspector")]
    public GameObject cloudPrefab;
    public int numClouds = 40;
    public Vector3 cloudPosMin = new Vector3(-50, -5, 10);
    public Vector3 cloudPosMax = new Vector3(150, 100, 10);
    public float cloudScaleMin = 1f;
    public float cloudScaleMax = 3f;
    public float cloudSpeedMult = 0.5f;

    private GameObject[] cloudInstances;

    void Awake() {
        cloudInstances = new GameObject[numClouds];
        for (int i = 0; i < numClouds; i++) {
            GameObject cloud = Instantiate(cloudPrefab, transform);

            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(cloudPosMin.x, cloudPosMax.x);
            pos.y = Random.Range(cloudPosMin.y, cloudPosMax.y);

            // Bigger clouds are lower and closer; smaller clouds are higher and farther away
            float scaleU = Random.value;
            float scaleVal = Mathf.Lerp(cloudScaleMin, cloudScaleMax, scaleU);
            pos.y = Mathf.Lerp(cloudPosMin.y, pos.y, scaleU);
            pos.z = 100 - 90 * scaleU;

            cloud.transform.position = pos;
            cloud.transform.localScale = Vector3.one * scaleVal;
            cloudInstances[i] = cloud;
        }
    }

    void Update() {
        foreach (GameObject cloud in cloudInstances) {
            float scaleVal = cloud.transform.localScale.x;
            Vector3 pos = cloud.transform.position;

            // Bigger (closer) clouds move faster
            pos.x -= scaleVal * Time.deltaTime * cloudSpeedMult;

            // Wrap around to the right side
            if (pos.x <= cloudPosMin.x) {
                pos.x = cloudPosMax.x;
            }
            cloud.transform.position = pos;
        }
    }
}