using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Fuel : MonoBehaviour, IPointerDownHandler
{
    public float fuel;
    public void OnPointerDown(PointerEventData eventData)
    {
        CarManager.instance.FillFuel(fuel);
        Destroy(gameObject);
    }

}
