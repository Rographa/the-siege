using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class TextureGenerator
    {
        [MenuItem("Tools/Generate Basic Textures")]
        public static void GenerateBasicTextures()
        {
            GenerateGridTexture();
            GenerateBrickTexture();
            GenerateStripeTexture();
            GenerateNoiseTexture();
        }

        private static void GenerateGridTexture()
        {
            var tex = new Texture2D(64, 64);
        
            for (var x = 0; x < 64; x++)
            {
                for (var y = 0; y < 64; y++)
                {
                    if (x % 16 == 0 || y % 16 == 0)
                        tex.SetPixel(x, y, new Color(0.8f, 0.8f, 0.8f, 1f));
                    else
                        tex.SetPixel(x, y, Color.white);
                }
            }
        
            SaveTexture(tex, "GridTexture");
        }

        private static void GenerateBrickTexture()
        {
            var tex = new Texture2D(64, 64);
        
            for (var x = 0; x < 64; x++)
            {
                for (var y = 0; y < 64; y++)
                {
                    var isBrick = (x / 16 + y / 8) % 2 == 0;
                    tex.SetPixel(x, y, isBrick ? new Color(0.9f, 0.9f, 0.9f, 1f) : Color.white);
                }
            }
        
            SaveTexture(tex, "BrickTexture");
        }

        private static void GenerateStripeTexture()
        {
            var tex = new Texture2D(64, 64);
        
            for (var x = 0; x < 64; x++)
            {
                for (var y = 0; y < 64; y++)
                {
                    var isStripe = (x / 8) % 2 == 0;
                    tex.SetPixel(x, y, isStripe ? new Color(0.9f, 0.9f, 0.9f, 1f) : Color.white);
                }
            }
        
            SaveTexture(tex, "StripeTexture");
        }

        private static void GenerateNoiseTexture()
        {
            var tex = new Texture2D(64, 64);
        
            for (var x = 0; x < 64; x++)
            {
                for (var y = 0; y < 64; y++)
                {
                    var noise = Random.Range(0.95f, 1.0f);
                    tex.SetPixel(x, y, new Color(noise, noise, noise, 1f));
                }
            }
        
            SaveTexture(tex, "NoiseTexture");
        }

        private static void SaveTexture(Texture2D texture, string name)
        {
            var bytes = texture.EncodeToPNG();
            var path = "Assets/" + name + ".png";
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            Debug.Log($"Saved: {path}");
        }
    }
}
