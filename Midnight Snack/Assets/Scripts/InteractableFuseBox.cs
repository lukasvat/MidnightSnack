using UnityEngine;

public class InteractableFuseBox : MonoBehaviour
{
    public enum FuseBoxType 
    { 
        BackRoom,
        Bathroom
    }
    public FuseBoxType boxType;

    // Called by PlayerInteraction.cs
    public void Interact(PlayerInteraction player)
    {
        // Use GameManager to check story flags
        GameManager gm = GameManager.Instance;

        switch (boxType)
        {
            case FuseBoxType.BackRoom:
                if (!gm.isPowerOut)
                {
                    player.ShowPrompt("Power seems to be working fine.");
                }
                else if (gm.hasFuse)
                {
                    player.ShowPrompt("Fuse inserted. Power is back on!");
                    gm.OnPowerRestored();
                    // Disable this so it can't be used again
                    this.enabled = false;
                }
                else
                {
                    // Power is out, player has no fuse
                    player.ShowPrompt("A fuse is missing... Maybe the bathroom has one.");
                    gm.knowsFuseIsMissing = true;
                }
                break;

            case FuseBoxType.Bathroom:
                if (!gm.knowsFuseIsMissing)
                {
                    player.ShowPrompt("Just a fuse box.");
                }
                else if (gm.hasFuse)
                {
                    player.ShowPrompt("I already got the fuse from here.");
                }
                else if (!gm.hasTool)
                {
                    // Player knows fuse is missing, but has no tool
                    player.ShowPrompt("It's locked. I need something to break it open.");
                }
                else
                {
                    // Player has tool
                    player.ShowPrompt("Got the fuse!");
                    gm.OnPlayerGotFuse();
                    // Disable this so it can't be used again
                    this.enabled = false;
                }
                break;
        }
    }
}