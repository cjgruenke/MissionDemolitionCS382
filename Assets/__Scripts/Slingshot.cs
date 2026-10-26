using UnityEngine;

// Attached to Slingshot. Handles aiming, launching, the rubber band, and the snap sound.
public class Slingshot : MonoBehaviour {
    [Header("Set in Inspector")]
    public GameObject projectilePrefab;
    public GameObject projLinePrefab;
    public float velocityMult = 10f;
    public LineRenderer rubberBand;      // Line Renderer on the RubberBand child
    public Transform leftTip;            // Left end of the band
    public Transform rightTip;           // Right end of the band
    public AudioClip snapClip;           // Played when the projectile is released

    [Header("Set Dynamically")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private float maxMagnitude;
    private AudioSource audioSource;

    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        // How far the projectile can be pulled back
        maxMagnitude = GetComponent<SphereCollider>().radius;
        audioSource = GetComponent<AudioSource>();

        // Rubber band: 3 points (left tip, projectile, right tip)
        rubberBand.positionCount = 3;
        rubberBand.startWidth = 0.2f;
        rubberBand.endWidth = 0.2f;
        Color bandColor = new Color(0.35f, 0.2f, 0.1f);
        rubberBand.startColor = bandColor;
        rubberBand.endColor = bandColor;
    }

    void OnMouseEnter() {
        launchPoint.SetActive(true);
    }

    void OnMouseExit() {
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {
        // Only one shot at a time, and only while a level is being played
        if (aimingMode || !MissionDemolition.CAN_SHOOT) return;

        aimingMode = true;
        projectile = Instantiate(projectilePrefab);
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update() {
        if (aimingMode && projectile != null) {
            // Convert the mouse position to world space
            Vector3 mousePos2D = Input.mousePosition;
            mousePos2D.z = -Camera.main.transform.position.z;
            Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

            // Limit how far the projectile can be pulled
            Vector3 mouseDelta = mousePos3D - launchPos;
            mouseDelta.z = 0;
            if (mouseDelta.magnitude > maxMagnitude) {
                mouseDelta = mouseDelta.normalized * maxMagnitude;
            }
            projectile.transform.position = launchPos + mouseDelta;

            if (Input.GetMouseButtonUp(0)) {
                Release(mouseDelta);
            }
        }

        UpdateRubberBand();
    }

    void Release(Vector3 mouseDelta) {
        aimingMode = false;

        // Launch in the opposite direction of the pull
        Projectile proj = projectile.GetComponent<Projectile>();
        proj.Launch(-mouseDelta * velocityMult);

        // Camera follows the new projectile
        FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
        FollowCam.POI = projectile;

        // Draw a trail behind it
        GameObject lineGO = Instantiate(projLinePrefab);
        lineGO.GetComponent<ProjectileLine>().Track(projectile.transform);

        // Snap sound
        if (snapClip != null) audioSource.PlayOneShot(snapClip);

        projectile = null;
        MissionDemolition.SHOT_FIRED();
    }

    // Draws the band from the left tip, through the projectile (if one is loaded), to the right tip
    void UpdateRubberBand() {
        Vector3 towardCamera = new Vector3(0, 0, -0.3f);   // Keeps the band in front of the arms
        Vector3 middle;
        if (projectile != null) {
            middle = projectile.transform.position;
        } else {
            middle = (leftTip.position + rightTip.position) * 0.5f;
        }
        rubberBand.SetPosition(0, leftTip.position + towardCamera);
        rubberBand.SetPosition(1, middle + towardCamera);
        rubberBand.SetPosition(2, rightTip.position + towardCamera);
    }
}