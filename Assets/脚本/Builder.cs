using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mech
{
    public int index;
    public float cost;
    public GameObject prefabs;
   
}


public class Builder : SnapAble
{
    public Transform spawnPosition;
    public List<Mech> mechList;
    public void Building(int index)
    {
        foreach(Mech mech in mechList)
        {
            if(mech.index == index)
            {
                Instantiate(mech.prefabs, spawnPosition.position, Quaternion.identity);
            }
        }
    }



}
