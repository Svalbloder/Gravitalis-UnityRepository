using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
public class Relic : MonoBehaviour, IPointerDownHandler
{
    public GameObject relickObject;
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));

        spawnPosition.z = 0; 

        Instantiate(relickObject, spawnPosition, Quaternion.identity);

        Destroy(gameObject);
    }

}



