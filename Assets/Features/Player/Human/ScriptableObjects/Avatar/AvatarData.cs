using UnityEngine;

[CreateAssetMenu(fileName = "AvatarData", menuName = "GameData/Avatar")]
public class AvatarData : ScriptableObject
{
    public string avatarId;
    public string avatarName;
    public Sprite avatarSprite;
}
