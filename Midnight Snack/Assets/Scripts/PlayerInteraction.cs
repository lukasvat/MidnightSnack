using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public Transform mainCamera;
    public Transform handSocket;
    public GameObject captionUI;
    public TextMeshProUGUI captionPromptText;
    public float interactionDistance = 2.5f;
    public float interactionAngle = 45f;
    private GameObject heldItem = null;
    private Pickupable lookItem = null;
    private InteractableDoor lookDoor = null;
    private InteractableMicrowave lookMicrowave = null;
    private InteractableFuseBox lookFuseBox = null;
    private float promptLockoutTimer = 0f;
    private float promptLockoutDuration = 2f;


    void Update()
    {
        if (promptLockoutTimer > 0f)
            {
                promptLockoutTimer -= Time.deltaTime;
                return;
            }
        CheckForInteractables();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lookItem != null)
            {
                HoldExistingItem(lookItem);
            }
            else if (lookDoor != null)
            {
                lookDoor.Interact();
            }
            else if (lookMicrowave != null)
            {
                GameObject itemToPlace = PlaceHeldItem();
                lookMicrowave.PlaceBurger(itemToPlace);
                ClearLookTargets();
            }
            else if (lookFuseBox != null)
            {
                lookFuseBox.Interact(this);
                promptLockoutTimer = promptLockoutDuration;
            }
        }
    }

private void CheckForInteractables()
    {
        ClearLookTargets();

        Collider closestCollider = null;
        float minDistance = float.MaxValue;

        Collider[] hitColliders = Physics.OverlapSphere(mainCamera.position, interactionDistance);

        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToItem = (hitCollider.transform.position - mainCamera.position).normalized;
            if (directionToItem == Vector3.zero) continue;
            float angle = Vector3.Angle(mainCamera.forward, directionToItem);

            if (angle <= interactionAngle)
            {
                float distance = Vector3.Distance(mainCamera.position, hitCollider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCollider = hitCollider;
                }
            }
        }
        
        if (closestCollider == null)
        {
            if (captionUI != null) captionUI.SetActive(false);
            return;
        }

        // Interaction Logic
        if (heldItem != null)
        {
            // We are holding something. Check for Microwave.
            InteractableMicrowave microwave = closestCollider.GetComponent<InteractableMicrowave>();
            if (microwave != null)
            {
                Pickupable heldItemComponent = heldItem.GetComponent<Pickupable>();
                if (heldItemComponent != null && heldItemComponent.itemType == Pickupable.ItemType.Generic)
                {
                    lookMicrowave = microwave;
                    ShowPrompt("Press [E] to use Microwave");
                }
                else
                {
                    ShowPrompt("I can't put this in the microwave.");
                }
                return;
            }
        }
        else
        {
            // We are not holding anything. Check for Pickupables.
            Pickupable item = closestCollider.GetComponent<Pickupable>();
            if (item != null)
            {
                if (item.itemType == Pickupable.ItemType.Tool)
                {
                    if (GameManager.Instance.knowsToolIsNeeded)
                    {
                        lookItem = item;
                        ShowPrompt("Press [E] to pick up " + item.itemName);
                    }
                    else
                    {
                        ShowPrompt("I don't need this right now.");
                    }
                }
                else
                {
                    lookItem = item;
                    ShowPrompt("Press [E] to pick up " + item.itemName);
                }
                return; // Found an item, stop here.
            }
        }

        // Check for Door
        InteractableDoor door = closestCollider.GetComponent<InteractableDoor>();
        if (door != null)
        {
            lookDoor = door;
            ShowPrompt("Press [E] to " + (door.IsOpen() ? "close" : "open") + " Door");
            return;
        }

        // Check for FuseBox
        InteractableFuseBox fuseBox = closestCollider.GetComponent<InteractableFuseBox>();
        if (fuseBox != null)
        {
            lookFuseBox = fuseBox;
            ShowPrompt(fuseBox.GetPrompt());
            return;
        }

        // If we hit nothing interactable
        if (captionUI != null)
        {
            captionUI.SetActive(false);
        }
    }

private void HoldExistingItem(Pickupable item)
    {
        heldItem = item.gameObject;

        heldItem.transform.SetParent(handSocket);

        // Set its local position and rotation relative to the hand
        heldItem.transform.localPosition = item.positionOffset;
        heldItem.transform.localRotation = Quaternion.Euler(item.rotationOffset);

        // Reset the scale
        heldItem.transform.localScale = Vector3.one;

        Collider col = heldItem.GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Tell GameManager if this is the tool
        if (item.itemType == Pickupable.ItemType.Tool)
        {
            GameManager.Instance.OnPlayerGotTool();
        }

        ClearLookTargets();
    }

    private GameObject PlaceHeldItem()
    {
        GameObject itemToPlace = heldItem;
        heldItem = null;

        Collider col = itemToPlace.GetComponent<Collider>();
        if (col != null) col.enabled = true;
        
        Rigidbody rb = itemToPlace.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        return itemToPlace;
    }

    public void DropAndDestroyHeldItem()
    {
        if (heldItem != null)
        {
            GameObject itemToDestroy = heldItem;
            heldItem = null;
            Destroy(itemToDestroy);
        }
    }

    public void ShowPrompt(string message)
    {
        if (captionUI != null)
        {
            captionPromptText.text = message;
            captionUI.SetActive(true);
        }
    }

    private void ClearLookTargets()
    {
        lookItem = null;
        lookDoor = null;
        lookMicrowave = null;
        lookFuseBox = null;

        if (captionUI != null)
        {
            captionUI.SetActive(false);
        }
    }
}