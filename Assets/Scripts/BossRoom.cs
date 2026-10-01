using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoom : MonoBehaviour
{
    private RoomDirector _roomDirector;
    private PlayerMovementScript _playerMoveScript;
    private GunMenuEnabler _gunEnabler;
    private GameObject _playerObj;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform bossSpawnCamPos;
    [SerializeField] private Animator anim;

    Lerp camLerp;

    bool RoomHasReveld;
    bool MoveCamDone;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _roomDirector = GetComponent<RoomDirector>();
        _playerMoveScript = FindAnyObjectByType<PlayerMovementScript>(FindObjectsInactive.Include);
        _playerObj = _playerMoveScript.gameObject;
        _gunEnabler = _playerMoveScript.GetComponent<GunMenuEnabler>();
        camLerp = mainCam.GetComponent<Lerp>();
        _roomDirector.OnRoomStart += OnRoomStart;
        _roomDirector.OnRoomReveled += OnRoomReveled;
    }

    void OnRoomStart()
    {
        Debug.LogWarning("Stuff Ran");
        _playerMoveScript.enabled = false;
        _gunEnabler.enabled = false;
        _playerObj.transform.up = Vector2.right;
        camLerp.enabled = false;
        _playerObj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Invoke("Delayed", 1.5f);
    }

    void Delayed()
    {
        StartCoroutine(LerpCam(_playerObj.transform.position, bossSpawnCamPos.position, 2f, MoveCamAnimCallback));
    }

    void OnRoomReveled()
    {
        if (MoveCamDone)
        {
            Step2();
        }
        RoomHasReveld = true;
    }

    public void MoveCamAnimCallback()
    {
        if (RoomHasReveld)
        {
            Step2();
        }
        MoveCamDone = true;
    }

    void Step2()
    {
        StartCoroutine(RunBossSpawnAnim());
    }

    public IEnumerator LerpCam(Vector3 StartPos, Vector3 EndPos, float LerpTime, Action callBack = null)
    {
        camLerp.enabled = false;
        Transform camTrans = mainCam.transform;
        float curTime = 0;
        StartPos.z = -10;
        EndPos.z = -10;
        camTrans.position = StartPos;
        while (curTime < LerpTime)
        {
            Vector3 lerpedPos = Vector3.Lerp(StartPos, EndPos, curTime / LerpTime);
            camTrans.position = lerpedPos;
            curTime += Time.unscaledDeltaTime;
            yield return null;
        }
        camTrans.position = EndPos;
        callBack?.Invoke();
    }

    IEnumerator RunBossSpawnAnim()
    {
        Time.timeScale = 0;
        anim.SetTrigger("StartFight");
        yield return null;
        //Todo add the boss spawn anim here
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSecondsRealtime(animationLength);


        yield return LerpCam(bossSpawnCamPos.position, _playerObj.transform.position, 2f);
        Time.timeScale = 1;
        RestartGameplay();
    }

    void RestartGameplay()
    {
        _playerMoveScript.enabled = true;
        _gunEnabler.enabled = true;
        camLerp.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameEnd()
    {
        StartCoroutine(EndCredits());
    }

    IEnumerator EndCredits()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2f);
        yield return LerpCam(mainCam.transform.position, bossSpawnCamPos.position, 2f);

        //Credits
        anim.SetTrigger("Fakeout");
        yield return null;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSecondsRealtime(animationLength);

        yield return new WaitForSecondsRealtime(2f);

        yield return LerpCam(mainCam.transform.position, _playerMoveScript.transform.position, 2f);

        yield return new WaitForSecondsRealtime(1f);
        anim.SetTrigger("Credits");
        yield return null;

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        animationLength = stateInfo.length;

        yield return new WaitForSecondsRealtime(animationLength);
        Time.timeScale = 1;
        SceneManager.LoadScene("Menui scene");
    }
}
