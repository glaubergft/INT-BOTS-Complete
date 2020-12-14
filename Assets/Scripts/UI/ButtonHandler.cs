using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UINavigation;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, IPointerDownHandler
{
    public AudioSource clickSound;

    [SerializeField]
    private Panel panelToOpen;

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Selectable>().OnPointerExit(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.selectedObject.GetComponent<Button>() != null)
        {
            GetComponent<Button>().onClick.Invoke();
            Input.ResetInputAxes(); //Send the input back to 0 for one frame.
        }

        if (clickSound != null)
        {
            clickSound.Play();
        }

        if (panelToOpen && !panelToOpen.gameObject.activeSelf)
        {
            Submit();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Selectable>().Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<EventSystem>()?.SetSelectedGameObject(null);
    }

    public void Submit()
    {
        if (panelToOpen == null)
        {
            var parentPanel = gameObject.FindParentComponent<Panel>();
            Panel.Close(parentPanel);
        }
        else
        {
            Panel.Open(panelToOpen);
        }
    }



}
