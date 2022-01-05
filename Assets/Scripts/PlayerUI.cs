using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public GameObject healthIconPrefab;
    public GameObject rallyIconPrefab;
    public GameObject healthContainer;
    public float healthLeftPadding = 50;
    public GameObject breakEffect;

    public GameObject pauseIcon;

    private List<GameObject> healthIcons;
    private List<GameObject> rallyIcons;
    private List<bool> activeHearts;

    private void Awake()
    {
        healthIcons = new List<GameObject>();
        rallyIcons = new List<GameObject>();
        activeHearts = new List<bool>();
    }

    public void Pause(bool pause)
    {
        pauseIcon.SetActive(pause);
    }

    public void UpdateHealth(int health, int rally)
    {
        int healthCount = healthIcons.Count;
        bool playBreakAnim = false;
        for (int i = healthCount; i < health; i++)
        {
            healthIcons.Add(Instantiate(healthIconPrefab, healthContainer.transform));
            activeHearts.Add(false);
        }
        int rallyCount = rallyIcons.Count;
        for (int i = rallyCount; i < rally; i++)
        {
            rallyIcons.Add(Instantiate(rallyIconPrefab, healthContainer.transform));
        }
        for (int i = 0; i < healthIcons.Count; i++)
        {
            healthIcons[i].SetActive(false);
        }
        for (int i = 0; i < rallyIcons.Count; i++)
        {
            rallyIcons[i].SetActive(false);
        }
        float currentPosition = healthLeftPadding;
        for (int i = 0; i < health; i++)
        {
            healthIcons[i].SetActive(true);
            var icon = healthIcons[i].GetComponent<RectTransform>();
            var position = icon.anchoredPosition;
            position.x = currentPosition;
            icon.anchoredPosition = position;
            currentPosition += icon.rect.width + healthLeftPadding;
        }
        for (int i = 0; i < healthIcons.Count; i++)
        {
            if (!healthIcons[i].activeSelf && activeHearts[i])
            {
                playBreakAnim = true;
                breakEffect.GetComponent<RectTransform>().position = healthIcons[i].GetComponent<RectTransform>().position;
            }
        }
        if (playBreakAnim)
        {
            breakEffect.GetComponent<Animator>().Play("Break");
        }
        for (int i = 0; i < rally; i++)
        {
            rallyIcons[i].SetActive(true);
            var icon = rallyIcons[i].GetComponent<RectTransform>();
            var position = icon.anchoredPosition;
            position.x = currentPosition;
            icon.anchoredPosition = position;
            currentPosition += icon.rect.width + healthLeftPadding;
        }
        for (int i = 0; i < healthIcons.Count; i++)
        {
            activeHearts[i] = healthIcons[i].activeSelf;
        }
    }
}
