using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private GameObject logPrefab;
    [SerializeField] private Transform holdItemPosition;

    private GameObject currentItem;
    //private ThirdPersonCharacter tpcController;

    void Awake()
    {
        //tpcController = GetComponent<ThirdPersonCharacter>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Tree"))
        {
            Debug.Log("Colisión con Arbol");
            if (currentItem == null)
            {
                currentItem = Instantiate(logPrefab, holdItemPosition.position, Quaternion.identity);
                currentItem.transform.SetParent(holdItemPosition);

                ThirdPersonCharacter.m_MoveSpeedMultiplier = 0.5f;
            }

        }
        else if(coll.gameObject.CompareTag("Home") && currentItem != null)
        {

        }
    }
}
