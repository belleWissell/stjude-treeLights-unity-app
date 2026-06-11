using UnityEngine;
using AAMVC.Unity;

namespace AAMVC.Unity 
{

  public class SmoothedFloat
  {
    public float value = 0f; // Filtered value
    public float delay = 0f; // Mean delay
    public bool reset = true; // Reset on Next Update

    public void SetBlend(float blend, float deltaTime = 1f)
    {
      delay = deltaTime * blend / (1f - blend);
    }
    public float Update(float input, float deltaTime = 1f) {
      if (deltaTime > 0f && !reset) {
        float alpha = delay / deltaTime;
        float blend = alpha / (1f + alpha);
        // NOTE: If delay -> 0 then blend -> 0,
        // reducing the filter to this.value = value.
        // NOTE: If deltaTime -> 0 blend -> 1,
        // so the change in the filtered value will be suppressed
        value = Mathf.Lerp(value, input, 1f - blend);
      } else {
        value = input;
        reset = false;
      }
      return value;
    }
  }

  public class FPSTextObj : MonoBehaviour
  {
    //public AAMVC.Unity.ApplicationControl appControlObj;
    private float prevFPS = 0.0f;
    
    //[SerializeField]
    //private LeapProvider _provider;

    [SerializeField]
    private TextMesh _frameRateText;

    private SmoothedFloat _smoothedRenderFps = new SmoothedFloat();

    private void OnEnable() {
      //if (_provider == null) {
      //  _provider = Hands.Provider;
      //}

      if (_frameRateText == null) {
        _frameRateText = GetComponentInChildren<TextMesh>();
        if (_frameRateText == null) {
          Debug.LogError("Could not enable FpsLabel because no TextMesh was specified!");
          enabled = false;
        }
      }

      _smoothedRenderFps.delay = 0.3f;
      _smoothedRenderFps.reset = true;
    }

    private void Update() {
      _frameRateText.text = "";

      /*
      if (_provider != null) {
        Frame frame = _provider.CurrentFrame;

        if (frame != null) {
          _frameRateText.text += "Data FPS:" + _provider.CurrentFrame.CurrentFramesPerSecond.ToString("f2");
          _frameRateText.text += System.Environment.NewLine;
        }
      }
      */
      
      if (Time.smoothDeltaTime > Mathf.Epsilon) {
        _smoothedRenderFps.Update(1.0f / Time.smoothDeltaTime, Time.deltaTime);
      }
    
      _frameRateText.text += "Render FPS:" + Mathf.RoundToInt(_smoothedRenderFps.value).ToString("f2");
      if (_smoothedRenderFps.value != prevFPS)
      {
        prevFPS = _smoothedRenderFps.value;
        //appControlObj.updateFPS(prevFPS);
      }
    }
    
    public void setWindowSize(int whichW, int whichH)
    {
      float currentScreenW = whichW/100f; // 3d world is different scale (not pixels)
      float currentScreenH = whichH/100f; 
            
      //Vector3 upperRight = new Vector3(5000f+whichW/2f - targetWidth/2f - 5f, whichH/2f- targetHeight/2f - 5f, 500f);
      Vector3 upperCenter = new Vector3(0, currentScreenH/2f- 1f, 9.5f);
      Vector3 upperLeft = new Vector3(0.5f-currentScreenW/2f, currentScreenH/2f- 0.16f, 9.5f);
      this.transform.localPosition = upperLeft;

    }
  }
}
