using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank : SnapAble
{
    public float volume;
    public float currentFuel;
    public bool isFull;
    public Model model;
    public override void Start()
    {
        base.Start();
        AllocateValues();
        CarManager.instance.totalFuelVolume += volume;
        CarManager.instance.fuelTankList.Add(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(currentFuel == volume)
        {
            isFull = true;
        }
        else
        {
            isFull = false;
        }
    }
    private void AllocateValues()
    {
        switch (model)
        {
            case Model.I:
                volume = 50f;
                break;
            case Model.II:
                volume = 150f;
                break;
            case Model.III:
                volume = 200f; ;
                break;
        }
    }
}
