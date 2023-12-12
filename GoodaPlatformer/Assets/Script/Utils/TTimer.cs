using System;
using System.Collections.Generic;
using UnityEngine;

public class TTimer
{
    class TMono : MonoBehaviour
    {
        public Action OnUpdate;

        void Update()
        {
            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }
    }

    // Timer list hold all running timers
    static List<TTimer> m_timerList;

    // Container is a gameobject that hold timer list
    static GameObject m_container;

    static void Initialization()
    {
        if (m_container == null)
        {
            m_container = new GameObject("TimerContainer");
            m_timerList = new List<TTimer>();
        }
    }

    public static TTimer AddTimer(Action action, float time, string name = null)
    {
        Initialization();

        GameObject timerObject = new GameObject("TTimer", typeof(TMono));
        TTimer timer = new TTimer(action, time, timerObject, name);
        timerObject.GetComponent<TMono>().OnUpdate = timer.Update;
        m_timerList.Add(timer);
        return timer;
    }

    public static void StopTimer(string name)
    {
        List<TTimer> result = new List<TTimer>();
        for (int i = 0; i < m_timerList.Count; i++)
        {
            if (m_timerList[i].m_name == name)
            {
                result.Add(m_timerList[i]);
            }
        }

        for (int i = 0; i < result.Count; i++)
        {
            result[i].SelfDestroy();
        }
    }

    static void RemoveTimer(TTimer timer)
    {
        Initialization();
        m_timerList.Remove(timer);
    }

    Action m_action;
    float m_timer;
    bool m_isActive;
    GameObject m_owner;
    string m_name;

    TTimer(Action action, float timer, GameObject owner, string name = null)
    {
        m_action = action;
        m_timer = timer;
        m_isActive = true;
        m_owner = owner;
        m_name = name;
    }

    void Update()
    {
        if (m_isActive)
        {
            m_timer -= Time.deltaTime;
            if (m_timer < 0)
            {
                //Trigger action
                m_action();
                SelfDestroy();
            }
        }
    }

    public void SelfDestroy()
    {
        m_isActive = false;
        UnityEngine.Object.Destroy(m_owner);
        RemoveTimer(this);
    }
}