using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleManager : MonoBehaviour
{
    private static IDictionary<float, float> m_Requests = new Dictionary<float, float>();
    private static float m_CurrentRequestPriority = float.NaN;
    private static float m_CurrentRequestTimeScale = float.NaN;

    public static void Register(float priority, float timeScale)
    {
        m_Requests[priority] = timeScale;
        UpdateCurrentRequest();
    }

    public static void Unregister(float priority)
    {
        m_Requests.Remove(priority);
        UpdateCurrentRequest();
    }

    public static void UnregisterAll()
    {
        m_Requests.Clear();
        UpdateCurrentRequest();
    }

    private static void UpdateCurrentRequest()
    {
        m_CurrentRequestPriority = float.NaN;
        m_CurrentRequestTimeScale = float.NaN;

        if (m_Requests.Count > 0)
        {
            float minPriority = m_Requests.Keys.Min();
            m_CurrentRequestPriority = minPriority;
            m_CurrentRequestTimeScale = m_Requests[minPriority];
        }
    }

    void Update()
    {
        if (!float.IsNaN(m_CurrentRequestPriority))
            Time.timeScale = m_CurrentRequestTimeScale;
        else
            Time.timeScale = 1;
    }
    
}