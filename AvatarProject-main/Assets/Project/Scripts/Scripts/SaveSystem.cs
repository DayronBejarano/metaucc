using UnityEngine;
using System;
using System.IO;
using System.Text;

public static class SaveSystem
{
    public static string SanitizeProfileId(string profileID)
    {
        if (string.IsNullOrWhiteSpace(profileID))
        {
            return "default_profile";
        }

        string trimmed = profileID.Trim();
        char[] invalidChars = Path.GetInvalidFileNameChars();
        StringBuilder sb = new StringBuilder(trimmed.Length);

        foreach (char c in trimmed)
        {
            if (Array.IndexOf(invalidChars, c) < 0 && c != '/' && c != '\\' && c != '.')
            {
                sb.Append(c);
            }
            else if (c == '.')
            {
                sb.Append('_');
            }
        }

        string clean = sb.ToString().Trim();
        return string.IsNullOrEmpty(clean) ? "default_profile" : clean;
    }

    public static string GetSavePath(string profileID)
    {
        string safeId = SanitizeProfileId(profileID);
        string baseDir = Path.GetFullPath(Application.persistentDataPath);
        string fullPath = Path.GetFullPath(Path.Combine(baseDir, safeId + ".json"));

        if (!fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
        {
            return Path.Combine(baseDir, "default_profile.json");
        }

        return fullPath;
    }

    public static bool ProfileExists(string profileID)
    {
        return File.Exists(GetSavePath(profileID));
    }

    public static void SaveAvatar(AvatarData data, string profileID)
    {
        if (data == null)
        {
            Debug.LogError("SaveSystem: No se puede guardar un AvatarData nulo.");
            return;
        }

        try
        {
            string path = GetSavePath(profileID);
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json, Encoding.UTF8);
            Debug.Log("Avatar guardado en: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveSystem: Error al guardar el perfil '{profileID}': {ex.Message}");
        }
    }

    public static AvatarData LoadAvatar(string profileID)
    {
        string path = GetSavePath(profileID);
        if (!File.Exists(path))
        {
            return new AvatarData();
        }

        try
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new AvatarData();
            }

            AvatarData loaded = JsonUtility.FromJson<AvatarData>(json);
            return loaded ?? new AvatarData();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"SaveSystem: Archivo corrupto o ilegible en '{path}', usando valores por defecto. Detalle: {ex.Message}");
            return new AvatarData();
        }
    }
}
