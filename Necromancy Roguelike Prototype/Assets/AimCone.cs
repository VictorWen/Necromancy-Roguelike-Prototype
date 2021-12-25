using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCone : MonoBehaviour
{
    [SerializeField] private LineRenderer cone;
    [SerializeField] private LineRenderer aimLine;
    
    [SerializeField] private float angle = 1;
    [SerializeField] private float rotation = 0;
    [SerializeField] private float length = 5;

    public float Angle { get { return angle; } }

    public float Rotation { get { return rotation; } }

    public bool IsActive { get; private set; }

    public void SetAngle(float angle)
    {
        this.angle = angle;
        Draw();
    }

    public void SetRotation(float rotation)
    {
        this.rotation = rotation;
        Draw();
    }

    private void Draw()
    {
        float angle = this.angle / 2;
        float y = length * Mathf.Sin(angle);
        float x = length * Mathf.Cos(angle);

        // Rotate x and y
        float cos = Mathf.Cos(rotation);
        float sin = Mathf.Sin(rotation);
        
        float y0 = x * sin + y * cos;
        float x0 = x * cos - y * sin;

        float y1 = x * sin - y * cos;
        float x1 = x * cos + y * sin;

        cone.SetPosition(0, new Vector3(x0, y0, 0));
        cone.SetPosition(2, new Vector3(x1, y1, 0));

        aimLine.SetPosition(1, new Vector3(length * cos, length * sin, 0));
    }

    public void Show()
    {
        IsActive = true;
        cone.gameObject.SetActive(true);
        aimLine.gameObject.SetActive(true);
    }

    public void Hide()
    {
        IsActive = false;
        cone.gameObject.SetActive(false);
        aimLine.gameObject.SetActive(false);
    }

    public float RandomAim()
    {
        float randAngle = Random.Range(0, angle);
        randAngle -= angle / 2;
        randAngle += rotation;

        return randAngle;
    }
}
