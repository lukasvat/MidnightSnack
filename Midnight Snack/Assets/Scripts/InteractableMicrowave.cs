using UnityEngine;

public class InteractableMicrowave : MonoBehaviour
{
    [Header("Microwave Setup")]
    public Transform burgerSocket; 
    public GameObject microwaveLight;
    public float rotationSpeed = 30f;
    private bool isRunning = false;
    private GameObject burgerInstance = null;

    void Start()
    {
        // Start with the light off
        if (microwaveLight != null) microwaveLight.SetActive(false);
    }

    void Update()
    {
        // If the microwave is on, rotate the burger
        if (isRunning && burgerInstance != null)
        {
            burgerSocket.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    // called by PlayerInteraction.cs
    public void PlaceBurger(GameObject burger)
    {
        burgerInstance = burger;
        isRunning = true;

        burger.transform.SetParent(burgerSocket);
        burger.transform.localPosition = Vector3.zero;
        burger.transform.localRotation = Quaternion.identity;

        if (microwaveLight != null) microwaveLight.SetActive(true);

        GameManager.Instance.OnMicrowaveUsed();
    }
}