using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameMode { idle, playing, levelEnd, gameOver }

// Attached to GameManager. Runs levels, counts shots, and handles Game Over and Play Again.
public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S;

    [Header("Set in Inspector")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI shotsText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverStatsText;
    public Vector3 castlePos = new Vector3(50f, -9.5f, 0f);
    public GameObject[] castles;              // Ordered easy to hard

    [Header("Set Dynamically")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;

    void Awake() {
        S = this;
    }

    void Start() {
        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        gameOverPanel.SetActive(false);
        StartLevel();
    }

    void StartLevel() {
        // Clear the previous level
        if (castle != null) Destroy(castle);
        Projectile.DESTROY_PROJECTILES();
        ProjectileLine.CLEAR();

        // Build the new castle
        castle = Instantiate(castles[level], castlePos, Quaternion.identity);

        Goal.goalMet = false;
        mode = GameMode.playing;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
        UpdateGUI();
    }

    void Update() {
        UpdateGUI();

        if (mode == GameMode.playing && Goal.goalMet) {
            mode = GameMode.levelEnd;
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            Invoke(nameof(NextLevel), 2f);
        }
    }

    void NextLevel() {
        level++;
        if (level >= levelMax) {
            GameOver();
        } else {
            StartLevel();
        }
    }

    void GameOver() {
        mode = GameMode.gameOver;

        // Save the best (lowest) shot count between sessions
        int best = PlayerPrefs.GetInt("BestShots", 0);
        if (best == 0 || shotsTaken < best) {
            best = shotsTaken;
            PlayerPrefs.SetInt("BestShots", best);
        }

        gameOverStatsText.text = "You knocked down all " + levelMax + " castles in "
            + shotsTaken + " shots!\nBest: " + best + " shots";
        gameOverPanel.SetActive(true);
    }

    void UpdateGUI() {
        int shownLevel = Mathf.Min(level + 1, levelMax);
        levelText.text = "Level: " + shownLevel + " of " + levelMax;
        shotsText.text = "Shots Taken: " + shotsTaken;
    }

    // Hooked up to PlayAgainButton's On Click ()
    public void PlayAgain() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    static public void SHOT_FIRED() {
        S.shotsTaken++;
    }

    static public GameObject GET_CASTLE() {
        return S.castle;
    }

    static public bool CAN_SHOOT {
        get { return S != null && S.mode == GameMode.playing; }
    }
}