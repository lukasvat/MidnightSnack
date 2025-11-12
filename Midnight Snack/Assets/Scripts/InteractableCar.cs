using UnityEngine;

public class InteractableCar : MonoBehaviour
{
    public string GetPrompt()
    {
        return "Press [E] to escape!";
    }

    // Called by PlayerInteraction.cs when E is pressed
    public void Interact()
    {
        // Only allow winning if the monster is active
        if (GameManager.Instance.isMonsterActive)
        {
            GameManager.Instance.WinGame();
        }
    }
}