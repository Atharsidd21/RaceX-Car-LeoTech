// using UnityEngine;
// using UnityEditor;

// public class URP_MaterialConverter : EditorWindow
// {
//     [MenuItem("Tools/Convert All Materials to URP")]
//     public static void ConvertAllMaterialsToURP()
//     {
//         string[] materialGuids = AssetDatabase.FindAssets("t:Material");
//         int convertedCount = 0;

//         foreach (string guid in materialGuids)
//         {
//             string path = AssetDatabase.GUIDToAssetPath(guid);
//             Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

//             if (mat == null) continue;

//             Shader oldShader = mat.shader;

//             // Skip already URP materials or UI/Text shaders
//             if (oldShader.name.StartsWith("Universal Render Pipeline") ||
//                 oldShader.name.Contains("TextMeshPro") ||
//                 oldShader.name.Contains("UI") ||
//                 oldShader.name.Contains("Sprite"))
//                 continue;

//             // Convert to URP/Lit
//             Color color = mat.HasProperty("_Color") ? mat.color : Color.white;
//             Texture mainTex = mat.HasProperty("_MainTex") ? mat.mainTexture : null;

//             mat.shader = Shader.Find("Universal Render Pipeline/Lit");

//             if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
//             if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", mainTex);

//             EditorUtility.SetDirty(mat);
//             convertedCount++;
//         }

//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();

//         EditorUtility.DisplayDialog("URP Material Converter", $"Converted {convertedCount} materials to URP.", "OK");
//     }
// }
