using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Attach this to an empty GameObject called "GameManager".
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References (assign in Inspector)")]
    public Text scoreText;

    [Header("FPS Win UI (assign in Inspector)")]
    public GameObject winPanel;
    public Text winText;

    int score = 0;
    bool isGameOver = false;
    readonly HashSet<EnemyNpc> registeredNpcs = new HashSet<EnemyNpc>();
    readonly HashSet<EnemyNpc> defeatedNpcs = new HashSet<EnemyNpc>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (winPanel != null) winPanel.SetActive(false);
        UpdateScoreUI();
    }

    public void RegisterNpc(EnemyNpc npc)
    {
        if (npc != null)
            registeredNpcs.Add(npc);
    }

    public void NpcKilled(EnemyNpc npc)
    {
        if (isGameOver || npc == null || !defeatedNpcs.Add(npc))
            return;

        score++;
        UpdateScoreUI();

        if (registeredNpcs.Count > 0 && score >= registeredNpcs.Count)
            WinGame();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    void WinGame()
    {
        isGameOver = true;
        FpsPlayerController playerController = FindFirstObjectByType<FpsPlayerController>();
        FpsMouseLook mouseLook = FindFirstObjectByType<FpsMouseLook>();
        Pistol pistol = FindFirstObjectByType<Pistol>();
        WeaponSwitcher weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();

        if (playerController != null) playerController.enabled = false;
        if (mouseLook != null) mouseLook.enabled = false;
        if (pistol != null) pistol.enabled = false;
        if (weaponSwitcher != null) weaponSwitcher.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (winPanel != null) winPanel.SetActive(true);
        if (winText != null) winText.text = "U WIN";
    }

}
