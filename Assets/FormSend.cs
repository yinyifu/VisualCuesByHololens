using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using UnityEngine.SceneManagement;

public class FormSend : MonoBehaviour {
    public GameObject toggleObj;
    public GameObject intensityObj;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void sendForm(string straightOrCurve) {
        Toggle m_Toggle = toggleObj.GetComponent<Toggle>();
        bool isOn1 = m_Toggle.isOn;
        Slider m_slide = intensityObj.GetComponent<Slider>();
        double value = m_slide.value;

        StaticClass.straightOrCurve = straightOrCurve;
        StaticClass.haveObstacle = isOn1;
        StaticClass.obstacleIntensity = value;

        SceneManager.LoadScene(sceneName: "VisualCue");
    }
}
