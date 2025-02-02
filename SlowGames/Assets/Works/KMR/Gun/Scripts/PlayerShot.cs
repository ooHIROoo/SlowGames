﻿
using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Reload))]
public class PlayerShot : MonoBehaviour
{
    Reload _reload;

    [SerializeField]
    GameObject _mazzleFlush;

    [SerializeField]
    GameObject _bullet;

    Recoil _recoil;

    //[SerializeField]
    //int _oneShotCount = 3;

    //[SerializeField]
    //float _vibrationPower = 200;

    //[SerializeField]
    //float _burstIntervalTime = 0.2f;

    [SerializeField]
    int _maxBulletsNumbers = 20;

    [SerializeField]
    private Animator _triggerAnim = null;

    int _reloadHash;

    bool _isReload;

    int _bulletsNumber;

    public int maxBulletsNumbers
    {
        get { return _maxBulletsNumbers; }
        set { _maxBulletsNumbers = value; }
    }

    public int bulletsNumber
    {
        get { return _bulletsNumber; }
        set { _bulletsNumber = value; }
    }

    enum shotType
    {
        notReload,
        autoReload
    }

    bool _isShot = false;

    public bool isShot
    {
        get { return _isShot; }
    }

    //float _time;

    int _burstCount;


    AimAssist _aimAssist;

    SteamVR_TrackedObject _trackedObject;
    SteamVR_Controller.Device _device;

    [SerializeField]
    shotType _shotType = shotType.autoReload;

    [SerializeField]
    Animator _gunAnim = null; 

    bool _isStart = false;

    AudioSource _bulletShotSe = null;

    public bool isStart
    {
        get{ return _isStart; }
        set { _isStart = value; }
    }

    int _reShotHash;
    bool _reShot = false;

    void Start()
    {
        _isReload = false;
        _reloadHash = Animator.StringToHash("isReload");
        _reShotHash = Animator.StringToHash("reShot");
        //_isStart = true;
        _recoil = GetComponent<Recoil>();
        //_shotType = shotType.autoReload;
        _aimAssist = GetComponentInChildren<AimAssist>();
        _bulletsNumber = _maxBulletsNumbers;
        _reload = GetComponent<Reload>();
        //_burstCount = _oneShotCount;
       // _time = _burstIntervalTime;
        if (!SteamVR.active) return;
        _trackedObject = GetComponent<SteamVR_TrackedObject>();

        _bulletShotSe = AudioManager.instance.getSe(AudioName.SeName.PlayerGun);
    }

    void Update()
    {
        //Debug.Log(_isReload);
        if (SteamVR.active) { _device = SteamVR_Controller.Input((int)_trackedObject.index); }

        if (!_isStart) return;
        //Debug.Log(_reShot);
        ReloadAnim();


        //_gunAnim.SetBool(_reShotHash, _reShot);


        //if (_reShot)
        //{
        //    _reShot = false;
        //    _gunAnim.SetBool(_reShotHash, _reShot);
        //}
        //if (_isShot) { _isShot = false; }


        if (_reload.isReload)
        {
            if (_isShot) _isShot = false;
            return;
        }

        

        if (_reShot)
        {
            return;
        }
        if (_gunAnim != null)
        {
            float value = _device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x; //トリガーのニュウリョクの深さを0～1で受け取る
            SetAnimFrame(value); //Animationの決定
        }
        ThreeBurst();

        if (!SteamVR.active && !Input.GetKeyDown(KeyCode.A) ||
            SteamVR.active && !_device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) { return; }
        
        _aimAssist.OrientationCorrection();
        _isShot = true;
        //_burstCount = _oneShotCount;
        
    }

    void ThreeBurst()
    {
        if (_shotType == shotType.autoReload)
        {
            if (_bulletsNumber <= 0) return;
        }
        if (_isShot == false) return;
        _gunAnim.speed = 1.0f;

        //_time -= Time.unscaledDeltaTime;
        //if (_time > 0) return;

        _bulletShotSe.Play();

        //_recoil.RecoilAnimation();

        if (SteamVR.active)
        {
            _device.TriggerHapticPulse(4000);
        }
        GameObject shotBullet = Instantiate(_bullet);
        //ScoreManager.instance.AddShotCount();
        if (_aimAssist.enemyHit == false)
        {
            shotBullet.transform.rotation = transform.rotation;
            shotBullet.GetComponent<Shot>().direction = transform.forward - transform.up;
        }
        else
        if (_aimAssist.enemyHit == true)
        {
            shotBullet.transform.rotation = transform.rotation;
            shotBullet.GetComponent<Shot>().direction = _aimAssist.enemyDirection;
        }

        //_gunAnim.SetTrigger("isShot");
        //ResetShotMove();

        _reShot = true;
        //_gunAnim.speed = 1.0f;
        _gunAnim.SetBool(_reShotHash, _reShot);
        StartCoroutine(ShotInterval());


        //弾の発生位置変更
        shotBullet.transform.position = transform.position + transform.forward * 0.4f - transform.up * 0.4f;
        var effect = Instantiate(_mazzleFlush);
        effect.transform.position = transform.position;
        effect.transform.LookAt(shotBullet.transform);
        effect.transform.position = transform.position + transform.forward * 0.2f - transform.up * 0.2f;

       //        _time = _burstIntervalTime;
       _burstCount--;
        if (_shotType == shotType.autoReload)
        {
            _bulletsNumber--;
        }
        //if (_burstCount < 1)
        //{
        //    _burstCount = _oneShotCount;
        _isShot = false;
        //}
    }

    private IEnumerator ShotInterval()
    {
        //var time = 0.0f;
        //while(time < 0.05f)
        //{
        //    time += Time.unscaledDeltaTime;
        //}
        yield return new WaitForSecondsRealtime(0.05f);
        _reShot = false;
        _gunAnim.SetBool(_reShotHash, _reShot);
        yield return null;
    }

    void SetAnimFrame(float frame)
    {
        //var clip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        //float time = (float)frame / clip.frameRate;
        if (!_gunAnim.GetCurrentAnimatorStateInfo(0).IsName("Wait")) return;
        var animationHash = _gunAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        _gunAnim.Play(animationHash, 0, frame);
    }

    void ResetShotMove()
    {
        _gunAnim.Play(0, 0, 0);
    }

    void ReloadAnim()
    {
        if(_reload.isReload)
        {
            _isReload = true;
            _gunAnim.speed = 10.0f * SlowMotion._instance.RealSpeed(); ;
        }
        else
        if(!_reload.isReload)
        {
            _gunAnim.speed = 10.0f * SlowMotion._instance.RealSpeed();
            _isReload = false;

        }
        _gunAnim.SetBool(_reloadHash, _isReload);
        

    }

    


}
