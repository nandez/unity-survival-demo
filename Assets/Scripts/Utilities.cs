using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    private static Camera mainCam;
    public static Camera MainCam
    {
        get
        {
            if (mainCam == null)
                mainCam = Camera.main;

            return mainCam;
        }
    }

    private static Ray ray;
    private static RaycastHit hit;

    public static (Vector3, Vector3) GetWorldSize()
    {
        var bottomLeftCorner = new Vector3(0f, 0f);
        var topRightCorner = new Vector3(1f, 1f);

        var dist = MainCam.transform.position.y * 2;

        ray = MainCam.ViewportPointToRay(bottomLeftCorner);
        var bottomLeft = GameManager.Instance.mapWrapperCollider.Raycast(ray, out hit, dist) ? hit.point : new Vector3();

        ray = MainCam.ViewportPointToRay(topRightCorner);
        var topRight = GameManager.Instance.mapWrapperCollider.Raycast(ray, out hit, dist) ? hit.point : new Vector3();

        return (bottomLeft, topRight);
    }
}
