﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserAuthenticationText : MonoBehaviour {

    public enum TextType
    {
        point,
        Descript,
        Clear
    }

    [SerializeField]
    private TextType _type = TextType.Descript;

    private Text _text = null;

    private float _time = 0.0f;
    private float _testTime = 0.5f;

    void Start()
    {
        _text = GetComponent<Text>();
        if(_type == TextType.Descript)
        {
            _text.text = "id access";
        }
        else if(_type == TextType.Clear)
        {
            _text.text = "complete";
        }
        else if(_type == TextType.point)
        {
            _text.text = ".";
        }
    }

    void Update()
    {
        if (_type != TextType.point) return;
        _time += Time.deltaTime;
        if(_time > _testTime)
        {
            _testTime += 0.5f;
            _text.text += ".";
        }
        if(_testTime > 2.5f)
        {
            _testTime = 0.5f;
            _text.text = ".";
            _time = 0.0f;

        }
    }


}
