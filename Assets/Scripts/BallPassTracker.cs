using System.Collections.Generic;
using UnityEngine;

public class BallPassTracker : MonoBehaviour
{
    public static BallPassTracker SharedInstance { get ; private set; }
    private HashSet<int> enteredTop = new HashSet<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (SharedInstance != null && SharedInstance != this)
        {
            Destroy(this);
            return;
        } 

        SharedInstance = this;

    }

    public void MarkEnteredTop(Rigidbody rb)
    {
        if (rb == null) return;
        enteredTop.Add(rb.GetInstanceID());
    }

    public void ClearEnteredTop(Rigidbody rb)
    {
        if (rb == null) return;
        enteredTop.Remove(rb.GetInstanceID());
    }

    public bool HasEnteredTop(Rigidbody rb)
    {
        if (rb == null || !enteredTop.Contains(rb.GetInstanceID()))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
