using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public GameObject Generator1;
    public GameObject Generator2;
    public GameObject Generator3;
    public Transform spawnPoint;
    public void SpawnPowerTank()
    {
        Instantiate(Generator1, spawnPoint.position, spawnPoint.rotation);
    }
    public void SpawnFuelTank()
    {
        Instantiate(Generator2, spawnPoint.position, spawnPoint.rotation);
    }
    public void SpawnGenerator()
    {
        Instantiate(Generator3, spawnPoint.position, spawnPoint.rotation);
    }
}
