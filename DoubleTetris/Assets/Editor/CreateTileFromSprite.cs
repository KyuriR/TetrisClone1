using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;

// EDITOR-ONLY UTILITY. This must live inside a folder named "Editor"
// anywhere under Assets (e.g. Assets/Editor/CreateTileFromSprite.cs) —
// Unity excludes anything in an "Editor" folder from game builds automatically.
public class CreateTileFromSprite
{
    [MenuItem("Assets/Create Tile From Selected Sprite(s)")]
    private static void CreateTiles()
    {
        // Works on however many sprites you have selected in the Project window at once.
        foreach (Object obj in Selection.objects)
        {
            Sprite sprite = obj as Sprite;

            // Handle the case where you selected the source texture rather
            // than the sprite sub-asset (common when Sprite Mode = Single).
            if (sprite == null)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            if (sprite == null)
            {
                Debug.LogWarning($"Skipped '{obj.name}' — not a sprite or texture with a sprite.");
                continue;
            }

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = Color.white; // change per-tile afterward in its own Inspector if you want tint variants

            string sourcePath = AssetDatabase.GetAssetPath(sprite);
            string folder = Path.GetDirectoryName(sourcePath);
            string savePath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(folder, sprite.name + "_Tile.asset"));

            AssetDatabase.CreateAsset(tile, savePath);
            Debug.Log($"Created Tile asset: {savePath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
