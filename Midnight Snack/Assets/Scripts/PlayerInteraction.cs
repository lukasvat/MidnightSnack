using UnityEngine;
using TMPro;

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

    void Update()
    {
        CheckForInteractables();

        if (lookItem != null && Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem(lookItem);
        }
    }

    private void CheckForInteractables()
    {
        if (heldItem != null)
        {
            lookItem = null;
            if (captionUI != null)
            {
                captionUI.SetActive(false);
            }
            return;
        }

        // Find the closest item within our interaction cone
        Pickupable closestItem = null;
        float minDistance = float.MaxValue;

        Collider[] hitColliders = Physics.OverlapSphere(mainCamera.position, interactionDistance);

        foreach (var hitCollider in hitColliders)
        {
            Pickupable item = hitCollider.GetComponent<Pickupable>();

            if (item != null)
            {
                // Check if item is in our view cone
                Vector3 directionToItem = (item.transform.position - mainCamera.position).normalized;

                if (directionToItem == Vector3.zero) continue;

                float angle = Vector3.Angle(mainCamera.forward, directionToItem);

                if (angle <= interactionAngle)
                {
                    // Check if it's the closest one so far
                    float distance = Vector3.Distance(mainCamera.position, item.transform.position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestItem = item;
                    }
                }
            }
        }

        // Update UI based on the closest item found
        lookItem = closestItem;

        if (lookItem != null)
        {
            // Found an item: update text and show UI
            captionPromptText.text = "Press [E] to pick up " + lookItem.itemName;
            captionUI.SetActive(true);
        }
        else
        {
            // No item found: hide UI
            if (captionUI != null)
            {
                captionUI.SetActive(false);
            }
        }
    }

    private void PickUpItem(Pickupable item)
    {
        heldItem = item.gameObject;

        // Parent item to hand and set its position/rotation
        heldItem.transform.SetParent(handSocket);
        heldItem.transform.localPosition = item.positionOffset;
        heldItem.transform.localRotation = Quaternion.Euler(item.rotationOffset);

        // Disable physics and collider
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

        // Hide the UI
        lookItem = null;
        if (captionUI != null)
        {
            captionUI.SetActive(false);
        }
    }
}