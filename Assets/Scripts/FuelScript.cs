using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelScript : MonoBehaviour
{
    public float fuelAmount;
    [Range(0f,1f)]public float maxIncrease; //Range from 0 - 1 to make balancing easier
}
