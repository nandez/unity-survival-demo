using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MinimapController : MonoBehaviour
{
    [SerializeField] private Material indicatorMat;
    [SerializeField] private float lineWidth;
    private Camera minimapCam;

    void Awake()
    {
        minimapCam = GetComponent<Camera>();
    }

    void OnPostRender()
    {
        (Vector3 minWorldPoint, Vector3 maxWorldPoint) = Utilities.GetWorldSize();
        Vector3 minViewportPoint = minimapCam.WorldToViewportPoint(minWorldPoint);
        Vector3 maxViewportPoint = minimapCam.WorldToViewportPoint(maxWorldPoint);
        float minX = minViewportPoint.x;
        float minY = minViewportPoint.y;
        float maxX = maxViewportPoint.x;
        float maxY = maxViewportPoint.y;

        GL.PushMatrix();
        {
            indicatorMat.SetPass(0);
            GL.LoadOrtho();

            GL.Begin(GL.QUADS);
            GL.Color(new Color(1f, 1f, 0.85f));
            {
                GL.Vertex(new Vector3(minX, minY + lineWidth, 0));
                GL.Vertex(new Vector3(minX, minY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, minY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, minY + lineWidth, 0));

                GL.Vertex(new Vector3(minX + lineWidth, minY, 0));
                GL.Vertex(new Vector3(minX - lineWidth, minY, 0));
                GL.Vertex(new Vector3(minX - lineWidth, maxY, 0));
                GL.Vertex(new Vector3(minX + lineWidth, maxY, 0));

                GL.Vertex(new Vector3(minX, maxY + lineWidth, 0));
                GL.Vertex(new Vector3(minX, maxY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, maxY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, maxY + lineWidth, 0));

                GL.Vertex(new Vector3(maxX + lineWidth, minY, 0));
                GL.Vertex(new Vector3(maxX - lineWidth, minY, 0));
                GL.Vertex(new Vector3(maxX - lineWidth, maxY, 0));
                GL.Vertex(new Vector3(maxX + lineWidth, maxY, 0));
            }
            GL.End();
        }
        GL.PopMatrix();
    }
}
