using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemName;
    public Text itemDescription;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetUpTooltip(ItemData_SO item)
    {
        itemName.text = item.itemName;
        itemDescription.text = item.description;
    }

    void OnEnable()
    {
        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Update the position of the tooltip based on the mouse position
    /// </summary>
    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners); // get the corners of the tooltip

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        if (mousePos.y < height)
            rectTransform.position = mousePos + Vector3.up * height * 0.6f;
        else if (Screen.width - mousePos.x < width)
            rectTransform.position = mousePos + Vector3.left * width * 0.6f;
        else
            rectTransform.position = mousePos + Vector3.right * width * 0.6f;
    }
}
