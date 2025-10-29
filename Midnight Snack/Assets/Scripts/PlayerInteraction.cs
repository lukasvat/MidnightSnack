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

    void Update()
    {
        CheckForInteractables();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lookItem != null)
            {
                // We are looking at an existing item
                HoldExistingItem(lookItem);
            }
            else if (lookDoor != null)
            {
                // We are looking at a door
                lookDoor.Interact();
            }
            else if (lookMicrowave != null)
            {
                // Place the item we're holding into the microwave
                GameObject itemToPlace = PlaceHeldItem();
                lookMicrowave.PlaceBurger(itemToPlace);
                ClearLookTargets();
            }
        }
    }

    private void CheckForInteractables()
    {
        // Clear "look" targets
        lookItem = null;
        lookDoor = null;
        lookMicrowave = null;

        // Find the closest collider in our interaction cone
        Collider closestCollider = null;
        float minDistance = float.MaxValue;

        Collider[] hitColliders = Physics.OverlapSphere(mainCamera.position, interactionDistance);

        foreach (var hitCollider in hitColliders)
        {
            // Check if item is in our view cone
            Vector3 directionToItem = (hitCollider.transform.position - mainCamera.position).normalized;

            if (directionToItem == Vector3.zero) continue;

            float angle = Vector3.Angle(mainCamera.forward, directionToItem);

            if (angle <= interactionAngle)
            {
                // Check if it's the closest one
                float distance = Vector3.Distance(mainCamera.position, hitCollider.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCollider = hitCollider;
                }
            }
        }
        
        // We found the closest object, check what it is
        if (closestCollider == null)
        {
            if (captionUI != null) captionUI.SetActive(false);
            return;
        }

        // --- Interaction Logic ---
        // Check for interactables based on whether we are holding an item

        if (heldItem != null)
        {
            // We are holding something. Look for Microwaves or Doors.
            InteractableMicrowave microwave = closestCollider.GetComponent<InteractableMicrowave>();
            if (microwave != null)
            {
                lookMicrowave = microwave;
                captionPromptText.text = "Press [E] to use Microwave";
                captionUI.SetActive(true);
                return;
            }

            InteractableDoor door = closestCollider.GetComponent<InteractableDoor>();
            if (door != null)
            {
                lookDoor = door;
                captionPromptText.text = "Press [E] to " + (door.IsOpen() ? "close" : "open") + " Door";
                captionUI.SetActive(true);
                return;
            }
        }
        else
        {
            // We are NOT holding anything. Look for Pickups or Doors.
            Pickupable item = closestCollider.GetComponent<Pickupable>();
            if (item != null)
            {
                lookItem = item;
                captionPromptText.text = "Press [E] to pick up " + item.itemName;
                captionUI.SetActive(true);
                return;
            }

            InteractableDoor door = closestCollider.GetComponent<InteractableDoor>();
            if (door != null)
            {
                lookDoor = door;
                captionPromptText.text = "Press [E] to " + (door.IsOpen() ? "close" : "open") + " Door";
                captionUI.SetActive(true);
                return;
            }
        }

        // If we hit nothing interactable
        if (captionUI != null)
        {
            captionUI.SetActive(false);
        }
    }

    // Handles picking up items already in the world
    private void HoldExistingItem(Pickupable item)
    {
        heldItem = item.gameObject;

        // Parent item to hand and set its position/rotation
        heldItem.transform.SetParent(handSocket);
        heldItem.transform.localPosition = item.positionOffset;
        heldItem.transform.localRotation = Quaternion.Euler(item.rotationOffset);

        // Disable physics/collider
        Collider col = heldItem.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        ClearLookTargets();
    }

    // "drop" the item from our hand
    private GameObject PlaceHeldItem()
    {
        GameObject itemToPlace = heldItem;
        heldItem = null;

        // Re-enable collider so it can sit in the microwave
        Collider col = itemToPlace.GetComponent<Collider>();
        if (col != null) col.enabled = true;
        
        Rigidbody rb = itemToPlace.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        return itemToPlace;
    }

    // Helper function to clear UI/targets
    private void ClearLookTargets()
    {
        lookItem = null;
        lookDoor = null;
        lookMicrowave = null;

        if (captionUI != null)
        {
            captionUI.SetActive(false);
        }
    }
}
