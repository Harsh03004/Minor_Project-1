using System.Collections;
using System;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "Skill" , menuName = "New Skill")]
public class Skill : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public string Info;
    public string AdditionalEffect;
    public int SkillPointToUnlock = 1;
    public int StatForAE = 99;
    [HideInInspector] public SkillButton button;
    public GameObject SkillPrefab;
}

[CustomEditor(typeof(Skill))]
public class SkillEditior : Editor
{
    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        var skill = (Skill)target;

        if(skill == null || skill.Icon == null)
        {
            return null;
        }

        var texture = new Texture2D(width, height);
        EditorUtility.CopySerialized(skill.Icon.texture, texture);

        return texture;
    }
}