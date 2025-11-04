using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoardSlotView : MonoBehaviour
{
    public int x;
    public int y;

    private BoardSlot slot;

    public void Bind(BoardSlot slot)
    {
        this.slot = slot;
    }

    public BoardSlot Slot => slot;

    public bool IsEmpty => slot != null && slot.IsEmpty;


    // feedback visuel
    public void Highlight(bool active)
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = active ? Color.yellow : Color.white;
        }
    }
}
