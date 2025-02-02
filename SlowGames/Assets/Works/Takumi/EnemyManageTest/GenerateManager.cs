﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//波状攻撃のデータ構造体です
[System.Serializable]
public struct WaveData
{
    //敵の死亡数がこの値以上の場合以下の情報で敵を出現させます.
    public int _startDieCount;

    public int _generateTimingCount;     //シーン内にいるエネミーが残り何体再度出現させるか
    public int _generateCount;           //何体ずつだすか
    public int _enemyLimit;                  //同時出現数の限界値
    public List<EnemyType> _generateTypeList;//どのタイプを出すか
    public List<RareEnemy> _rareEnemyInfo;
   

    public WaveData(int startDieCount,
                    int generateTimingCount,int generateCount,
                    int enemyLimit,List<EnemyType> generateTypeList,
                    List<RareEnemy> rareEnemy)
    {

       _startDieCount = startDieCount;
       
       _generateTimingCount =  generateTimingCount;
       _generateCount       =  generateCount;
       _generateTypeList    =  new List<EnemyType>();
       _enemyLimit          =  enemyLimit;
       _generateTypeList    =  generateTypeList;
       _rareEnemyInfo       =  new List<RareEnemy>();
       _rareEnemyInfo       =  rareEnemy;
       
    }

}

[System.Serializable]
public struct RareEnemy
{
    public EnemyType type;
    public int       generateTiming;
}


public class GenerateManager : MonoBehaviour
{

    EnemyGenerator _enemyGenerator;

    //生成した数を管理
    int[] _currentEnemysCount = new int[((int)TargetPosition.Last)];
   
    //同じ場所に何体まで出せるか(全体数の限界値ではありません)
    [SerializeField,Range(1,20)]
    int _enemyLimit = 1;

    [SerializeField]
    List<WaveData> _waveDate = new List<WaveData>();
    List<int> _rareEnemyCount = new List<int>();
    static int _currentWaveCount = 0;
    [SerializeField]
    int _MAX_ENEMY = 30;
    int _enemyNumber = 0; //次が何番目の生成かを取得.

    /// <summary>
    /// 
    /// </summary>
    /// <returns>int型で現在のウェーブ数が帰ってきます。(※０始まりです。)</returns>
    public static int GetCurrentWave()
    {
         return _currentWaveCount;
    }





    //死亡数をカウントします
    public int _deathCount = 0;
    


    void Start()
    {
        _genePos = new List<TargetPosition>();
        _genePos.Add(TargetPosition.Left);
        _genePos.Add(TargetPosition.Right);
        _genePos.Add(TargetPosition.Front);
        _genePos.Add(TargetPosition.UpLeft);
        _genePos.Add(TargetPosition.UpRight);
        _genePos.Add(TargetPosition.UpFront);

        //初期化
        _enemyGenerator = this.gameObject.GetComponent<EnemyGenerator>();
        if (GameDirector.instance != null)
        {
            _MAX_ENEMY = GameDirector.instance.clearEnemyKillCount;
        }

        for (int i = 0; i < _waveDate.Count; i++)
        {
            _rareEnemyCount.Add(0);
        }

        _deathCount = 0;

        //開幕３体配置.
        _currentWaveCount = 0;
//      var waveData = _waveDate[_currentWaveCount];
//      SetEnemy(1,waveData._generateTypeList);

        if (_isTutorial)
        {
            TutorialSet();
        }
    }

    [SerializeField]
    bool _isTutorial = false;

    //チュートリアル
    public bool isTutorial
    {
        set
        {
            _isTutorial = value;
            if (value)
            {
                TutorialSet();     
            }
                
        }

        get{ return _isTutorial;}

    }


    List<TargetPosition> _genePos;// = new List<TargetPosition>();

    void Awake()
    {
        

    }

    public void TutorialSet()
    {

//        //チュートリアル用の三体を生成
//        for (int i = 0; i < _genePos.Count; i++)
//        {
//            DefaultSetEnemy(EnemyType.Easy,_genePos[i]);
//            _enemyNumber += 1;
//        }

        StartCoroutine(Set());

    }

