using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiamondCollector : MonoBehaviour
{
    public int numberOfDiamonds { get; private set; }

    public UnityEvent<DiamondCollector> OnDiamondCollected;
    public void DiamondCollected()
    {
        numberOfDiamonds++;
        OnDiamondCollected.Invoke(this);
    }
}
