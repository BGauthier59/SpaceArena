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

    [Header("Reparation")] [SerializeField]
    private int reparationInputs;

    private int reparationInputsCounter;
    [SerializeField] private ReparationArea[] allReparationAreas;
    [SerializeField] private ReparationIconData reparationIconData;

    [Tooltip("A security collider that enables when repairing this base element, killing every entity inside")]
    [SerializeField]
    private Collider securityCollider;

    private ReparationArea lastCheckingArea;
    private ReparationArea currentCheckingArea;

    private bool isIconMoving;
    [SerializeField] private float iconMoveDuration;
    private float iconMoveTimer;

    public Renderer[] elementColorRenderers;
    public Color color;

    [Header("GUI")] [SerializeField] private GameObject baseElementInfo;
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

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage);
        SetLifeSlider();
    }

    protected override void Death()
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
    }

    protected virtual void OnDestroyed()
    {
        smokeVFX.Play();
        foreach (var area in allReparationAreas)
        {
            area.ActivateArea();
        }
    }

    private bool TryToRepair()
    {
        return reparationInputsCounter >= reparationInputs;
    }

    protected virtual void OnFixed()
    {
        partyManager.arenaFeedbackManager.OnExcitementGrows?.Invoke(1);

        smokeVFX.Stop();

        foreach (var area in allReparationAreas)
        {
            if (!area.gameObject.activeSelf) continue;
            area.currentPlayerOnArea.manager.GetPoint(partyManager.baseManager.reparationPoint, transform.position);
        }

        CancelReparation();
        foreach (var area in allReparationAreas)
        {
            area.DeactivateArea();
        }

        isDead = false;
        Heal(totalLife);
        if(securityCollider) StartCoroutine(EnablingSecurityCollider());
    }

    private IEnumerator EnablingSecurityCollider()
    {
        securityCollider.enabled = true;
        yield return new WaitForFixedUpdate();
        securityCollider.enabled = false;
    }

    public void CheckPlayersOnReparationAreas()
    {
        foreach (var area in allReparationAreas)
        {
            if (!area.gameObject.activeSelf) continue;
            if (!area.isPlayerOn) return;
        }

        // Tous les joueurs sont là et prêts pour la réparation !
        foreach (var area in allReparationAreas)
        {
            if (!area.gameObject.activeSelf) continue;
            area.isEveryPlayerOn = true;
        }

        BeginsReparation();
        return;
    }

    public void BeginsReparation()
    {
        // Faire apparaître l'icône de l'input   

        reparationIconData.iconObject.SetActive(true);
        reparationIconData.iconBottom.SetActive(true);

        currentCheckingArea = allReparationAreas[0];
        reparationIconData.iconObject.transform.position = allReparationAreas[0].iconPosition.position;
        allReparationAreas[0].isWaitingForInput = true;
    }

    public void CancelReparation()
    {
        // A tout moment si un joueur quitte la zone de réparation

        foreach (var area in allReparationAreas)
        {
            if (!area.gameObject.activeSelf) continue;
            area.isEveryPlayerOn = false;
        }

        reparationIconData.iconObject.SetActive(false);
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
            } while (nextArea == currentCheckingArea || !nextArea.gameObject.activeSelf);

            lastCheckingArea = currentCheckingArea;
            currentCheckingArea = nextArea;
            iconMoveTimer = 0f;
            isIconMoving = true;
            reparationIconData.iconBottom.SetActive(false);
        }
    }

    public new virtual void Update()
    {
        if (isIconMoving)
        {
            if (iconMoveTimer >= iconMoveDuration)
            {
                reparationIconData.iconObject.transform.position = currentCheckingArea.iconPosition.position;
                currentCheckingArea.isWaitingForInput = true;
                reparationIconData.iconBottom.SetActive(true);
                isIconMoving = false;
            }
            else
            {
                reparationIconData.iconObject.transform.position = Vector3.Lerp(lastCheckingArea.iconPosition.position,
                    currentCheckingArea.iconPosition.position, iconMoveTimer / iconMoveDuration);

                iconMoveTimer += Time.deltaTime;
            }
        }
    }

    #region Trigger & Collision

    #endregion

    #region GUI

    private void InitializeBaseElementInfo()
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

        baseElementInfo.transform.SetParent(partyManager.mainCanvas.transform);
        baseElementInfo.transform.localRotation = Quaternion.identity;
        baseElementInfo.transform.localScale = Vector3.one;
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

    #endregion
}