using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuOptionsManager : MonoBehaviour
{
    private EventSystem eventSystem;
    [SerializeField] private SliderGUI[] sliders;
    private GameObject lastSelected;

    [Serializable]
    public struct SliderGUI
    {
        public Slider slider;
        public TextMeshProUGUI value;
        public SliderType type;
    }

    public enum SliderType
    {
        Main, Music, Sfx
    }

    private void Start()
    {
        eventSystem = EventSystem.current;
        lastSelected = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(sliders[0].slider.gameObject);
    }

    public void OnSliderChange(int type)
    {
        var sliderType = (SliderType) type;
        var sliderGUI = new SliderGUI();
        
        foreach (var slider in sliders)
        {
            if (slider.type != sliderType) continue;
            sliderGUI = slider;
            break;
        }
        
        sliderGUI.value.text = sliderGUI.slider.value.ToString();
    }

    public void OnQuit()
    {
        eventSystem.SetSelectedGameObject(lastSelected);
        SceneManager.UnloadSceneAsync(GameManager.instance.optionMenuIndex);
    }

    public void OnLanguageChanged(int index)
    {
        Language language;
        
        switch (index)
        {
            case 0:
                language = Language.French;
                break;
            
            case 1:
                language = Language.English;
                break;
            
            default:
                Debug.LogError("Language not available, set to english by default");
                language = Language.English;
                break;
        }

        GameManager.instance.settings.currentLanguage = language;
        GameManager.instance.TranslateTexts();
    }
    
}
