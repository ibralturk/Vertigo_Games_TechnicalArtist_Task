using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CustomAssetImporter : EditorWindow
{
    private string sourceFolderPath = ""; // Asset'lerin bulunduðu kaynak klasör
    private string targetFolderPath = ""; // Asset'lerin import edileceði hedef klasör
    private string[] filesInSourceFolder;
    private Vector2 scrollPosition;
    private Dictionary<string, bool> fileSelectionStates = new Dictionary<string, bool>(); // Dosya seçim durumlarý

    [MenuItem("Window/Custom Asset Importer")]
    public static void ShowWindow()
    {
        GetWindow<CustomAssetImporter>("Custom Asset Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Asset Importer", EditorStyles.boldLabel);

        // Kaynak klasör seçme butonu
        if (GUILayout.Button("Select Source Folder (Asset Location)"))
        {
            sourceFolderPath = EditorUtility.OpenFolderPanel("Select Source Folder", "", "");
            if (!string.IsNullOrEmpty(sourceFolderPath))
            {
                filesInSourceFolder = Directory.GetFiles(sourceFolderPath);
                fileSelectionStates.Clear(); // Seçim durumlarýný temizle
                foreach (var file in filesInSourceFolder)
                {
                    if (!file.EndsWith(".meta")) // Meta dosyalarýný atla
                    {
                        fileSelectionStates[file] = false; // Baþlangýçta seçili deðil
                    }
                }
            }
        }

        // Kaynak klasör yolu gösterimi
        if (!string.IsNullOrEmpty(sourceFolderPath))
        {
            GUILayout.Label("Source Folder: " + sourceFolderPath);
        }

        // Hedef klasör seçme butonu
        if (GUILayout.Button("Select Target Folder (Save Location)"))
        {
            targetFolderPath = EditorUtility.OpenFolderPanel("Select Target Folder", "", "");
        }

        // Hedef klasör yolu gösterimi
        if (!string.IsNullOrEmpty(targetFolderPath))
        {
            GUILayout.Label("Target Folder: " + targetFolderPath);
        }

        // Kaynak klasördeki dosyalarý listeleme ve seçim yapma
        if (!string.IsNullOrEmpty(sourceFolderPath) && filesInSourceFolder != null && filesInSourceFolder.Length > 0)
        {
            GUILayout.Label("Files in Source Folder:");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            foreach (var file in filesInSourceFolder)
            {
                if (file.EndsWith(".meta")) continue; // Meta dosyalarýný atla

                // Dosya seçim durumunu göster
                fileSelectionStates[file] = EditorGUILayout.ToggleLeft(file, fileSelectionStates[file]);
            }
            GUILayout.EndScrollView();
        }

        // Import iþlemi butonu
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

        // Hedef klasörün Unity projesi içinde olup olmadýðýný kontrol et
        if (!targetFolderPath.StartsWith(Application.dataPath))
        {
            Debug.LogError("Target folder must be inside the Unity project's Assets folder.");
            return;
        }

        // Seçilen dosyalarý import et
        foreach (var file in fileSelectionStates)
        {
            if (file.Value) // Sadece seçili dosyalarý iþle
            {
                string fileName = Path.GetFileName(file.Key);
                string targetFilePath = Path.Combine(targetFolderPath, fileName);

                // Dosyayý kopyala
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