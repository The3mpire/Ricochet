using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    #region Editor Operations
    [CustomEditor(typeof(Neon))]
    public class TMapInspector : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update"))
            {
                Neon neon = (Neon)target;
                neon.UpdateLineRenderers();
            }
            if (GUILayout.Button("Flash"))
            {
                Neon neon = (Neon)target;
                neon.Flash();
            }
        }
    }
    #endregion
}
