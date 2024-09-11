using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFatigue : MonoBehaviour
{
    public int maxFatigue = 5; // Maximum fatigue level
    public int currentFatigue; // Current fatigue level
    // Start is called before the first frame update
    void Start()
    {
        currentFatigue = maxFatigue; // Initialize fatigue
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Call this to recover fatigue (e.g., at the start of a new turn)
    public void RecoverFatigue(int amount)
    {
        currentFatigue = Mathf.Min(maxFatigue, currentFatigue + amount);
    }
}
