using UnityEngine;
using UnityEditor;
using System.IO;

public class CustomImporterTool : EditorWindow
{
    private string targetFolderPath;
    private string externalFolderPath;

    [MenuItem("Window/Custom Importer")]
    public static void ShowWindow()
    {
        GetWindow<CustomImporterTool>("Custom Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Importer Tool", EditorStyles.boldLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Select Target Folder"))
        {
            targetFolderPath = EditorUtility.OpenFolderPanel("Select Target Folder", "", "");
        }

        if (GUILayout.Button("Select External Folder"))
        {
            externalFolderPath = EditorUtility.OpenFolderPanel("Select External Folder", "", "");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Import Assets"))
        {
            ImportAssets();
        }
    }

    private void ImportAssets()
    {
        if (string.IsNullOrEmpty(targetFolderPath) || string.IsNullOrEmpty(externalFolderPath))
        {
            Debug.LogError("Please select both target and external folders.");
            return;
        }

        string[] externalFiles = Directory.GetFiles(externalFolderPath);
        foreach (string file in externalFiles)
        {
            string fileName = Path.GetFileName(file);
            string newFilePath = Path.Combine(targetFolderPath, fileName);

            if (File.Exists(newFilePath))
            {
                Debug.Log($"File {fileName} already exists in target folder. Skipping.");
                continue;
            }

            File.Copy(file, newFilePath);
            Debug.Log($"Copied {fileName} to {targetFolderPath}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Import process completed.");
    }
}