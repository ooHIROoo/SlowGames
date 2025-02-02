﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{    

    //チュートリアル
    [SerializeField]
    bool _isTutorial = false;



    [SerializeField]
    GameObject _deathEffect;

    [SerializeField]
    private EnemyType _type = EnemyType.Easy;
    public EnemyType Type { get {return _type;}}

    //Transform _targetPostion;
    //多重Hitを避ける
    bool death = false;

    [System.Serializable]
    public struct EnemyAttackInfo
    {
        public float moveSpeed;     //移動速度,移動時間,
        public float stayTimeMax;   //待機時間
        public float sideMoveRange; //横移動の幅
        public float activeTimeMax;    //行動数の限界.
        public int   shotFrequency;      //何回にどのくらい撃つかの頻度.
        public int   chamberValue;       //何発連続で撃つか
        public float shotDelay;          ////連続で撃つ時の遅延時間

        public float generateRate; //出現率
    }

    [SerializeField]
    List<EnemyAttackInfo> _enemyAttackInfos;
    EnemyAttackInfo _enemyInfo;
    public EnemyAttackInfo info{ get { return _enemyInfo;} }

    public float _activeCounter;    //行動中(攻撃前)のカウントをする用

    public TargetPosition  _generatePostion;

    bool _doFall = false;

    [SerializeField]
    int _attackDamege = 2;


    public bool doFall
    {
        set{ _doFall = value; }
        get{ return _doFall;}
    }

    void Start()
    {
        death = false;

        int waveCount = GenerateManager.GetCurrentWave();

        //それ以上のデータがない場合,最大の設定を入れる
        if (waveCount >= _enemyAttackInfos.Count)
        {
            waveCount = _enemyAttackInfos.Count - 1;
        }

        _enemyInfo = _enemyAttackInfos[waveCount];

        //空中から落とすタイプは落ちる処理をする
        if ((int)_generatePostion > (int)TargetPosition.Right)
        {
            _doFall = true;
        }

    }



    //弾にあたったら死
    void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.tag == TagName.Bullet)
        {  
            //意図せず二回たまにあたってしまった場合を避ける
            if (death)
            {
                return;
            }
    
            death = true;
       
            //エフェクト
            var effect = Instantiate(_deathEffect);
            effect.transform.position = transform.position;

            if (_isTutorial)
            {

            }
            else
            {
                AjustDeath();
            }
            Destroy(this.gameObject); 
           
        }
        else if( other.gameObject.tag == TagName.Player)
        {
            if(_type == EnemyType.Tackle)
            {
                //test:
                var playerHp =  FindObjectOfType<PlayerHP>();
                playerHp.Damage(_attackDamege);
                playerHp.BarrierEffectCreate(transform.position);
                AjustDeath();
            }

            Destroy(this.gameObject); 
        }
    }

    void TutorialDeath()
    {
        //チュートリアル
    }

    //死ぬ直前処理
    void AjustDeath()
    {
            //死ぬ
            FindObjectOfType<GenerateManager>().AddDeathCount(_generatePostion);
            AudioManager.instance.stop3DSe(gameObject,AudioName.SeName.Flying);
            AudioManager.instance.play3DSe(gameObject,AudioName.SeName.EnemyExplotion);
            //Test:スコア
//          ScoreManager.instance.AddHitEnemyCount();
//          ScoreManager.instance.AddScore(_type);
    }


    //スコアを更新せず静かに殺します 
    public void SilentDestroy()
    {
        FindObjectOfType<GenerateManager>().AddDeathCount(_generatePostion);
        Destroy(this.gameObject);
    }


//    //待機時間
//    [SerializeField,Range(0,5)]
//    public float _stayTimeMax = 1.0f;
//
//    //横移動の幅
//    [SerializeField,Range(0,10)]
//    public float _sideMoveRange = 8.0f;
//
//    //行動中のカウントをする用
//    public float _activeCounter = 0;
//    [SerializeField,Range(0,5)]
//    public float _activeTimeMax = 2;
//
//    //何回にどのくらい撃つかの頻度.
//    public int _shotFrequency = 3;
//
//    //何発連続で撃つか
//    public int _chamberValue = 1;
// 
//    //連続で撃つ時の遅延時間
//    [SerializeField]
//    public float _shotDelay = 1.0f;


    //移動速度,移動時間,
 //   public float _moveSpeed   = 3.0f;

  
}
