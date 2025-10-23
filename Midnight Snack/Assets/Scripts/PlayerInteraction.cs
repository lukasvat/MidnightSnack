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
        }
    }

    private void CheckForInteractables()
    {
        // Clear "look" targets
        lookItem = null;
        lookDoor = null;

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
        if (closestCollider != null)
        {
            // Only interact with items if not holding one
            if (heldItem == null)
            {
                // Check if it's a pickupable item
                Pickupable item = closestCollider.GetComponent<Pickupable>();
                if (item != null)
                {
                    lookItem = item;
                    captionPromptText.text = "Press [E] to pick up " + item.itemName;
                    captionUI.SetActive(true);
                    return; // Found item
                }
            }

            // We can always interact with doors
            InteractableDoor door = closestCollider.GetComponent<InteractableDoor>();
            if (door != null)
            {
                lookDoor = door;
                captionPromptText.text = "Press [E] to " + (door.IsOpen() ? "close" : "open") + " Door";
                captionUI.SetActive(true);
                return; // Found door
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

    // Helper function to clear UI/targets
    private void ClearLookTargets()
    {
        lookItem = null;
        lookDoor = null;

        if (captionUI != null)
        {
            captionUI.SetActive(false);
        }
    }
}
