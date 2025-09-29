# Settings / CoreData System

Ce système gère tous les paramètres du jeu (audio, vidéo, gameplay, rules, etc.) en mode data-driven via JSON.

Il permet :

- D’avoir une **source par défaut** (valeurs usine).
- De sauvegarder uniquement les **overrides utilisateur**.
- De valider/structurer les paramètres grâce aux **metadata**.
- D’accéder aux settings via un **fallback automatique** : `UserSettings -> DefaultSettings.`

## Fichiers impliqués

### 1. `DefaultSettings.json`

- Stocké dans **Resources/**.
- Contient toutes les valeurs par défaut du jeu.
- Exemple :

```
{
  "video": {
    "resolution": "1920x1080",
    "fullscreen": true
  },
  "audio": {
    "masterVolume": 100,
    "musicVolume": 100
  }
}
```

### 2. `UserSettings.json`

- Stocké dans `Application.persistentDataPath`.
- Contient uniquement les paramètres modifiés par le joueur.
- Exemple (après changement utilisateur) :

```
{
  "video": {
    "resolution": "3840x2160",
    "fullscreen": true
  },
  "audio": {
    "masterVolume": 50,
    "musicVolume": 0
  }
}
```

### 3. `MetadataSettings.json`

- Stocké dans **Resources/**.
- Décrit la structure et contraintes des paramètres.
- Utilisé pour validation, génération UI, localisation, etc.
- Exemple :

```
{
  "audio": {
    "masterVolume": {
      "label": "Master Volume",
      "description": "Controls the overall game audio.",
      "type": "slider",
      "min": 0,
      "max": 100,
      "decimal": false
    }
  },
  "video": {
      "resolution": {
      "label": "Resolution",
      "description": "Sets the display resolution.",
      "type": "dropdown",
      "options": ["1920x1080", "3840x2160"]
    }
  },
  "rules": {
    "same": {
      "label": "Same",
      "description": "[...]",
      "type": "toggle"
    }
  }
}
```
---

## Fonctionnement interne

### Chargement (`SettingsManager.Awake()`)

1. Charge **DefaultSettings** depuis Resources.
2. Charge **MetadataSettings** depuis Resources.
3. Charge **UserSettings** depuis `persistentDataPath` (ou crée un fichier vide si absent).

### Lecture (`GetSetting<T>(path, field)`)

1. Cherche dans **UserSettings** -> si trouvé, retourne la valeur.
2. Sinon fallback sur **DefaultSettings**.
3. Si absent des deux -> warning et retourne `default(T)`.

```
bool fullscreen = SettingsManager.Instance.GetSetting<bool>("video", "fullscreen");
```

### Écriture (`SetOverride<T>(path, field, value)`)

- Écrit/override la valeur dans **UserSettings**.
- Exemple :

```
SettingsManager.Instance.SetOverride("rules", "coinToss", true);
SettingsManager.Instance.Save();
```

### Sauvegarde (`Save()`)

- `UserSettings.json` est mis à jour sur disque.

### Reset

- `ResetAllSettings()` supprime `UserSettings.json` -> retour aux defaults.

---

## Avantages

- **Clair** : séparation nette entre defaults, overrides et metadata.
- **Flexible** : facile d’ajouter des sections (juste un node JSON).
- **Lisible** : `UserSettings.json` reste minimal, parfait pour debug.
- **Évolutif** : ajouter une nouvelle option = éditer DefaultSettings.json + MetadataSettings.json.

## Conventions

- Les **sections** sont des objets racine (`audio`, `video`, `rules`, etc.).
- Les **noms de paramètres** sont en camelCase (`masterVolume`, `coinToss`).
- Chaque nouveau paramètre doit être ajouté dans :
    - `DefaultSettings.json` (valeur par défaut).
    - `MetadataSettings.json` (type, contraintes etc.).

## Exemple d’utilisation

### Récupérer un setting

```
float volume = SettingsManager.Instance.GetSetting<float>("audio", "masterVolume");
```

### Modifier et sauvegarder

```
SettingsManager.Instance.SetOverride("rules", "coinToss", true);
SettingsManager.Instance.Save();
```

### Reset complet

```
SettingsManager.Instance.ResetAllSettings();
```

---