    IEnumerator Set()
    {
        //チュートリアル用の三体を生成
        for (int i = 0; i < _genePos.Count; i++)
        {
            DefaultSetEnemy(EnemyType.Easy,_genePos[i]);
            _enemyNumber += 1;
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;


    }


    public void GameStartSet()
    {
//        List<TargetPosition> genePos = new List<TargetPosition>();
//
//        genePos.Add(TargetPosition.Left);
//        genePos.Add(TargetPosition.Right);
//        genePos.Add(TargetPosition.UpFront);
//
//        //ゲームスタート用の三体を生成
//        for (int i = 0; i < genePos.Count; i++)
//        {
//            DefaultSetEnemy(EnemyType.Easy,genePos[i]);
//        }

    }


    //固定位置に生成
    void DefaultSetEnemy(EnemyType enemyType = EnemyType.Easy,TargetPosition targetPosition = TargetPosition.Left)
    {
            //生成した場所のカウントを覚えておく
            _currentEnemysCount[(int)targetPosition] += 1;
            
            //生成
            _enemyGenerator.GenerateEnemy(enemyType, targetPosition);    
                         
    }

    void Update()
    {

        //test: ”G”Keyで生成
        if (Input.GetKeyDown(KeyCode.G))
        {

            for (int i = 0; i < 3; i++)
            {
                DefaultSetEnemy(EnemyType.Easy,TargetPosition.UpLeft);
            }
            //SetEnemy();
//     
//            string debugCount = "";
//            foreach (var count in _currentEnemysCount)
//            {
//                debugCount += count;
//            }
//            Debug.Log(debugCount);

        }
       
    }

    //死んだ回数を記憶
    public void AddDeathCount(TargetPosition generatePosition)
    {
        //死んだ数を死んだを更新
        _deathCount += 1;
        _currentEnemysCount[(int)generatePosition] -= 1;
        UpdateEnemyCount();
    }



    //生きてるエネミーの総数を取得
    public int GetLiveEnemyCount()
    {
        int currentEnemyCount = 0;
        string sousu = "";

        foreach (var count in _currentEnemysCount)
        {
                 currentEnemyCount += count;
                 sousu += count.ToString();
        }
       //  Debug.Log(sousu);
         return currentEnemyCount;
    }


//    //敵キャラを生成
//    void UpdateEnemyCount()
//    {
//
//        //ウェーブデータを感じた.
//        var waveData = _waveDate[_currentWaveCount];
//
//        //ウェーブのデータが最大にいったらそれ以上はいかない
//        if (_currentWaveCount < _waveDate.Count - 1)
//        {
//
//            //敵の死亡数が一定数行っていたらウェーブを更新
//            if (_deathCount > _waveDate[(_currentWaveCount + 1)]._startDieCount)
//            {
//                _currentWaveCount += 1;
//            }
//
//        }
//
//        //生成しようとする数
//        int GenerateCount = waveData._generateCount;
//
//
//
//        //指定した的キャラの出現
//        //最大数でてたら通らない
//        int rareEnemyCount = _rareEnemyCount[_currentWaveCount];
//        if (waveData._rareEnemyInfo.Count > rareEnemyCount)
//        {
//            //タイミングに合わせて、ホーミングタイプのキャラを出す
//            if (_deathCount >= waveData._rareEnemyInfo[rareEnemyCount].generateTiming)
//            {
//                //指定したエネミータイプを生成 
//                //更新
//                SetEnemy(1, waveData._rareEnemyInfo[rareEnemyCount].type);
//                _rareEnemyCount[_currentWaveCount] += 1;
//            }
//        }
//
//        //死ぬごとに、敵キャラを生成
//        int liveEnemysCount = GetLiveEnemyCount(); 
//
//        //現在生き残ってる数が新たに敵を出現させるタイミングかをチェック.
//        if (liveEnemysCount <= waveData._generateTimingCount)
//        {
//            //限界値以上はださない
//            if (_MAX_ENEMY <= _deathCount + liveEnemysCount)
//            {
//                return;
//            }
//           
//            //限界値以上出さない
//            if (_MAX_ENEMY < _deathCount + liveEnemysCount + waveData._generateCount)
//            { 
//                
//                int lastCount = _MAX_ENEMY - (_deathCount + liveEnemysCount);
//                //Debug.Log("max_enmy:" + _MAX_ENEMY + " _deathocount:" +_deathCount +" liveEnemyCount :"+ liveEnemysCount);
//                SetEnemy(lastCount, waveData._generateTypeList);
//            }
//            else 
//            {
//                //設定した分のエネミーを出す
//                SetEnemy(waveData._generateCount, waveData._generateTypeList);
//            }
//        }
//
//    }


    //敵キャラを生成
    void UpdateEnemyCount()
    {
        
        //ウェーブのデータの更新チェック.
        if (_currentWaveCount < _waveDate.Count - 1)
        {

            //敵の死亡数が一定数行っていたらウェーブを更新
            if (_deathCount > _waveDate[(_currentWaveCount + 1)]._startDieCount)
            {
                _currentWaveCount += 1;
            }

        }
     

        //現在のウェーブデータに合わせた生成情報を取得.
        var waveData = _waveDate[_currentWaveCount];
        //現在ステージ常に敵キャラ.
        int liveEnemysCount = GetLiveEnemyCount();


        if (waveData._generateTimingCount < liveEnemysCount)
            return;

        //限界値以上はださない
        if (_MAX_ENEMY <= _deathCount + liveEnemysCount)
        {
            return;
        }


        //生成する数
        int GenerateCount = waveData._generateCount;

        //最後の数を調整.
        if (_MAX_ENEMY < _deathCount + liveEnemysCount + waveData._generateCount)
        { 
            GenerateCount = _MAX_ENEMY - (_deathCount + liveEnemysCount);
        }

        //生成.
        for (int i = 0; i < GenerateCount; i++)
        {
            //生成番号を更新.
            _enemyNumber += 1;
           
            //指定した的キャラの出現
            //最大数でてたら通らない

            int rareEnemyCount = _rareEnemyCount[_currentWaveCount];


            //レアキャラ生成チェック
            //それ以外は通常生成
            if (_enemyNumber == waveData._rareEnemyInfo[rareEnemyCount].generateTiming)
            {
                
                SetEnemy(1, waveData._rareEnemyInfo[rareEnemyCount].type);
               

                if (waveData._rareEnemyInfo.Count - 1 == rareEnemyCount)
                {

                }
                else
                {
                    _rareEnemyCount[_currentWaveCount] += 1;
                }
            }
            else
            {
                SetEnemy(1, waveData._generateTypeList);
                
            }

            if (_currentWaveCount < _waveDate.Count - 1)
            {
                //生成中に、ウェーブ更新をチェック.
                if (_enemyNumber >= _waveDate[(_currentWaveCount + 1)]._startDieCount)
                {
                
                    _currentWaveCount += 1;
                    waveData = _waveDate[_currentWaveCount];
                }
            }

       }

  

    }



//    //敵を生成する場所の数、　またそこから出す敵のカウントを設定、敵キャラを配置]
//    //*敵キャラがいる場合は生成させない
//    void SetEnemy(int count = 1,EnemyType enemyType = EnemyType.Easy)
//    {
//
//        StartCoroutine(DelayGenerate(enemyType,count,0.5f));
//
//
////        for (int i = 0; i < count; i++)
////        {
////        
////            //地上の出現位置をランダムに取得 ,//そこに敵キャラが一定以上いたら、再取得
////            TargetPosition generatePosition = _enemyGenerator.GetRandomGeneratePos(_currentEnemysCount, 5);
////
////            //位置を示さないものが帰ってきたら処理しない
////            //なおこれはよくない処理です
////            if (generatePosition == TargetPosition.Last)
////            {
////                Debug.Log("生成できませんでした。");
////                return;
////            }
////
////            //生成した場所のカウントを覚えておく
////            _currentEnemysCount[(int)generatePosition] += 1;
////
////            //生成
////            _enemyGenerator.GenerateEnemy(EnemyType.Easy, generatePosition);
////        }
//                                
//    }
//
//    //一気に生成させない
//    IEnumerator DelayGenerate(EnemyType enemyType, int count = 1, float delayTime = 0.5f)
//    {
//
//        float counter = 0;
//        
//        for (int i = 0; i < count; i++)
//        {
//
//            //地上の出現位置をランダムに取得 ,//そこに敵キャラが一定以上いたら、再取得
//            TargetPosition generatePosition = _enemyGenerator.GetRandomGeneratePos(_currentEnemysCount, _enemyLimit);
//
//            //位置を示さないものが帰ってきたら処理しない
//            //なおこれはよくない処理です
//            if (generatePosition == TargetPosition.Last)
//            {
//                Debug.Log("生成できませんでした。");
//
//                break;
//            }
//
//            //生成した場所のカウントを覚えておく
//            _currentEnemysCount[(int)generatePosition] += 1;
//
//            //生成
//            _enemyGenerator.GenerateEnemy(EnemyType.Easy, generatePosition);
//
//            counter = delayTime;
//
//            yield return null;
//
//            while (true)
//            {
//                counter -= Time.deltaTime;
//
//                if (counter < 0)
//                {
//                  break;
//                }
//
//                yield return null;
//            }
//
//            yield return null;
//        }
//
//
//    }

    //敵を生成する場所の数、　またそこから出す敵のカウントを設定、敵キャラを配置]
    //*敵キャラがいる場合は生成させない
    void SetEnemy(int count,List<EnemyType> enemyTypes)
    {
        StartCoroutine(DelayGenerate(enemyTypes,count,0.5f));
    }
    void SetEnemy(int count,EnemyType enemyType)
    {

        StartCoroutine(DelayGenerate(enemyType,count,0.5f));
    }


    //一気に生成させない
    IEnumerator DelayGenerate(EnemyType enemyType, int count = 1, float delayTime = 0.5f)
    {

        float counter = 0;
        
        for (int i = 0; i < count; i++)
        {

            //地上の出現位置をランダムに取得 ,//そこに敵キャラが一定以上いたら、再取得
            TargetPosition generatePosition = _enemyGenerator.GetRandomGeneratePos(_currentEnemysCount, _enemyLimit);

            //位置を示さないものが帰ってきたら処理しない
            //なおこれはよくない処理です
            if (generatePosition == TargetPosition.Last)
            {
                Debug.Log("生成できませんでした。");

                break;
            }

            //生成した場所のカウントを覚えておく
            _currentEnemysCount[(int)generatePosition] += 1;

            //生成
            _enemyGenerator.GenerateEnemy(enemyType, generatePosition);

            counter = delayTime;

            yield return null;

            //
            while (true)
            {
                counter -= Time.deltaTime;

                if (counter < 0)
                {
                  break;
                }

                yield return null;
            }

            yield return null;
        }

    }

    //一気に生成させない
    IEnumerator DelayGenerate(List<EnemyType> enemyTypes, int count = 1, float delayTime = 0.5f)
    {

        float counter = 0;
        
        for (int i = 0; i < count; i++)
        {

            //地上の出現位置をランダムに取得 ,//そこに敵キャラが一定以上いたら、再取得
            TargetPosition generatePosition = _enemyGenerator.GetRandomGeneratePos(_currentEnemysCount, _enemyLimit);

            //位置を示さないものが帰ってきたら処理しない
            //なおこれはよくない処理です
            if (generatePosition == TargetPosition.Last)
            {
                Debug.Log("生成できませんでした。");

                break;
            }

            //生成した場所のカウントを覚えておく
            _currentEnemysCount[(int)generatePosition] += 1;

            //生成
            _enemyGenerator.GenerateEnemy(ref enemyTypes, generatePosition);

            counter = delayTime;

            yield return null;

            while (true)
            {
                counter -= Time.deltaTime;

                if (counter < 0)
                {
                  break;
                }

                yield return null;
            }

            yield return null;
        }

    }

    public void DestroyAllEnemy()
    {

        //List<Enemy> enemys = new List<Enemy>();
        var enemys = GameObject.FindObjectsOfType<Enemy>();

        foreach (var enemy in enemys)
        {
            Destroy(enemy.gameObject);
        }
    }


}

