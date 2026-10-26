using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

// Attached to Main Camera. Follows the point of interest (POI) and switches views.
public class FollowCam : MonoBehaviour {
    static public GameObject POI;         // What the camera is following; null = slingshot
    static private FollowCam S;

    public enum eView { slingshot, castle, both }

    [Header("Set in Inspector")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    public GameObject viewBothGO;
    public TextMeshProUGUI viewButtonText;

    [Header("Set Dynamically")]
    public float camZ;
    public eView nextView = eView.castle;

    void Awake() {
        S = this;
        POI = null;
        camZ = transform.position.z;
    }

    void FixedUpdate() {
        Vector3 destination = Vector3.zero;   // Slingshot view

        // Stop following a projectile once it comes to rest
        if (POI != null) {
            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if (poiRigid != null && poiRigid.IsSleeping()) {
                POI = null;
            }
        }

        if (POI != null) {
            destination = POI.transform.position;
        }

        // Never go below or left of minXY
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        // Ease toward the destination
        destination = Vector3.Lerp(transform.position, destination, easing);
        destination.z = camZ;
        transform.position = destination;

        // Zoom out as the camera rises, so the ground stays in view
        Camera.main.orthographicSize = destination.y + 10;
    }

    // Hooked up to ViewButton's On Click ()
    public void CycleView() {
        SwitchView(nextView);
        // Deselect the button so pressing Space doesn't click it again
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    static public void SWITCH_VIEW(eView newView) {
        if (S != null) S.SwitchView(newView);
    }

    void SwitchView(eView newView) {
        switch (newView) {
            case eView.slingshot:
                POI = null;
                nextView = eView.castle;
                break;
            case eView.castle:
                POI = MissionDemolition.GET_CASTLE();
                nextView = eView.both;
                break;
            case eView.both:
                POI = viewBothGO;
                nextView = eView.slingshot;
                break;
        }

        if (viewButtonText != null) {
            if (nextView == eView.slingshot) viewButtonText.text = "Show Slingshot";
            else if (nextView == eView.castle) viewButtonText.text = "Show Castle";
            else viewButtonText.text = "Show Both";
        }
    }
}