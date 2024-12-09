using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Cursor = UnityEngine.Cursor;

public class TextInputHandler : MonoBehaviour
{
    public InputField textInputField;
    public RandomWalker walker;
    public AlphabethScriptableObject alphabeth;
    public GameObject canvasGo;
    public Player player;
    public DistanceBasedReaderController readerController;

    [CanBeNull] private GameObject effectGo;
    
    private bool justSubbmitted = false;
    private bool readyToRestart = true;
    public void Submit(string text)
    {
        // use 1.0f as scaling for everything
        string textToDisplay = textInputField.text;
        List<float> letterScaling = Enumerable.Repeat(2f ,TextUtil.ToSingleLine(textToDisplay).Replace(" ", "").Length).ToList();
        effectGo = VFXUtil.CreateEffectFromPath(walker.GetPath(), textToDisplay, alphabeth, letterScaling);
        
        readerController.Reset();
        readerController.SetEffect(effectGo.GetComponent<VisualEffect>());
        
        effectGo.GetComponent<VisualEffect>().SetVector3("SpawnPosition", player.transform.position);
        
        canvasGo.SetActive(false);
        player.movementActive = true;
        
        justSubbmitted = true;
    }

    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (justSubbmitted)
            {
                justSubbmitted = false;
                readyToRestart = true;
                return;
            }
            if (readyToRestart)
            {
                walker.Reset();
                walker.pause = false;
                readyToRestart = false;

                readerController.StopReading();
                readerController.Reset();
                DestroyImmediate(effectGo);
                
                return;
            }
            
            walker.pause = true;
            canvasGo.SetActive(true);
            player.movementActive = false;
            
            EventSystem.current.SetSelectedGameObject(textInputField.gameObject, null);
            textInputField.OnPointerClick(new PointerEventData(EventSystem.current));
        }
    }
}
