using UnityEngine;

// This script manages the main story events
public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    // Story Event Flags
    public bool hasPlayerPickedUpBurger = false;
    public bool hasPlayerUsedMicrowave = false;
    public bool hasPlayerLeftMoney = false;
    public bool isPowerOut = false;
    public bool hasPlayerRestoredPower = false;
    public bool isMonsterActive = false;

    // Object References
    public GameObject allStoreLights;
    public GameObject bodyToAppear;
    public GameObject monsterAI;
    public InteractableDoor backRoomDoor;

    void Awake()
    {
        // Setup the Singleton
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
        if (bodyToAppear != null) bodyToAppear.SetActive(false);
        if (monsterAI != null) monsterAI.SetActive(false);
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
    }
    
    public void OnMoneyLeft()
    {
        if (hasPlayerLeftMoney) return;
        hasPlayerLeftMoney = true;
    }

    public void OnPowerRestored()
    {
        if (hasPlayerRestoredPower) return;
        hasPlayerRestoredPower = true;
        isPowerOut = false;
    }


    // --- Helper Functions ---
    private void TriggerPowerOutage()
    {
        isPowerOut = true;
        // Power outage logic goes here
    }

    private void TriggerMonsterReveal()
    {
        isMonsterActive = true;
        // Monster logic goes here
    }
}