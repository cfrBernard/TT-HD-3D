using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AvatarDatabase", menuName = "GameData/AvatarDatabase")]
public class AvatarDatabase : ScriptableObject
{
    public List<AvatarData> avatars;

    public AvatarData GetAvatarById(string id)
    {
        return avatars.Find(a => a.avatarId == id);
    }
}
