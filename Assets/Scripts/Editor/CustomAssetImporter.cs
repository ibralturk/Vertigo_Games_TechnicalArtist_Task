using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class CustomAssetImporter : EditorWindow
{
    private string sourceFolderPath = ""; // Asset'lerin bulundu�u kaynak klas�r
    private string targetFolderPath = ""; // Asset'lerin import edilece�i hedef klas�r
    private string[] filesInSourceFolder;
    private Vector2 scrollPosition;
    private Dictionary<string, bool> fileSelectionStates = new Dictionary<string, bool>(); // Dosya se�im durumlar�
    private string searchText = ""; // Aranacak kelime
    private string replaceText = ""; // De�i�tirilecek kelime

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

        // Name Replace Alan�
        GUILayout.Label("Name Replace", EditorStyles.boldLabel);
        searchText = EditorGUILayout.TextField("Search Text", searchText);
        replaceText = EditorGUILayout.TextField("Replace Text", replaceText);

        // T�m�n� Se� ve T�m�n� Kald�r Butonlar�
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All"))
        {
            SelectAllFiles(true); // T�m dosyalar� se�
        }
        if (GUILayout.Button("Deselect All"))
        {
            SelectAllFiles(false); // T�m dosyalar�n se�imini kald�r
        }
        GUILayout.EndHorizontal();

        // Kaynak klas�rdeki dosyalar� listeleme ve se�im yapma
        if (!string.IsNullOrEmpty(sourceFolderPath) && filesInSourceFolder != null && filesInSourceFolder.Length > 0)
        {
            GUILayout.Label("Files in Source Folder:");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            foreach (var file in filesInSourceFolder)
            {
                if (file.EndsWith(".meta")) continue; // Meta dosyalar�n� atla

                // Dosya ismini de�i�tirilmi� haliyle g�ster
                string fileName = Path.GetFileName(file);
                string newFileName = fileName.Replace(searchText, replaceText);
                GUILayout.BeginHorizontal();
                fileSelectionStates[file] = EditorGUILayout.ToggleLeft($"{fileName} -> {newFileName}", fileSelectionStates[file]);
                GUILayout.EndHorizontal();
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

    private void SelectAllFiles(bool select)
    {
        // fileSelectionStates s�zl���ndeki t�m dosyalar�n se�im durumunu g�ncelle
        var keys = fileSelectionStates.Keys.ToList(); // S�zl�k anahtarlar�n� bir listeye al
        foreach (var key in keys)
        {
            fileSelectionStates[key] = select; // T�m dosyalar� se� veya se�imi kald�r
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
                string newFileName = fileName.Replace(searchText, replaceText); // Dosya ismini de�i�tir
                string targetFilePath = Path.Combine(targetFolderPath, newFileName);

                // Dosyay� kopyala
                File.Copy(file.Key, targetFilePath, true);

                // Unity'de import et
                string relativePath = "Assets" + targetFilePath.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Imported: {file.Key} -> {relativePath}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Import process completed.");
    }
}