using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    ItemUI currentItemUI;

    void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        QuestUI.Instance.itemTooltip.gameObject.SetActive(true);
        QuestUI.Instance.itemTooltip.GetComponent<ItemTooltip>().SetUpTooltip(currentItemUI.currentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.itemTooltip.gameObject.SetActive(false);
    }
}
