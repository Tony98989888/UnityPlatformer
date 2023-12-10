

using System;
using UnityEngine;

public class TTimer
{
    Action m_action;
    float m_timer;
    
    public TTimer(Action action, float timer)
    {
        m_action = action;
        m_timer = timer;
    }

    void Update()
    {
        m_timer -= Time.deltaTime;
        if (m_timer < 0)
        {
            //Trigger action
            m_action();
        }
    }
}
