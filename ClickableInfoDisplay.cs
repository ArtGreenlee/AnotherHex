using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ClickableInfoDisplay : MonoBehaviour
{
    public static ClickableInfoDisplay instance;
    public TextMeshProUGUI clickableNameText;
    public TextMeshProUGUI magnitudeText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI speedText;
    public Image clickableImage;
    private Clickable clickableDisplayed;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void displayClickable(Clickable clickable)
    {
        if (!TryGetComponent<Entity>(out Entity entity))
        {
            return;
        }
        gameObject.SetActive(true);
        clickableDisplayed = clickable;
        clickableNameText.text = entity.name;
        infoText.text = entity.info;
        /*if (entity.magnitudeMin == entity.magnitudeMax)
        {
            magnitudeText.text = "Magnitude: " + (entity.magnitudeMin * entity.magnitudeScaling).ToString();
        }
        else
        {
            magnitudeText.text = "Magnitude: " + (entity.magnitudeMin * entity.magnitudeScaling).ToString() + " - " + (entity.magnitudeMax * entity.magnitudeScaling).ToString();
        }   */
        rangeText.text = "Range: " + entity.getRange().ToString();
        speedText.text = "Cooldown: " + entity.getSpeed().ToString();
    }

    private void Update()
    {
        if (clickableDisplayed != null)
        {
            displayClickable(clickableDisplayed);
        }
    }

    public void stopDisplayingClickable()
    {
        clickableDisplayed = null;
        gameObject.SetActive(false);
    }
}
