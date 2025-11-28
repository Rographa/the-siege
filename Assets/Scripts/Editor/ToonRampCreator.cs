using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ToonRampCreator : EditorWindow
    {
        private Gradient _gradient = new Gradient();
        private Texture2D _rampTexture;
        private int _textureWidth = 128;
    
        [MenuItem("Tools/Create Toon Ramp")]
        public static void ShowWindow()
        {
            GetWindow<ToonRampCreator>("Toon Ramp Creator");
        }
    
        private void OnGUI()
        {
            GUILayout.Label("Toon Ramp Texture Creator", EditorStyles.boldLabel);
        
            _textureWidth = EditorGUILayout.IntField("Texture Width", _textureWidth);
        
            EditorGUILayout.GradientField("Ramp Gradient", _gradient);
        
            if (GUILayout.Button("Generate Ramp Texture"))
            {
                GenerateRampTexture();
            }
        
            if (_rampTexture != null)
            {
                GUILayout.Label("Preview:");
                var previewRect = GUILayoutUtility.GetAspectRect(4f);
                EditorGUI.DrawPreviewTexture(previewRect, _rampTexture);
            
                if (GUILayout.Button("Save Texture"))
                {
                    SaveTexture();
                }
            }
        }
    
        private void GenerateRampTexture()
        {
            _rampTexture = new Texture2D(_textureWidth, 1, TextureFormat.RGBA32, false);
            _rampTexture.wrapMode = TextureWrapMode.Clamp;
            _rampTexture.filterMode = FilterMode.Bilinear;
        
            for (var x = 0; x < _textureWidth; x++)
            {
                var t = x / (float)(_textureWidth - 1);
                var color = _gradient.Evaluate(t);
                _rampTexture.SetPixel(x, 0, color);
            }
        
            _rampTexture.Apply();
        }
    
        private void SaveTexture()
        {
            var path = EditorUtility.SaveFilePanel("Save Ramp Texture", "Assets", "ToonRamp", "png");
            if (!string.IsNullOrEmpty(path))
            {
                var bytes = _rampTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(path, bytes);
                AssetDatabase.Refresh();
            }
        }
    }
}