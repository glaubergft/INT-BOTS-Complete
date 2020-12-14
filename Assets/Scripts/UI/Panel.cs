using UnityEngine;
using UnityEngine.UI;

namespace UINavigation
{
    public class Panel : MonoBehaviour
    {
        
        public Selectable defaultButton;

        public delegate void OpenedDelegate();
        public event OpenedDelegate Opened;

        public delegate void ClosedDelegate();
        public event ClosedDelegate Closed;

        public bool KeepOpenedWhenCloseAll = false;

        private bool _isOpened = false;

        public bool IsOpened { get => _isOpened; }

        public static T Find<T>(out Canvas canvas)
        {
            canvas = FindObjectOfType<Canvas>();
            return Find<T>();
        }

        public static T Find<T>()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                return default(T);
            }
            else 
            {
                var panelItem = canvas.GetComponentInChildren<T>(true);
                return panelItem;
            }
            
        }

        public static T Open<T>()
        {
            Canvas canvas;
            var panelItem = Find<T>(out canvas);
            Open(panelItem as Panel, canvas);
			return panelItem;
        }

        public static void Open(Panel panelItem, Canvas canvas = null)
        {
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
            }
            
            CloseAllPanels(canvas);
            
            if (panelItem.defaultButton != null)
            {
                panelItem.defaultButton.Select();
            }
            
            panelItem.gameObject.SetActive(true);

            if (panelItem.Opened != null)
            {
                panelItem.Opened();
            }

            panelItem._isOpened = true;
        }

        public static Panel Close<T>()
        {
            var canvas = FindObjectOfType<Canvas>();
            var panelItem = canvas.GetComponentInChildren<T>(true);
            Close(panelItem as Panel, canvas);
            return panelItem as Panel;
        }

        public static void Close(Panel panelItem, Canvas canvas = null)
        {
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
            }
            
            if (panelItem.Closed != null && panelItem.gameObject.activeSelf)
            {
                panelItem.Closed();
            }

            panelItem.gameObject.SetActive(false);

            panelItem._isOpened = false;
        }
  
        public static void CloseAllPanels()
        {
			var mainPanel = FindObjectOfType<Canvas>();
            CloseAllPanels(mainPanel);
        }

        public static void CloseAllPanels(Canvas canvas)
        {
            var panels = canvas.GetComponentsInChildren<Panel>(true);
            foreach (var panel in panels)
            {
				if (panel != canvas && !panel.KeepOpenedWhenCloseAll)
				{
                    Close(panel, canvas);
				}
            }
        }

        public static bool CheckAllPanelsAreClosed()
        {
            var canvas = FindObjectOfType<Canvas>();
            var panels = canvas.GetComponentsInChildren<Panel>(false);            
            return panels.Length == 0;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                var canvas = FindObjectOfType<Canvas>();
                Panel panelItem = canvas.GetComponentInChildren<Panel>(false);
                if (panelItem && panelItem.defaultButton && panelItem.defaultButton.GetComponent<ButtonHandler>())
                {
                    panelItem.defaultButton.GetComponent<ButtonHandler>().Submit();
                }
            }

        }

    }
}


