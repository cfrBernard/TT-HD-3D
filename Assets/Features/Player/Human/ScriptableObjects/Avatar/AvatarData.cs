using UnityEngine;

[CreateAssetMenu(fileName = "AvatarData", menuName = "GameData/AvatarData")]
public class AvatarData : ScriptableObject
{
    public string avatarId;
    public Sprite avatarSprite;
}
