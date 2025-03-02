using System;
using System.IO;
using RWCustom;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

public static class Helpers
{
    public static void LoadFromImage(string path, string name)
    {
        if (!Futile.atlasManager.DoesContainAtlas(name))
        {
            if (!Futile.atlasManager.DoesContainAtlas(name))
            {
                Texture2D texture = new Texture2D(1, 1, GraphicsFormat.R8G8B8A8_SRGB, 0, TextureCreationFlags.None);
                texture.LoadImage(File.ReadAllBytes(AssetManager.ResolveFilePath(path)));
                texture.filterMode = FilterMode.Point;
                Futile.atlasManager.LoadAtlasFromTexture(name, texture, false);
            }
        }

        if (!Futile.atlasManager.DoesContainAtlas(name))
        {
            Custom.LogWarning($"Image {name} ({path}) failed to load!");
        }
    }

    public static void FromBezier(this TriangleMesh mesh, Vector2 A, Vector2 cA, Vector2 B, Vector2 cB, Func<float,float> thicknessFunction)
    {
        for (int i = 0; i < mesh.vertices.Length / 4; i++)
        {
            float thickness = thicknessFunction.Invoke(i / (mesh.vertices.Length / 4f));
            float nextThickness = thicknessFunction.Invoke((i + 1) / (mesh.vertices.Length / 4f));

            Vector2 p(float j) => Custom.Bezier(A, cA, B, cB, j / (mesh.vertices.Length / 4f));
            mesh.MoveVertice(0 + i * 4, p(i) + Custom.PerpendicularVector(p(i + 1)) * thickness );
            mesh.MoveVertice(1 + i * 4, p(i) - Custom.PerpendicularVector(p(i + 1)) * thickness );
            mesh.MoveVertice(2 + i * 4, p(i + 1) + Custom.PerpendicularVector(p(i + 2)) * nextThickness );
            mesh.MoveVertice(3 + i * 4, p(i + 1) - Custom.PerpendicularVector(p(i + 2)) * nextThickness );
        }
    }

    public static void FromBezier(this TriangleMesh mesh, Vector2 A, Vector2 cA, Vector2 B, Vector2 cB, float thickness)
    {
        mesh.FromBezier(A, cA, B, cB, _ => thickness);
    }

    public static void FromBezier(this TriangleMesh mesh, Vector2 A, Vector2 cA, Vector2 B, Vector2 cB, float thicknessA, float thicknessB)
    {
        mesh.FromBezier(A, cA, B, cB, t => Mathf.Lerp(thicknessA, thicknessB, t));
    }
}