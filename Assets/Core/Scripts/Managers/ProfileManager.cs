using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }
    private JObject UserProfile;

    private const string DefaultProfilePath = "DefaultProfile";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadOrInitProfile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadOrInitProfile()
    {

    }
}
