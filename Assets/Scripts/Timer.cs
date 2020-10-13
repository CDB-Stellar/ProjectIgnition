using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    //public varibles
    [SerializeField] private float targetTime;
    [SerializeField] private bool timerActive = false; //is the timer ticking?
    //private varibles
    private float totalTime; //time left on timer
    
    public bool timerDone { get; private set; } //timer is done
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            totalTime -= Time.deltaTime;
            if (totalTime <= 0)
            {
                timerDone = true; 
                EndTimer();
            }
        }
    }
    public void ResetTimer()
    {
        timerDone = false;
        totalTime = targetTime;
    }
    public void StartTimer() 
    {
        if (timerDone)        
            Debug.LogError(this.GetInstanceID().ToString() + "Timer has not been reset");
        else
            timerActive = true;
    }
    public void EndTimer()
    {
        timerActive = false;
        timerDone = true;
    }
}
