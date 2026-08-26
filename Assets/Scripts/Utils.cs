using Unity.Mathematics;
using UnityEngine;

public static class Utils
{
    public static Vector2 GetMouseWorldPosition()
    {
        // Берем позицию мыши на экране (в пикселях) и переводим в игровой мир через главную камеру
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(vec.x, vec.y);
    }

    public static Vector2 GetDirectionToMouse(Vector2 pos)
    {
        Vector2 mousePos = GetMouseWorldPosition();
        return (mousePos - pos).normalized;
    }

    public static Quaternion GetRotationToMouse(Vector2 pos)
    {
        Vector2 direction = GetDirectionToMouse(pos);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        return rotation;
    }



    public static LineRenderer CreateSimpleLine(Color color, float width)
    {
        GameObject lineObj = new GameObject("Line");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = color;
        line.startWidth = width;
        line.endWidth = width;
        line.positionCount = 2;

        return line;
    }
}