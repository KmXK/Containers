using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Van : MonoBehaviour
{
    [SerializeField] private ContainerPlatform _platform;
    
    private TextMeshPro[] _smallTexts;
    private TextMeshPro _largeText;

    private void Awake()
    {
        var canvas = transform.GetChild(0).GetComponentInChildren<Canvas>().transform;
        var texts = canvas.GetComponentsInChildren<TextMeshPro>();
        _smallTexts = texts[..2];
        _largeText = texts[2];
    }

    public ContainerPlatform Platform => _platform;

    public void SetSmallTexts(params string[] ids)
    {
        _smallTexts[0].text = ids[0];
        _smallTexts[1].text = ids[1];
        
        foreach (var smallText in _smallTexts)
        {
            smallText.gameObject.SetActive(true);
        }
    }

    public void SetLargeText(string id)
    {
        _largeText.text = id;
        _largeText.gameObject.SetActive(true);
    }
}
