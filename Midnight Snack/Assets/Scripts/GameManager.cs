using UnityEngine;
using System.Collections;

// This script manages the main story events
public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    public bool hasPlayerPickedUpBurger = false;
    public bool hasPlayerUsedMicrowave = false;
    public bool isPowerOut = false;
    public bool knowsFuseIsMissing = false;
    public bool hasTool = false;
    public bool hasFuse = false;
    public bool hasPlayerRestoredPower = false;
    public bool isMonsterActive = false;
    public bool knowsToolIsNeeded = false;
    public float powerOutageDelay = 10f;
    public float monsterSpawnDelay = 15f;
    public GameObject allStoreLights;
    public GameObject bathroomLights;
    public GameObject phoneLight;
    public GameObject bodyToAppear;
    public GameObject monsterAI;
    public GameObject monsterNavMesh;
    public InteractableDoor backRoomDoor;

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
        if (allStoreLights != null) allStoreLights.SetActive(true);
        if (bathroomLights != null) bathroomLights.SetActive(true);
        if (bodyToAppear != null) bodyToAppear.SetActive(false);
        if (monsterAI != null) monsterAI.SetActive(false);
        if (phoneLight != null) phoneLight.SetActive(false);
        if (monsterNavMesh != null) monsterNavMesh.SetActive(true);
    }

    // --- Public Event Functions ---
    public void OnBurgerPickedUp()
    {
        if (hasPlayerPickedUpBurger) return;
        hasPlayerPickedUpBurger = true;
    }

    public void OnMicrowaveUsed()
    {
        if (hasPlayerUsedMicrowave) return;
        hasPlayerUsedMicrowave = true;
        
        // Start the 10-second timer
        StartCoroutine(StartPowerOutageTimer());
    }
    
    public void OnPlayerGotTool()
    {
        hasTool = true;
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
    }
}