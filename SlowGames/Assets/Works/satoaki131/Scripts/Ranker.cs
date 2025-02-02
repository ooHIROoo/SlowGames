﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranker : MonoBehaviour {

    private int _lastScore = 0; //最終スコア

    [SerializeField]
    private Text _text = null;

    [System.Serializable]
    public struct RankData
    {
        public int score;
        public string rankName;
        public Color textColor;
    }

    [SerializeField, Tooltip("ランクに合わせたスコアを入れる")]
    private RankData[] _data;

    void Start()
    {
        _lastScore = ScoreManager.instance.getScore(); //最後はコメント外す
        for(int i = 0; i < _data.Length; i++)
        {
            if(_lastScore > _data[i].score) { continue; }
            _text.text =  _data[i].rankName;
            _text.color = _data[i].textColor;
            break;
        }
        //最高スコアより大きかった時の処理
        if(_lastScore > _data[_data.Length - 1].score)
        {
            _text.text = _data[_data.Length - 1].rankName;
            _text.color = _data[_data.Length - 1].textColor;
        }
    }

    void Update()
    {

    }
}
