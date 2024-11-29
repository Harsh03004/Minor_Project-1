using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance { get; set;}
    [SerializeField] Image Background;
    [SerializeField] TabData[] Tabs;
    int _currIndex;
    TabData _currTab;

    #region Script Initialization

    private void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        _currTab = Tabs[0];
        ChangeTab(0);
    }
    #endregion

    private void Update()
    {
        MyInput();
    }

    void MyInput()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _currIndex > 0)
        {
            ChangeTab(_currIndex - 1);
        }
        if (Input.GetKeyDown(KeyCode.E) && _currIndex < Tabs.Length - 1)
        {
            ChangeTab(_currIndex + 1);
        }
    }

    public void ChangeTab(int index)
    {
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => SkillMarket.instance.CloseMarket());
        if (_currTab != null)s.Append(_currTab.TabButton.DORotate(new Vector3(0,0,45) , .2f).SetEase(Ease.OutBack));
        s.AppendCallback(() => ActivateTab(index));
        s.Append(Tabs[index].TabButton.DORotate(new Vector3(0,0,0) , .2f).SetEase(Ease.OutBack));
    }

    void ActivateTab(int index)
    {
        if(_currTab != null)_currTab.Tab.SetActive(false);
        _currTab = Tabs[index];
        _currIndex = index;
        Background.color = _currTab.BgColor;
        _currTab.Tab.SetActive(true);
    }
}
[System.Serializable]
public class TabData
{
    public string Name;
    public Transform TabButton;
    public GameObject Tab;
    public Color BgColor;
}
