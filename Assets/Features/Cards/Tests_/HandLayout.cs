using UnityEngine;

public class HandLayout : MonoBehaviour
{
    public Transform[] slots;

    public Vector3 GetSlotPosition(int index)
    {
        if (index < 0 || index >= slots.Length) return transform.position;
        return slots[index].position;
    }

    public Quaternion GetSlotRotation(int index)
    {
        if (index < 0 || index >= slots.Length) return Quaternion.identity;
        return slots[index].rotation;
    }
}
