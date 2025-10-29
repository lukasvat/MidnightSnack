using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public enum ItemType
    {
        Generic, // burger
        Tool // used to break the lock
    }

    public ItemType itemType = ItemType.Generic;
    public string itemName = "burger";
    public Vector3 positionOffset = new Vector3(0, 0, 0);
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
}