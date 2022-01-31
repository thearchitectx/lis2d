using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D[] m_Cursors;
    [SerializeField] private Vector2[] m_Hotspots;
    private static bool HideRequested;
    private static int RequestedCursor;

    public static void RequestHide()
    {
        HideRequested = true;
    }

    public static void RequestTargetCursor()
    {
        RequestedCursor = 1;
    }

    public static void RequestTargetHand()
    {
        RequestedCursor = 2;
    }

    void Update()
    {
        Cursor.visible = !HideRequested;
        Cursor.SetCursor(m_Cursors[RequestedCursor], m_Hotspots[RequestedCursor], CursorMode.Auto);

        HideRequested = false;
        RequestedCursor = 0;
    }
}