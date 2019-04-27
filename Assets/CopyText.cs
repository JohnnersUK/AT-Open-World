using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CopyText : MonoBehaviour
{
    private Text me;
    [SerializeField] private GameObject master;

    private void Start()
    {
        me = GetComponent<Text>();
    }

    void Update ()
    {
        me.text = master.GetComponent<Text>().text;
	}
}
