 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<Tabbutton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public List<GameObject> objectsToSwap;

    public Tabbutton selectedTab;

    void Start()
    {
        if(tabButtons != null && tabButtons.Count > 0)
        {
            selectedTab = tabButtons[0];
            selectedTab.background.sprite = tabActive;
        }
    }
    public void Subscribe(Tabbutton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<Tabbutton>();
        }
        tabButtons.Add(button);
    }

    public void OnTabEnter(Tabbutton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(Tabbutton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(Tabbutton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for(int i=0; i<objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (Tabbutton button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab) continue;
            button.background.sprite = tabIdle;
        }
    }
}
