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

    // Called by PlayerInteraction.cs
    public void Interact(PlayerInteraction player)
    {
        GameManager gm = GameManager.Instance;
        float captionDuration = 5f;

        switch (boxType)
        {
            case FuseBoxType.BackRoom:
                if (gm.isPowerOut && gm.hasFuse)
                {
                    // Restore Power
                    gm.OnPowerRestored();
                    this.enabled = false;
                }
                else if (gm.isPowerOut && !gm.hasFuse)
                {
                    // Set the story flag
                    gm.knowsFuseIsMissing = true;
                    gm.ShowTemporaryMainCaption("A fuse is missing... Maybe the bathroom has one.", captionDuration);
                }
                else
                {
                    // Power is on
                    gm.ShowTemporaryMainCaption("Power seems to be working fine.", captionDuration);
                }
                break;

            case FuseBoxType.Bathroom:
                if (gm.knowsFuseIsMissing && !gm.hasFuse && gm.hasTool)
                {
                    // Get the fuse
                    gm.OnPlayerGotFuse();
                    gm.ShowTemporaryMainCaption("Got the fuse!", captionDuration);
                    player.DropAndDestroyHeldItem();
                    this.enabled = false; // Disable this box
                }
                else if (gm.knowsFuseIsMissing && !gm.hasTool)
                {
                    // Remind player
                    gm.ShowTemporaryMainCaption("It's locked. I need to find a tool.", captionDuration);
                    gm.knowsToolIsNeeded = true;
                }
                else
                {
                    // Player doesn't need to be here
                    gm.ShowTemporaryMainCaption("Just a fuse box.", captionDuration);
                }
                break;
        }
    }
}