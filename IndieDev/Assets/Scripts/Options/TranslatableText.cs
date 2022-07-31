using TMPro;
using UnityEngine;

public class TranslatableText : MonoBehaviour
{
    public string frenchText;
    public string englishText;
    [HideInInspector] public TextMeshProUGUI translatableText;
    
    private void Start()
    {
        translatableText = GetComponent<TextMeshProUGUI>();
        if (translatableText == null)
        {
            Debug.LogError("No TextMeshProUGUI available!");
            return;
        }
        switch (GameManager.instance.settings.currentLanguage)
        {
            case Language.French:
                translatableText.text = frenchText;
                break;
            case Language.English:
                translatableText.text = englishText;
                break;
        }
        GameManager.instance.allTranslatableTexts.Add(this);
    }
}
