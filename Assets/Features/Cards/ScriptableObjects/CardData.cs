using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/CardData", order = 0)]
public class CardData : ScriptableObject
{
    public string cardId;
    public string cardName;
    public int level;

    public Texture2D frontAlbedo;

    [Header("Stats")]
    public int north;
    public int south;
    public int west;
    public int east;
}
