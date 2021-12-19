
using UnityEngine;
using UnityEngine.UI;

public class AspectEnforcer : MonoBehaviour
{
  [SerializeField] public AspectRatioFitter Fitter;

  void OnRectTransformDimensionsChange()
  {
      if (Camera.main == null)
        return;
        
      // Determine ratios of screen/window & target, respectively.
      float screenRatio = Screen.width / (float)Screen.height;

      if(Mathf.Approximately(screenRatio, Fitter.aspectRatio)) {
          // Screen or window is the target aspect ratio: use the whole area.
          Camera.main.rect = new Rect(0, 0, 1, 1);
      }
      else if(screenRatio > Fitter.aspectRatio) {
          // Screen or window is wider than the target: pillarbox.
          float normalizedWidth = Fitter.aspectRatio / screenRatio;
          float barThickness = (1f - normalizedWidth)/2f;
          Camera.main.rect = new Rect(barThickness, 0, normalizedWidth, 1);
      }
      else {
          // Screen or window is narrower than the target: letterbox.
          float normalizedHeight = screenRatio / Fitter.aspectRatio;
          float barThickness = (1f - normalizedHeight) / 2f;
          Camera.main.rect = new Rect(0, barThickness, 1, normalizedHeight);
      }
  }

}
