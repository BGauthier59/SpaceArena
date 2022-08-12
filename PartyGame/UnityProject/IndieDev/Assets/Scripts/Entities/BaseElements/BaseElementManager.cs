using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseElementManager : Entity
{
    [Header("Reparation")] [SerializeField]
    private int reparationInputs;

    private int reparationInputsCounter;
    [SerializeField] private ReparationArea[] allReparationAreas;
    [SerializeField] private GameObject reparationIcon;

    private ReparationArea lastCheckingArea;
    private ReparationArea currentCheckingArea;

    private bool isIconMoving;
    [SerializeField] private float iconMoveDuration;
    private float iconMoveTimer;
    
    public Material reparationAreaDeviceEnabled;

    #region Entity

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
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
    }

    #endregion

    public override void Start()
    {
        base.Start();

        BaseManager.instance.allBaseElements.Add(this);
        foreach (var area in allReparationAreas)
        {
            area.associatedElement = this;
        }
    }

    public virtual void OnDestroyed()
    {
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

        reparationInputsCounter = 0;
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
}