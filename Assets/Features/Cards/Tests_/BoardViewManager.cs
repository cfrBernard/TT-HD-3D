using UnityEngine;

public class BoardViewManager : MonoBehaviour
{
    public BoardManager boardManager;
    public BoardSlotView[] slotViews;

    private void Awake()
    {
        if (boardManager == null)
            boardManager = GetComponent<BoardManager>();

        // Lier chaque view à son slot logique
        foreach (var view in slotViews)
        {
            BoardSlot slot = boardManager.GetSlot(view.x, view.y);
            view.Bind(slot);
        }
    }

    public BoardSlotView GetView(int x, int y)
    {
        foreach (var view in slotViews)
        {
            if (view.x == x && view.y == y)
                return view;
        }
        return null;
    }

    public BoardSlotView GetView(BoardSlot slot)
    {
        foreach (var view in slotViews)
        {
            if (view.Slot == slot)
                return view;
        }
        return null;
    }
}
