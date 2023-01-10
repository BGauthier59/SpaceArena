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
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Toggle rumbleToggle;
    
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
        Initialization();
    }

    private void Initialization()
    {
        if (!GameManager.instance)
        {
            Debug.LogWarning("Game Manager hasn't been found!");
            return;
        }
        
        GameManager.instance.TranslateTexts();

        var settings = GameManager.instance.settings;
        foreach (var sliderGui in sliders)
        {
            switch (sliderGui.type)
            {
                case SliderType.Main:
                    sliderGui.slider.value = settings.mainVolumeValue;
                    break;
                case SliderType.Music:
                    sliderGui.slider.value = settings.musicVolumeValue;
                    break;
                case SliderType.Sfx:
                    sliderGui.slider.value = settings.sfxVolumeValue;
                    break;
                default:
                    Debug.LogError("This type is invalid!");
                    return;
            }
        }

        languageDropdown.value = settings.currentLanguage switch
        {
            Language.French => (int)Language.French,
            Language.English => (int)Language.English,
            _ => (int)Language.English // English by default
        };

        rumbleToggle.isOn = GameManager.instance.settings.rumbleActivated;
        
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
                
        var value = (int)sliderGUI.slider.value;
        sliderGUI.value.text = value.ToString();

        var settings = GameManager.instance.settings;

        switch (sliderGUI.type)
        {
            case SliderType.Main:
                settings.mainVolumeValue = value;
                break;
            case SliderType.Music:
                settings.musicVolumeValue = value;
                break;
            case SliderType.Sfx:
                settings.sfxVolumeValue = value;
                break;
            default:
                Debug.LogError("This type is invalid!");
                return;
        }
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

    public void OnRumbleToggle(bool active)
    {
        if (!GameManager.instance)
        {
            Debug.LogWarning("Game Manager hasn't been found!");
            return;
        }
        GameManager.instance.settings.rumbleActivated = active;
    }
}
