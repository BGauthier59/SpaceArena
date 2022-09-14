using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BaseElementManager : Entity
{
    [Header("Destroy")] 
    [SerializeField] private GameObject onDestroyText;
    
    [Header("Reparation")] 
    [SerializeField] private int reparationInputs;

    private int reparationInputsCounter;
    [SerializeField] private ReparationArea[] allReparationAreas;
    [SerializeField] private GameObject reparationIcon;

    private ReparationArea lastCheckingArea;
    private ReparationArea currentCheckingArea;

    private bool isIconMoving;
    [SerializeField] private float iconMoveDuration;
    private float iconMoveTimer;
    
    public Material reparationAreaDeviceEnabled;

    [Header("GUI")] 
    [SerializeField] private GameObject baseElementInfo;
    [SerializeField] private Slider lifeSlider;
    [SerializeField] private Image lifeSliderFill;
    [SerializeField] private TextMeshProUGUI baseElementNameText;
    [SerializeField] private TextMeshProUGUI baseElementLifeText;
    [SerializeField] private BaseElementName baseElementName;
    
    [Serializable]
    private struct BaseElementName
    {
        public string frenchName;
        public string englishName;
    }
    
    #region Entity

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        SetLifeSlider();
    }

    public override void Death()
    {
        if (isDead) return;
        OnDestroyed();
        base.Death();
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
        SetLifeSlider();
    }

    #endregion

    public override void Start()
    {
        base.Start();

        InitializeBaseElementInfo();
        BaseManager.instance.allBaseElements.Add(this);
        foreach (var area in allReparationAreas)
        {
            area.associatedElement = this;
        }
    }

    public virtual void OnDestroyed()
    {
        onDestroyText.SetActive(true);
        foreach (var area in allReparationAreas)
        {
            area.ActivateArea();
        }
    }

    public bool TryToRepair()
    {
        return reparationInputsCounter >= reparationInputs;
    }

    public virtual void OnFixed()
    {
        onDestroyText.SetActive(false);
        CancelReparation();
        foreach (var area in allReparationAreas)
        {
            area.DeactivateArea();
        }

        isDead = false;
        Heal(totalLife);
    }

    public void CheckPlayersOnReparationAreas()
    {
        foreach (var area in allReparationAreas)
        {
            if (!area.isPlayerOn) return;
        }

        // Tous les joueurs sont là et prêts pour la réparation !
        BeginsReparation();
    }
    
    public void BeginsReparation()
    {
        // Faire apparaître l'icône de l'input   

        reparationIcon.SetActive(true);
        currentCheckingArea = allReparationAreas[0];
        reparationIcon.transform.position = allReparationAreas[0].iconPosition.position;
        allReparationAreas[0].isWaitingForInput = true;
    }

    public void CancelReparation()
    {
        // A tout moment si un joueur quitte la zone de réparation

        reparationIcon.SetActive(false);
        isIconMoving = false;
        currentCheckingArea = null;
        reparationInputsCounter = 0;
    }

    public void SetCheckingArea()
    {
        reparationInputsCounter++;
        currentCheckingArea.isWaitingForInput = false;

        if (TryToRepair()) OnFixed();
        else
        {
            if (allReparationAreas.Length <= 1) Debug.LogError("Only one reparation area!");
            ReparationArea nextArea;
            do
            {
                nextArea = allReparationAreas[Random.Range(0, allReparationAreas.Length)];
            } while (nextArea == currentCheckingArea);

            lastCheckingArea = currentCheckingArea;
            currentCheckingArea = nextArea;
            iconMoveTimer = 0f;
            isIconMoving = true;
        }
    }

    public virtual void Update()
    {
        if (isIconMoving)
        {
            if (iconMoveTimer >= iconMoveDuration)
            {
                reparationIcon.transform.position = currentCheckingArea.iconPosition.position;
                currentCheckingArea.isWaitingForInput = true;
                isIconMoving = false;
            }
            else
            {
                reparationIcon.transform.position = Vector3.Lerp(lastCheckingArea.iconPosition.position,
                    currentCheckingArea.iconPosition.position, iconMoveTimer / iconMoveDuration);

                iconMoveTimer += Time.deltaTime;
            }
        }
    }

    #region Trigger & Collision

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetBaseElementInfo(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetBaseElementInfo(false);
        }
    }

    #endregion

    #region GUI

    public void InitializeBaseElementInfo()
    {
        switch (GameManager.instance.settings.currentLanguage)
        {
            case Language.French:
                baseElementNameText.text = baseElementName.frenchName;
                break;
            
            case Language.English:
                baseElementNameText.text = baseElementName.englishName;
                break;
        }

        lifeSlider.maxValue = totalLife;

        baseElementInfo.transform.SetParent(GameManager.instance.mainCanvas.transform);
        SetLifeSlider();
        SetBaseElementInfo(false);
    }

    public void SetBaseElementInfo(bool active)
    {
        baseElementInfo.SetActive(active);
        baseElementInfo.transform.position = Camera.main.WorldToScreenPoint(transform.position + BaseManager.instance.baseElementInfoOffset);
    }
    
    public void SetLifeSlider()
    {
        lifeSlider.value = currentLife;
        lifeSliderFill.color = BaseManager.instance.baseLifeGradient.Evaluate((float) currentLife / totalLife);
        baseElementLifeText.text = $"{currentLife}/{totalLife}";
    }

    #endregion
}