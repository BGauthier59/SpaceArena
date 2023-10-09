using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public abstract class BaseElementManager : Entity
{
    [Header("Destroy")] [SerializeField] private VisualEffect smokeVFX;

    [Header("Reparation")] private int areaCount;
    [SerializeField] private ReparationArea[] allReparationAreas;
    [SerializeField] private ReparationIconData reparationIconData;

    [Tooltip("A security collider that enables when repairing this base element, killing every entity inside")]
    [SerializeField]
    private Collider securityCollider;

    public Renderer[] elementColorRenderers;
    public Color color;

    [Header("GUI")] [SerializeField] private GameObject baseElementInfo;
    [SerializeField] private Slider lifeSlider;
    [SerializeField] private Image lifeSliderFill;
    [SerializeField] private TextMeshProUGUI baseElementNameText;
    [SerializeField] private TextMeshProUGUI baseElementLifeText;
    [SerializeField] private BaseElementName baseElementName;

    [SerializeField] private Animation baseElementNameAnim;
    [SerializeField] private TextMeshProUGUI baseElementNameAnimText;

    [Serializable]
    private struct BaseElementName
    {
        public string frenchName;
        public string englishName;
    }

    #region Entity

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage);
        SetLifeSlider();
    }

    protected override void Death(Entity killer)
    {
        if (isDead) return;
        OnDestroyed();
        base.Death(killer);
    }

    protected override void Heal(int heal)
    {
        base.Heal(heal);
        SetLifeSlider();
    }

    #endregion

    #region Initialization

    public override void Start()
    {
        base.Start();

        smokeVFX.Stop();

        InitializeBaseElementInfo();

        // Choix de couleur automatisé
        partyManager.baseManager.allBaseElements.Add(this);

        SetBaseElementColor();

        SetReparationsAreaNumber();
        foreach (var area in allReparationAreas)
        {
            if (!area.gameObject.activeSelf) continue;
            area.associatedElement = this;
        }
    }

    protected virtual void SetBaseElementColor()
    {
        color = partyManager.baseManager.baseElementColor[partyManager.baseManager.allBaseElements.Count - 1];
        foreach (var rd in elementColorRenderers)
        {
            rd.material = partyManager.baseManager.colorVariantMaterial;
            rd.material.color = color;
            rd.material.SetColor("_EmissionColor", color * 1);
        }
    }

    private void SetReparationsAreaNumber()
    {
        int counter = 1;
        int number = Random.Range(2, GameManager.instance.playersNumber + 1);

        foreach (var ra in allReparationAreas)
        {
            if (counter > number)
            {
                ra.gameObject.SetActive(false);
                continue;
            }

            ra.gameObject.SetActive(true);
            counter++;
        }

        areaCount = number;
    }

    #endregion
    
    protected virtual void OnDestroyed()
    {
        smokeVFX.Play();
        EnableReparation();
        
    }
    
    protected virtual void OnFixed()
    {
        // Feedbacks
        partyManager.arenaFeedbackManager.OnExcitementGrows?.Invoke(1);
        smokeVFX.Stop();

        // Players get points
        foreach (var area in allReparationAreas)
        {
            if (!area.gameObject.activeSelf) continue;
            area.currentPlayerOnArea.manager.GetPoint(partyManager.baseManager.reparationPoint, transform.position);
        }
        
        // Deactivate reparation
        CancelReparation();
        
        // Reborn
        isDead = false;
        Heal(totalLife);
        
        // Security colliders if needed
        if (securityCollider) StartCoroutine(EnablingSecurityCollider());
    }

    private IEnumerator EnablingSecurityCollider()
    {
        securityCollider.enabled = true;
        yield return new WaitForFixedUpdate();
        securityCollider.enabled = false;
    }

    #region Reparation

    public void EnableReparation()
    {
        foreach (var area in allReparationAreas)
        {
            area.ActivateArea();
        }
    }

    public bool IsEveryPlayerReady()
    {
        for (int i = 0; i < areaCount; i++)
        {
            if (!allReparationAreas[i].isPlayerOn) return false;
        }

        return true;
    }

    public void EveryPlayerIsReady()
    {
        // Feedbacks
    }

    public void TryRepair()
    {
        // Check is everyone is completed
        for (int i = 0; i < areaCount; i++)
        {
            if (!allReparationAreas[areaCount].isCompleted) return;
        }
        
        OnFixed();
    }

    public void CancelReparation()
    {
        reparationIconData.DisableReparationIcon();
        foreach (var area in allReparationAreas)
        {
            area.DeactivateArea();
        }
    }

    #endregion

    #region GUI

    private void InitializeBaseElementInfo()
    {
        baseElementNameText.text = GameManager.instance.settings.currentLanguage switch
        {
            Language.French => baseElementName.frenchName,
            Language.English => baseElementName.englishName,
            _ => baseElementNameText.text
        };

        lifeSlider.maxValue = totalLife;

        // Base Element Info
        var tr1 = baseElementInfo.transform;
        tr1.SetParent(partyManager.mainCanvas.transform);
        tr1.localRotation = Quaternion.identity;
        tr1.localScale = Vector3.one;

        // Base Element Name
        var tr2 = baseElementNameAnim.transform;
        tr2.SetParent(partyManager.mainCanvas.transform);
        tr2.localRotation = Quaternion.identity;
        tr2.localScale = Vector3.one;
        baseElementNameAnim.gameObject.SetActive(false);

        SetLifeSlider();
        SetBaseElementInfo(false);
    }

    public void SetBaseElementInfo(bool active)
    {
        baseElementInfo.SetActive(active);
        baseElementInfo.transform.position = transform.position + Vector3.forward * 2;
    }

    private void SetLifeSlider()
    {
        lifeSlider.value = currentLife;
        lifeSliderFill.color = partyManager.baseManager.baseLifeGradient.Evaluate((float)currentLife / totalLife);
        baseElementLifeText.text = $"{currentLife}/{totalLife}";
    }

    public void PlayBaseElementNameAnim()
    {
        baseElementNameAnimText.text = GameManager.instance.settings.currentLanguage switch
        {
            Language.French => baseElementName.frenchName,
            Language.English => baseElementName.englishName,
            _ => "No name found!"
        };

        baseElementNameAnim.gameObject.SetActive(true);
        baseElementNameAnim.transform.position = transform.position + Vector3.forward + Vector3.right * 4;
        baseElementNameAnim.Play(baseElementNameAnim.clip.name);
    } // Happens once when game starts

    #endregion
}