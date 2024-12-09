using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsTracker : MonoBehaviour
{
    public float displayRate = 1;
    
    private float _deltaTime = 0f;
    private Text _fpsText;

    private float _timeSinceLastDisplay = 0f;
    private LinkedList<float> _fpsCalculated = new LinkedList<float>();

    private void Start()
    {
        _fpsText = gameObject.GetComponent<Text>();
    }

    void Update () {
        // update time since last display
        _timeSinceLastDisplay += Time.deltaTime;
        
        // calc fps
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsCalculated.AddLast(fps); // save calculated fps

        // check if display should happen
        if (_timeSinceLastDisplay > (1f / displayRate))
        {
            // calc average fps over last time span
            float summedFps = 0f;
            foreach (var calcFps in _fpsCalculated)
                summedFps += calcFps;
            float averageFps = summedFps / _fpsCalculated.Count;
            // display average
            _fpsText.text = Mathf.Ceil(averageFps).ToString();
            // remove past fps values
            _fpsCalculated.Clear();
            // reset time to display again
            _timeSinceLastDisplay = 0f;
        }
    }
}
