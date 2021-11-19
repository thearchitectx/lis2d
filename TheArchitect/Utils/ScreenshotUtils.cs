using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheArchitect
{
    public class ScreenshotUtils
    {
        
        public static IEnumerator TakeScreenshot(int widthScale, System.Action<Texture2D> callBack)
        {
            List<Canvas> allCanvasEnabledCanvas = GameObject.FindObjectsOfType<Canvas>().Where( c => c.enabled ).ToList();
            allCanvasEnabledCanvas.ForEach( c => c.enabled = false );

            yield return new WaitForEndOfFrame();
            
            Texture2D texScreen = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            try {
                texScreen.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
                texScreen.Apply();
                texScreen = TextureScaler.scaled(texScreen, widthScale, Mathf.CeilToInt(1.0f * widthScale / Screen.width * Screen.height));
            } finally {
                // Recover from screenshot
                allCanvasEnabledCanvas.ForEach( c => c.enabled = true );
            }

            callBack.Invoke(texScreen);
        }
    }
}