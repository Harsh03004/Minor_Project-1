using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SkillMarket : MonoBehaviour
{
    public static SkillMarket instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI TitleText;
    [SerializeField] Image IconImg;
    [SerializeField] TextMeshProUGUI DetailText;
    [SerializeField] Slider UnlockBar;
    [SerializeField] TextMeshProUGUI InputText;
    [SerializeField] TextMeshProUGUI RequienmentText;
    [SerializeField] TextMeshProUGUI AdditionalEffectText;
    [SerializeField] TextMeshProUGUI AdditionalRequienmentText;
    [SerializeField] Transform SkillSelector;

    private bool CanUnlock;
    private bool IsMarketActive;
    private SkillButton _button;
    private Skill skillinfo;

    private float UnlockTime = 0;

    private playerattack playerAttackScript; // Reference to playerattack for skill points

    #region Market Initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Get the playerattack script
        playerAttackScript = FindObjectOfType<playerattack>();
    }
    #endregion

    #region Public Methods
    public void OpenMarket(Skill skillinfo)
    {
        if (_button != skillinfo.button)
        {
            this.skillinfo = skillinfo;

            Sequence s = DOTween.Sequence();
            s.Append(transform.DOLocalMoveX(1220, .2f).SetEase(Ease.InBack));
            s.Join(SkillSelector.DOMove(skillinfo.button.transform.position, .2f).SetEase(Ease.OutBack));
            s.AppendCallback(() => SetMarket());
            s.Append(transform.DOLocalMoveX(710, .2f).SetEase(Ease.OutBack));
        }
    }

    public void CloseMarket()
    {
        IsMarketActive = false;
        _button = null;

        transform.DOLocalMoveX(1220, .2f).SetEase(Ease.InBack);
        SkillSelector.DOLocalMove(new Vector3(-1100, 115, 0), .2f).SetEase(Ease.OutBack);
    }

    private void SetMarket()
    {
        TitleText.text = skillinfo.Name;
        IconImg.sprite = skillinfo.Icon;
        DetailText.text = skillinfo.Info;
        AdditionalEffectText.text = skillinfo.AdditionalEffect;

        int skillPoints = playerAttackScript?.skillPoints ?? 0;

        RequienmentText.SetText($"{skillPoints} / {skillinfo.SkillPointToUnlock}");
        AdditionalRequienmentText.SetText($"{EconomyManager.instance.coin} / {skillinfo.StatForAE}");

        _button = skillinfo.button;

        if (CanUnlock = (!_button.IsUnlocked && skillPoints >= skillinfo.SkillPointToUnlock))
        {
            UnlockBar.value = UnlockTime = 0;

            InputText.transform.parent.localPosition = new Vector3(-75, 0, 0);
            RequienmentText.transform.parent.gameObject.SetActive(true);
            UnlockBar.gameObject.SetActive(true);
            InputText.color = Color.white;
            InputText.text = "HOLD ( H )";
            IsMarketActive = true;
        }
        else
        {
            UnlockBar.gameObject.SetActive(false);
            RequienmentText.transform.parent.gameObject.SetActive(false);
            InputText.transform.parent.localPosition = new Vector3(0, 0, 0);
            InputText.color = _button.IsUnlocked ? Color.white : Color.red;

            InputText.text = _button.IsUnlocked ? "PURCHASED" : "NOT ENOUGH SKILL POINTS";
            IsMarketActive = false;
        }
    }
    #endregion

    #region Market Update
    private void FixedUpdate()
    {
        if (IsMarketActive && !_button.IsUnlocked && Input.GetKey(KeyCode.H))
        {
            if (UnlockTime < 1)
            {
                UnlockTime += Time.deltaTime;
                UnlockBar.value = UnlockTime;
            }
            else
            {
                int skillPoints = playerAttackScript?.skillPoints ?? 0;

                if (skillPoints >= skillinfo.SkillPointToUnlock)
                {
                    // Deduct skill points and reflect the change
                    playerAttackScript.skillPoints -= skillinfo.SkillPointToUnlock;

                    // Update the skill points display in the EconomyManager
                    EconomyManager.instance.UpdateXPText(playerAttackScript.skillPoints);

                    // Unlock the skill
                    _button.UnlockSkill();

                    // Refresh the market UI
                    SetMarket();
                }
                else
                {
                    Debug.LogError("Not enough skill points to unlock the skill!");
                }
            }
        }
        else if (UnlockTime > 0)
        {
            UnlockTime -= Time.deltaTime;
            UnlockBar.value = UnlockTime;
        }
    }
    #endregion
}
