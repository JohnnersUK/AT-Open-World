using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    Image bar;
	// Use this for initialization
	void Start ()
    {
        bar = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        bar.fillAmount = GameManager.Instance.PlayerHealth / 10;
	}
}
