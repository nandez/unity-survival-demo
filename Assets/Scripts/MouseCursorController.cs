using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorController : MonoBehaviour
{
    [Header("Cursor Icons")]
    [SerializeField] private Texture2D axeCursorSprite;
    [SerializeField] private Texture2D pickAxeCursorSprite;
    [SerializeField] private Texture2D attackCursorSprite;
    [SerializeField] private Texture2D hammerCursorSprite;


    public void OnMouseOverActionableItem(string actionableTag)
    {
        if (actionableTag?.Equals(Constants.TagNames.Wood) == true)
        {
            Cursor.SetCursor(axeCursorSprite, Vector2.zero, CursorMode.Auto);
        }
        else if (actionableTag?.Equals(Constants.TagNames.Ore) == true)
        {
            Cursor.SetCursor(pickAxeCursorSprite, Vector2.zero, CursorMode.Auto);
        }
        else if (actionableTag?.Equals(Constants.TagNames.Stone) == true)
        {
            Cursor.SetCursor(pickAxeCursorSprite, Vector2.zero, CursorMode.Auto);
        }
        else if (actionableTag?.Equals(Constants.TagNames.Workbench) == true)
        {
            Cursor.SetCursor(hammerCursorSprite, Vector2.zero, CursorMode.Auto);
        }
        else if (actionableTag?.Equals(Constants.TagNames.Enemy) == true)
        {
            Cursor.SetCursor(attackCursorSprite, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            // Por defecto, reseteamos el cursor...
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
