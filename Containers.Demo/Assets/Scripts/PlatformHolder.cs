using System;
using System.Collections;
using System.Collections.Generic;
using Containers.Core;
using UnityEngine;

public class PlatformHolder : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _delta;
    [SerializeField] private GameObject _platformPrefab;

    private Placeholder _placeholder;

    private (float Width, float Height) _platformSize;

    public Placeholder Placeholder => _placeholder;

    private void Awake()
    {
        _placeholder = new Placeholder(4, _width * _height);
        
        var platforms = new Platform[_width * _height];
        for (var i = 0; i < platforms.Length; i++)
        {
            platforms[i] = Instantiate(_platformPrefab).GetComponent<Platform>();
            platforms[i].IndexInHolder = i;
            platforms[i].Holder = this;
        }

        var t = platforms[0].GetComponent<Transform>();
        var localScale = t.localScale;
        _platformSize = (localScale.x, localScale.z);
        
        SetPlatformPositions(platforms);
    }

    private void SetPlatformPositions(Platform[] platforms)
    {
        var width = _platformSize.Width * _width + (_width - 1) * _delta;
        var height = _platformSize.Height * _height + (_height - 1) * _delta;

        (float X, float Z) startPosition = (-width / 2, -height / 2);
        var position = startPosition;
        
        for (var i = 0; i < _width; i++)
        {
            position.Z = startPosition.Z;
            
            for (var j = 0; j < _height; j++)
            {
                var transform = platforms[i * _height + j].GetComponent<Transform>();

                transform.localPosition = new Vector3(position.X, 0, position.Z);
                
                position.Z += _platformSize.Height + _delta;
            }

            position.X += _platformSize.Width + _delta;
        }
    }

    public void Place(Container container, Platform platform)
    {
        _placeholder.Place(container.Data, platform.IndexInHolder);
    }
}
