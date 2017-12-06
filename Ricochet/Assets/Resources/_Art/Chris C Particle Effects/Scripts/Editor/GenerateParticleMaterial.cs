using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace CCParticles
{
    public class GenerateParticleMaterial
    {
        #region Hidden Variables

        private static string suffix = " MAT";

        #endregion

        #region External Functions

        [MenuItem("Assets/Create/Particles/Additive Material", priority = 0)]
        public static void CreateAdditiveMaterial()
        {
            CreateMaterial("Particles/Additive");
        }

        #endregion

        #region Helper Methods

        private static void CreateMaterial(string shaderName)
        {
            try
            {
                UnityEngine.Object selectedObject = Selection.activeObject;
                if (selectedObject is Texture)
                {
                    string path = AssetDatabase.GetAssetPath(selectedObject);
                    string filename = Path.GetFileName(path);
                    string targetpath = path.Substring(0, path.LastIndexOf(".")) + suffix + ".mat";
                    if (File.Exists(targetpath))
                    {
                        Debug.LogError("File already exists, cannot create material");
                        return;
                    }

                    Material mat = new Material(Shader.Find(shaderName));

                    mat.mainTexture = selectedObject as Texture;

                    AssetDatabase.CreateAsset(mat, targetpath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong, could not create material...\r\n" + e.Message);
            }
        }

        #endregion

    }
}
