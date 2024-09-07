using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public bool isIdle = false; 
    public GameObject particleSystemObject; 
    public IdleAnimation animator;
    private Timer timer; 
    private bool wasIdle = false;

    void Start()
    {

        if (particleSystemObject == null)
            particleSystemObject = GetComponentInChildren<ParticleSystem>().gameObject;

        if (animator == null)
            animator = GetComponentInChildren<IdleAnimation>();

        timer = gameObject.GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isIdle)
        {
            particleSystemObject.SetActive(false);

            if (animator != null)
                animator.enabled = false;
            if (!wasIdle)
                wasIdle = true;
            timer.StopTimer();
        }
        else
        {
            if (!timer.IsRunning() && wasIdle)
            {
                timer.StartTimer();
            }
            if (timer.GetElapsedTime() >= 2f)
            {
                particleSystemObject.SetActive(true);

                if (animator != null)
                    animator.enabled = true;
                timer.StopTimer();
            }
            wasIdle = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        isIdle = true;
        timer.StopTimer();
    }

    // Called when the collision ends
    void OnCollisionExit(Collision collision)
    {
        isIdle = false; 
        if(wasIdle)
        {
            timer.StartTimer();
        }
         
    }
}
