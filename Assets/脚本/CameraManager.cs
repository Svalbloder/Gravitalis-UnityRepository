using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance; // µ¥ÀýÊµÀý;
    public GameObject cm1;
    public GameObject cm2;

    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    public void ChangeToNear()
    {
        cm2.SetActive(true);
    }
    public void ChangeBack()
    {
        cm2.SetActive(false);
    }

}
