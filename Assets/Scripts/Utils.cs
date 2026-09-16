using Unity.Mathematics;
using UnityEngine;

public static class Utils
{
    private static Plane _groundPlane = new Plane(Vector3.forward, Vector3.zero);

    public static Vector2 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (_groundPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            return new Vector2(point.x, point.y);
        }
        return Vector2.zero;
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