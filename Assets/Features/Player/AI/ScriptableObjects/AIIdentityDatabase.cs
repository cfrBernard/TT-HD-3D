using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AIIdentityDatabase", menuName = "GameData/AIIdentityDatabase")]
public class AIIdentityDatabase : ScriptableObject
{
    [Header("AI names")]
    public List<string> aiNames = new();

    [Header("Avatar database")]
    public AvatarDatabase avatarDatabase;

    // Random IA identity.
    public (string name, string avatarId) GetRandomIdentity()
    {
        return (GetRandomName(), GetRandomAvatarId());
    }

    public string GetRandomName()
    {
        if (aiNames == null || aiNames.Count == 0)
            return "AI_Bot";
        return aiNames[Random.Range(0, aiNames.Count)];
    }

    public string GetRandomAvatarId()
    {
        if (avatarDatabase == null || avatarDatabase.avatars.Count == 0)
            return "01A001";
        return avatarDatabase.avatars[Random.Range(0, avatarDatabase.avatars.Count)].avatarId;
    }
}
