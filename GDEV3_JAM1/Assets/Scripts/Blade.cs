using UnityEngine;
using System.Collections.Generic;

public class Blade : MonoBehaviour
{
    public List<GameObject> bladeHits = new List<GameObject>();

    public float baseDamage;
    public float weakDamage;
    public float buffedDamage;
}