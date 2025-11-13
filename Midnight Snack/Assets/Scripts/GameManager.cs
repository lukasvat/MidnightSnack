using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;

// This script manages the main story events
public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    // Events
    public bool hasPlayerPickedUpBurger = false;
    public bool hasPlayerUsedMicrowave = false;
    public bool isPowerOut = false;
    public bool knowsFuseIsMissing = false;
    public bool hasTool = false;
    public bool hasFuse = false;
    public bool hasPlayerRestoredPower = false;
    public bool isMonsterActive = false;
    public bool knowsToolIsNeeded = false;

    // Timing
    public float powerOutageDelay = 10f;
    public float monsterSpawnDelay = 10f;

    //Objects
    public GameObject allStoreLights;
    public GameObject bathroomLights;
    public GameObject phoneLight;
    public GameObject bodyToAppear;
    public GameObject monsterAI;
    public MonoBehaviour playerControlScript;
    public MonsterController monsterController;

    // UI
    public TextMeshProUGUI mainCaption;
    public GameObject gameOverUI;
    public GameObject winUI;
    private Coroutine currentCaptionCoroutine = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Set the initial state of the game
        mainCaption.gameObject.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);
        if (allStoreLights != null) allStoreLights.SetActive(true);
        if (bathroomLights != null) bathroomLights.SetActive(true);
        if (bodyToAppear != null) bodyToAppear.SetActive(false);
        if (monsterAI != null) monsterAI.SetActive(false);
        if (phoneLight != null) phoneLight.SetActive(false);

        //Show first caption
        ShowTemporaryMainCaption("I'm starving. Just need to grab a bite real quick.", 5f);
    }

    public void ShowMainCaption(string message)
    {
        // shows a permanent caption
        if (currentCaptionCoroutine != null)
        {
            StopCoroutine(currentCaptionCoroutine);
            currentCaptionCoroutine = null;
        }

        mainCaption.text = message;
        mainCaption.gameObject.SetActive(true);
    }

    // --- Public Event Functions ---
    public void OnBurgerPickedUp()
    {
        if (hasPlayerPickedUpBurger) return;
        hasPlayerPickedUpBurger = true;
        ShowTemporaryMainCaption("This looks good, but I need to heat it up.", 5f);
    }

    public void OnMicrowaveUsed()
    {
        if (hasPlayerUsedMicrowave) return;
        hasPlayerUsedMicrowave = true;
        ShowTemporaryMainCaption("30 seconds should do it.", 5f);

        // Start the 10-second timer
        StartCoroutine(StartPowerOutageTimer());
    }

    public void OnPlayerGotTool()
    {
        hasTool = true;
        ShowTemporaryMainCaption("I need to go back to get the fuse from the bathroom.", 5f);
    }

    public void OnPlayerGotFuse()
    {
        hasFuse = true;
        if (bathroomLights != null) bathroomLights.SetActive(false);
    }

    public void OnPowerRestored()
    {
        if (hasPlayerRestoredPower) return;
        hasPlayerRestoredPower = true;
        isPowerOut = false;

        if (allStoreLights != null) allStoreLights.SetActive(true);
        if (phoneLight != null) phoneLight.SetActive(false);
        if (bodyToAppear != null) bodyToAppear.SetActive(true);

        // Start the monster spawn timer
        StartCoroutine(ActivateMonsterAfterDelay());
    }

    // --- Helper Functions ---
    private IEnumerator StartPowerOutageTimer()
    {
        yield return new WaitForSeconds(powerOutageDelay);
        TriggerPowerOutage();
    }

    private void TriggerPowerOutage()
    {
        isPowerOut = true;
        if (allStoreLights != null) allStoreLights.SetActive(false);
        if (phoneLight != null) phoneLight.SetActive(true);
        ShowTemporaryMainCaption("Damn. Maybe there's a fuse box somewhere.", 5f);
    }
    private IEnumerator ActivateMonsterAfterDelay()
    {
        // Wait for 15 seconds
        yield return new WaitForSeconds(monsterSpawnDelay);
        TriggerMonsterReveal();
    }

    private void TriggerMonsterReveal()
    {
        isMonsterActive = true;
        if (monsterAI != null) monsterAI.SetActive(true);
        ShowTemporaryMainCaption("What the hell is that! I need to get to my car!", 5f);
    }

    /// Shows a story caption that will disappear.
    public void ShowTemporaryMainCaption(string message, float duration)
    {
        // Stop any previous caption coroutine
        if (currentCaptionCoroutine != null)
        {
            StopCoroutine(currentCaptionCoroutine);
        }

        mainCaption.text = message;
        mainCaption.gameObject.SetActive(true);

        // Start the new timer
        currentCaptionCoroutine = StartCoroutine(ClearMainCaptionAfterDelay(duration));
    }

    private IEnumerator ClearMainCaptionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Clear the text and hide the UI
        mainCaption.text = "";
        mainCaption.gameObject.SetActive(false);
        currentCaptionCoroutine = null;
    }

        public void WinGame()
    {
        // Disable player controls
        if (playerControlScript != null)
        {
            playerControlScript.enabled = false;
        }

        // Hide the monster
        if (monsterController != null)
        {
            monsterController.gameObject.SetActive(false);
        }

        // Show the Win UI
        if (winUI != null)
        {
            winUI.SetActive(true);
        }

        // Stop time and unlock cursor
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EndGame()
    {
        // Disable Player Controls
        if (playerControlScript != null)
        {
            playerControlScript.enabled = false;
        }

        // Lock the mouse cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Stop the Monster's Movement
        if (monsterController != null)
        {
            monsterController.enabled = false;
        }

        // Trigger the Jump Scare Action
        StartCoroutine(monsterController.JumpScareSequence());
    }
    
    public void ShowGameOverUI()
    {
        // Show the UI and stop all time
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Stop time
        Time.timeScale = 0f; 

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
    }