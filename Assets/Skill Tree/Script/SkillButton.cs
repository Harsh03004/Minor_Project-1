using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillButton : MonoBehaviour
{
    [Header("Button Data")]
    public Skill SkillData;
    public int SkillActivationRequiredCount = 0;
    public int RequiredLevel = 0;
    [HideInInspector] public bool IsUnlocked;

    [Header("Reference")]
    [SerializeField] GameObject Lock;
    public LinkedSkillData[] LinkedSkills;
    [SerializeField] GameObject[] UnlockIndicator;

    [Header("Player Reference")]
    public playerattack playerAttackInstance;  // Reference to the playerattack script

    #region Script Initialization
    void Start()
    {
        InitiateSkill();
    }

    private void OnDisable()
    {
    }
    void InitiateSkill()
    {
        SkillData.button = this;
        GetComponent<Image>().sprite = SkillData.Icon;
        CheckRequirnmentToUnclock();
    }
    #endregion

    #region Public Methods
    public void CheckRequirnmentToUnclock()
    {
        if (CanUnlock() && !IsUnlocked)
        {
            Lock.SetActive(false);
            foreach (GameObject Obj in UnlockIndicator)
            {
                Obj.SetActive(true);
            }
        }
    }

    public void CallSkillMarket()
    {
        SkillMarket.instance.OpenMarket(SkillData);
    }

    public void UnlockSkill()
    {
        if (CanUnlock())
        {
            IsUnlocked = true;

            // Unlocking the skill and visual effects
            GetComponent<Image>().color = Color.white;
            Debug.Log("Activated  " + transform.name);

            foreach (GameObject Obj in UnlockIndicator)
            {
                Obj.SetActive(false);
            }
            foreach (LinkedSkillData line in LinkedSkills)
            {
                if (line != null)
                {
                    line.Connection.startColor = Color.white;
                    line.Connection.endColor = Color.white;
                }
            }

            DoActivationActions();

            foreach (LinkedSkillData skill in LinkedSkills)
            {
                skill.LinkedSkill.CheckRequirnmentToUnclock();
            }

        }
    }

    void DoActivationActions()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOScale(1.4f, .1f).SetEase(Ease.InBack));
        s.Append(transform.DOScale(1, .2f).SetEase(Ease.OutBack));
    }
    #endregion

    #region Calculations
    public bool CanUnlock()
    {
        int count = 0;
        foreach (LinkedSkillData skill in LinkedSkills)
        {
            if (skill.LinkedSkill.IsUnlocked)
                count++;
        }

        return SkillActivationRequiredCount <= count;
    }
    #endregion

    #region Editor Methods
    private void OnValidate()
    {
        if (SkillData.Icon != null) this.GetComponent<Image>().sprite = SkillData.Icon;

        if (SkillActivationRequiredCount > LinkedSkills.Length)
        {
            SkillActivationRequiredCount = LinkedSkills.Length;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
        for (int i = 0; i < LinkedSkills.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, LinkedSkills[i].LinkedSkill.transform.position);
        }
    }
    #endregion
}

[System.Serializable]
public class LinkedSkillData
{
    public SkillButton LinkedSkill;
    public LineRenderer Connection;
}