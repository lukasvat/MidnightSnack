using UnityEngine;
using System.Collections;

public class InteractableDoor : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Store the original rotation
        closedRotation = transform.rotation;

        // add 90 degrees to the Y-axis
        Vector3 openEulerAngles = closedRotation.eulerAngles + new Vector3(0, openAngle, 0);
        openRotation = Quaternion.Euler(openEulerAngles);
    }

    public void Interact()
    {
        // Don't interact if the door is already moving
        if (isMoving)
        {
            return;
        }

        // Start the animation
        StartCoroutine(RotateDoor());
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    private IEnumerator RotateDoor()
    {
        isMoving = true;

        // Determine target rotation
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;
        Quaternion startRotation = transform.rotation;
        float time = 0;

        while (time < 1)
        {
            // Smoothly interpolate between current and target rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
            time += Time.deltaTime * openSpeed;
            yield return null; // Wait for the next frame
        }

        // Snap to final rotation to ensure accuracy
        transform.rotation = targetRotation;

        // Update state
        isOpen = !isOpen;
        isMoving = false;
    }
}

