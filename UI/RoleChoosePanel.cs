using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterChoosePanel : BasePanel
{
    private Transform rolesContent;
    private GameObject characterCellPrefab;
    
    protected override void Awake()
    {
        base.Awake();
        rolesContent = transform.Find("CharacterBar").GetComponent<ScrollRect>().content;
        characterCellPrefab = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/CharacterCell");
    }

    public void AddRole(Role role)
    {
        GameObject obj = Instantiate(characterCellPrefab, rolesContent, false);
        TMP_Text text = obj.transform.Find("Info").GetComponent<TMP_Text>();
        text.text = $"角色昵称：{role.Name}\n" +
                    $"角色职业: {role.Type}\n" +
                    $"角色等级: {role.Level}";
    }
    
    // 如果没有创建过角色中间的角色信息部分隐藏
    // 如果有创建过角色中间的角色信息默认显示上一次登入的角色
}
