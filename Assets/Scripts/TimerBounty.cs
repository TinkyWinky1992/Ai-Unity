using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBounty : MonoBehaviour

{
    private float timer = 0;
    private bool is_timer_running = true;
    private readonly float MAX_TIMER = 5f;

    public void onStop()
    {
        is_timer_running = false;
        timer = 0;
    }

    public void onResume()
    {
        is_timer_running = true;
    }

    private void Update()
    {
        if (is_timer_running)
        {
            timer += Time.deltaTime;

        }
        else
            timer = 0;
    }

    public bool isTimerAbove()
    {
        if (timer > MAX_TIMER)
            return true;
        return false;
    }
}