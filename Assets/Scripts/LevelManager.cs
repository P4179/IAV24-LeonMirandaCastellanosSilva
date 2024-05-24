using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Ciclo de dia
    private int days;
    [SerializeField]
    private float initTime = 6.0f;

    private float prevTime = 0.0f,
                  timeOfDay = 0.0f,
                  hour = 0.0f;

    [SerializeField]
    private float speedFactor = 2.0f;

    [SerializeField]
    private Gradient lightGradient;

    private Light lightSource;
    private Transform lightTr;

    [SerializeField]
    private TextMeshProUGUI dayText;

    // Start is called before the first frame update
    void Start()
    {
        prevTime = timeOfDay = hour = initTime;
        
        days = 1;

        lightSource = GameObject.Find("Light").GetComponent<Light>();
        lightTr = lightSource.transform;
        
        dayText = GameObject.Find("DayText").GetComponent<TextMeshProUGUI>();
        dayText.text = "Day " + days;

        changeLight(hour / 24.0f);
    }

    // Update is called once per frame
    void Update()
    {
        hour += Time.deltaTime * speedFactor;
        timeOfDay += Time.deltaTime * speedFactor;

        if (timeOfDay - prevTime >= 24 / speedFactor) {
            days++;
            dayText.text = "Day " + days;
            timeOfDay = prevTime = initTime;
        }

        hour %= (24 / speedFactor);
        changeLight(hour / (24.0f / speedFactor));
    }

    // Cambia el color de la luz dependiendo de la hora del dia.
    private void changeLight(float timePercent)
    {
        lightSource.color = lightGradient.Evaluate(timePercent);
        lightTr.rotation = Quaternion.Euler(new Vector3((timePercent * 360.0f) - 90.0f, 90.0f, 0.0f));
    }
    public int getDays() { return days; }
}
