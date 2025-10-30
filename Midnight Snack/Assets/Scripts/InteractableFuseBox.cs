using UnityEngine;

public class InteractableFuseBox : MonoBehaviour
{
    public enum FuseBoxType 
    { 
        BackRoom,
        Bathroom
    }
    public FuseBoxType boxType;

    public string GetPrompt()
    {
        return "Press [E] to inspect fuse box";
    }

    // Called by PlayerInteraction.cs when E is pressed
    public void Interact(PlayerInteraction player)
    {
        GameManager gm = GameManager.Instance;

        switch (boxType)
        {
            case FuseBoxType.BackRoom:
                if (gm.isPowerOut && gm.hasFuse)
                {
                    // Restore Power
                    gm.OnPowerRestored();
                    player.ShowPrompt("Power restored!");
                    this.enabled = false;
                }
                else if (gm.isPowerOut && !gm.hasFuse)
                {
                    // Set the story flag
                    gm.knowsFuseIsMissing = true;
                    player.ShowPrompt("A fuse is missing... Maybe the bathroom has one.");
                }
                else
                {
                    // Power is on
                    player.ShowPrompt("Power seems to be working fine.");
                }
                break;

            case FuseBoxType.Bathroom:
                if (gm.knowsFuseIsMissing && !gm.hasFuse && gm.hasTool)
                {
                    // Get the fuse
                    gm.OnPlayerGotFuse();
                    player.ShowPrompt("Got the fuse!");
                    player.DropAndDestroyHeldItem();
                    this.enabled = false; // Disable this box
                }
                else if (gm.knowsFuseIsMissing && !gm.hasTool)
                {
                    // Remind player
                    player.ShowPrompt("It's locked. I need to find a tool.");
                    gm.knowsToolIsNeeded = true;
                }
                else
                {
                    // Player doesn't need to be here
                    player.ShowPrompt("Just a fuse box.");
                }
                break;
        }
    }
}