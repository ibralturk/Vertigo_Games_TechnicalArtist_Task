using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CustomAssetImporter : EditorWindow
{
    private string sourceFolderPath = ""; // Asset'lerin bulundu�u kaynak klas�r
    private string targetFolderPath = ""; // Asset'lerin import edilece�i hedef klas�r
    private string[] filesInSourceFolder;
    private Vector2 scrollPosition;
    private Dictionary<string, bool> fileSelectionStates = new Dictionary<string, bool>(); // Dosya se�im durumlar�

    [MenuItem("Window/Custom Asset Importer")]
    public static void ShowWindow()
    {
        GetWindow<CustomAssetImporter>("Custom Asset Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Asset Importer", EditorStyles.boldLabel);

        // Kaynak klas�r se�me butonu
        if (GUILayout.Button("Select Source Folder (Asset Location)"))
        {
            sourceFolderPath = EditorUtility.OpenFolderPanel("Select Source Folder", "", "");
            if (!string.IsNullOrEmpty(sourceFolderPath))
            {
                filesInSourceFolder = Directory.GetFiles(sourceFolderPath);
                fileSelectionStates.Clear(); // Se�im durumlar�n� temizle
                foreach (var file in filesInSourceFolder)
                {
                    if (!file.EndsWith(".meta")) // Meta dosyalar�n� atla
                    {
                        fileSelectionStates[file] = false; // Ba�lang��ta se�ili de�il
                    }
                }
            }
        }

        // Kaynak klas�r yolu g�sterimi
        if (!string.IsNullOrEmpty(sourceFolderPath))
        {
            GUILayout.Label("Source Folder: " + sourceFolderPath);
        }

        // Hedef klas�r se�me butonu
        if (GUILayout.Button("Select Target Folder (Save Location)"))
        {
            targetFolderPath = EditorUtility.OpenFolderPanel("Select Target Folder", "", "");
        }

        // Hedef klas�r yolu g�sterimi
        if (!string.IsNullOrEmpty(targetFolderPath))
        {
            GUILayout.Label("Target Folder: " + targetFolderPath);
        }

        // Kaynak klas�rdeki dosyalar� listeleme ve se�im yapma
        if (!string.IsNullOrEmpty(sourceFolderPath) && filesInSourceFolder != null && filesInSourceFolder.Length > 0)
        {
            GUILayout.Label("Files in Source Folder:");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            foreach (var file in filesInSourceFolder)
            {
                if (file.EndsWith(".meta")) continue; // Meta dosyalar�n� atla

                // Dosya se�im durumunu g�ster
                fileSelectionStates[file] = EditorGUILayout.ToggleLeft(file, fileSelectionStates[file]);
            }
            GUILayout.EndScrollView();
        }

        // Import i�lemi butonu
        if (!string.IsNullOrEmpty(sourceFolderPath) && !string.IsNullOrEmpty(targetFolderPath))
        {
            if (GUILayout.Button("Import Selected Assets"))
            {
                ImportSelectedAssets();
            }
        }
    }

    private void ImportSelectedAssets()
    {
        if (string.IsNullOrEmpty(sourceFolderPath) || string.IsNullOrEmpty(targetFolderPath))
        {
            Debug.LogError("Source or Target folder path is not set.");
            return;
        }

        // Hedef klas�r�n Unity projesi i�inde olup olmad���n� kontrol et
        if (!targetFolderPath.StartsWith(Application.dataPath))
        {
            Debug.LogError("Target folder must be inside the Unity project's Assets folder.");
            return;
        }

        // Se�ilen dosyalar� import et
        foreach (var file in fileSelectionStates)
        {
            if (file.Value) // Sadece se�ili dosyalar� i�le
            {
                string fileName = Path.GetFileName(file.Key);
                string targetFilePath = Path.Combine(targetFolderPath, fileName);

                // Dosyay� kopyala
                File.Copy(file.Key, targetFilePath, true);

                // Unity'de import et
                string relativePath = "Assets" + targetFilePath.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
                Debug.Log("Imported: " + relativePath);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Import process completed.");
    }
}