using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [SerializeField] private Transform topBar;
    [SerializeField] private GameObject resourceUIPrefab;

    private Dictionary<string, Text> resourceTexts = new Dictionary<string, Text>();

    void Start()
    {
        // Iteramos sobre cada recurso para crear los elementos de UI
        // que van a indicar la cantidad actual de cada uno.
        foreach (var resource in GameManager.Resources)
        {
            // Creamos un nuevo gameobject y asignamos nombre e ícono del recurso actual..
            var display = Instantiate(resourceUIPrefab, topBar);
            display.name = $"ui_{resource.Key}";

            Debug.Log($"Sprites/{resource.Key}_icon");
            display.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/{resource.Key}_icon");

            // Actualizamos el valor del diccionario en la clave que matchee el recurso actual (ej: "wood")
            resourceTexts[resource.Key] = display.transform.Find("Text").GetComponent<Text>();
            SetResourceText(resource.Key, resource.Value.Amount, resource.Value.MaxAmount);


        }
    }


    private void SetResourceText(string resourceKey, int value, int maxValue)
    {
        if (resourceTexts.ContainsKey(resourceKey))
            resourceTexts[resourceKey].text = $"{value} / {maxValue}";
    }

    public void UpdateResourceTexts()
    {
        foreach (var resource in GameManager.Resources)
            SetResourceText(resource.Key, resource.Value.Amount, resource.Value.MaxAmount);
    }
}
