using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TabManager : MonoBehaviour
{
    public TabButton[] tabButtons;

    private int currentTabIndex = 0;

    // public InputAction nextTabAction;
    // public InputAction previousTabAction;

    // private void OnEnable()
    // {
    //     nextTabAction.Enable();
    //     previousTabAction.Enable();
    // 
    //     nextTabAction.performed += _ => SwitchToNextTab();
    //     previousTabAction.performed += _ => SwitchToPreviousTab();
    // }
    // 
    // private void OnDisable()
    // {
    //     nextTabAction.performed -= _ => SwitchToNextTab();
    //     previousTabAction.performed -= _ => SwitchToPreviousTab();
    // 
    //     nextTabAction.Disable();
    //     previousTabAction.Disable();
    // }

    private void Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            Button btn = tabButtons[i].GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => SwitchTab(index));
        }

        SwitchTab(currentTabIndex);
    }

    public void SwitchTab(int index)
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].SetActive(i == index);
        }

        currentTabIndex = index;

        // Focus un élément dans l'onglet
        // EventSystem.current.SetSelectedGameObject
    }

    // private void SwitchToNextTab()
    // {
    //     int nextIndex = (currentTabIndex + 1) % tabButtons.Length;
    //     SwitchTab(nextIndex);
    // }
    // 
    // private void SwitchToPreviousTab()
    // {
    //     int prevIndex = (currentTabIndex - 1 + tabButtons.Length) % tabButtons.Length;
    //     SwitchTab(prevIndex);
    // }
}
