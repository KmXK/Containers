using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Van : MonoBehaviour
{
    [SerializeField] private ContainerPlatform _platform;
    
    private TextMeshProUGUI[] _smallTexts;
    private TextMeshProUGUI _largeText;

    private void Awake()
    {
        var canvas = transform.GetChild(0).GetComponentInChildren<Canvas>().transform;
        var texts = canvas.GetComponentsInChildren<TextMeshProUGUI>(); 
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
        _largeText.gameObject.SetActive(false);
    }

    public void SetLargeText(string id)
    {
        _largeText.text = id;

        foreach (var t in _smallTexts) t.gameObject.SetActive(false);
        _largeText.gameObject.SetActive(true);
    }

    public void DisableText()
    {
        foreach (var t in _smallTexts) t.gameObject.SetActive(false);
        _largeText.gameObject.SetActive(false);
    }

    public void DisableSmallText(int index)
    {
        if(index >= 0 && index < _smallTexts.Length)
            _smallTexts[index].gameObject.SetActive(false);
    }
}
