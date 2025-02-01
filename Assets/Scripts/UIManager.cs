using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class UIManager : MonoBehaviour
{
    PlayerInputActions _inputActions;
    [Header("游戏界面")]
    public GameObject _uiStart;
    public GameObject _uiGameOver;
    public GameObject _uiHelp;
    [Header("游戏中UI")]
    public TextMeshProUGUI _scoreText;
    public TextMeshProUGUI _kidNumText;
    public Slider _playerSlider;
    public Slider _playerSlider2;
    public Image _playerSliderImage;
    public Image _playerSlider2Image;
    public Color _playerSliderImageColor_over;
    Color _playerSliderImageColor_original;
    public GameObject _player2JoinMessage;
    Coroutine _player2JoinMessageCoroutine;
    public GameObject _arrowGo;
    public float _arrowGoTime;
    [Header("游戏开始的对象")]
    public GameObject _start_itemManager;
    public GameObject _start_player;
    public GameObject _start_player2;
    public GameObject _start_icePlane;
    public GameObject _start_kidPrefabs;
    public Vector3 _start_kid_originalPos;
    public GameObject _start_SeaWave;
    public IceMountainManager _start_iceMountainManagerCS;
    [Header("游戏结束的对象&UI")]
    public Transform _end_Item;
    public CatchCollision _end_playerCatchCS;
    public TextMeshProUGUI _endUI_highScore;
    public TextMeshProUGUI _endUI_scoreLength;
    public TextMeshProUGUI _endUI_scoreKidCount;
    public TextMeshProUGUI _endUI_scoreResult;
    [Header("音乐")]
    public AudioClip _clipBottom;
    public AudioClip _clipPlayer2Join;
    AudioSource _audio;

    Vector3 _start_SeaWave_originalPos;
    int _highScore;
    int _score;
    int _kidCount;
    float _currentArrowGoTime;

    bool _isArrowGo;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _uiStart.SetActive(true);
        _uiGameOver.SetActive(false);
        //关闭游戏中的UI
        _uiHelp.SetActive(false);
        _scoreText.transform.parent.gameObject.SetActive(false);
        _playerSlider.transform.parent.gameObject.SetActive(false);
        _playerSlider2.transform.parent.gameObject.SetActive(false);
        _player2JoinMessage.SetActive(false);
        _arrowGo.SetActive(false);
        //关闭全部游戏开始时候的对象
        _start_itemManager.SetActive(false);
        _start_player.SetActive(false);
        _start_player2.SetActive(false);
        _start_icePlane.SetActive(false);
        _start_SeaWave.SetActive(false);
        //保存位置用以重新开始游戏
        _start_SeaWave_originalPos = _start_SeaWave.transform.position;
        //其他操作
        _playerSliderImageColor_original = _playerSliderImage.color;
    }

    private void Update()
    {
        //弹出游戏帮助
        if (_uiHelp.activeSelf
        && _inputActions.UI.Enter.WasPressedThisFrame())
        {
            GameRestart();
            _uiHelp.SetActive(false);
            PlayAudio(_clipBottom);
        }
        //游戏开始
        if (_uiStart.activeSelf
        && _inputActions.UI.Enter.WasPressedThisFrame())
        {
            _uiHelp.SetActive(true);
            _uiStart.SetActive(false);
            PlayAudio(_clipBottom);
        }
        //游戏中
        if (_scoreText.transform.parent.gameObject.activeSelf
        && _start_player2.activeSelf == false
        && _inputActions.GamePlay2.JoinGame.WasPressedThisFrame())
        {
            Player2Start();
            PlayAudio(_clipPlayer2Join);
        }
        //游戏结束
        if (_uiGameOver.activeSelf
        && _inputActions.UI.Enter.WasPressedThisFrame())
        {
            _uiHelp.SetActive(true);
            _uiGameOver.SetActive(false);
            PlayAudio(_clipBottom);
        }
        //ArrowGo的指引
        if (_isArrowGo)
        {
            _currentArrowGoTime += Time.deltaTime;
            if (_currentArrowGoTime > _arrowGoTime)
            {
                _currentArrowGoTime = 0;
                _arrowGo.SetActive(false);
                _isArrowGo = false;
            }
        }
    }

    void GameOver_start()
    {
        _uiGameOver.SetActive(true);
        _scoreText.transform.parent.gameObject.SetActive(false);
        _playerSlider.transform.parent.gameObject.SetActive(false);
        _playerSlider2.transform.parent.gameObject.SetActive(false);
        _player2JoinMessage.SetActive(false);
        _start_player.SetActive(false);
        _start_player2.SetActive(false);
        //游戏结束分数
        int resultScore = _score * _kidCount;
        _endUI_scoreLength.text = _score + " m";
        _endUI_scoreKidCount.text = _kidCount + " kid";
        _endUI_scoreResult.text = resultScore + "";
        if (resultScore > _highScore)
        {
            _highScore = resultScore;
        }
        _endUI_highScore.text = "" + _highScore;
    }
    void GameRestart()
    {
        //打开ItemManager和删除所有Item
        _start_itemManager.SetActive(true);
        for (int i = _end_Item.childCount - 1; i >= 0; i--)
        {
            Transform child = _end_Item.GetChild(i);
            Destroy(child.gameObject);
        }
        //打开UI和重置UI数值
        _scoreText.transform.parent.gameObject.SetActive(true);
        _playerSlider.transform.parent.gameObject.SetActive(true);
        _kidCount = 0;
        _scoreText.text = "0000000000";
        _kidNumText.text = "X00";
        _arrowGo.SetActive(true);
        _isArrowGo = true;
        _player2JoinMessage.SetActive(true);
        if (_player2JoinMessageCoroutine != null)
            StopCoroutine(_player2JoinMessageCoroutine);
        _player2JoinMessageCoroutine = StartCoroutine(EnumTimeObjOff(_player2JoinMessage, 10));
        //打开Player和重置
        _start_player.SetActive(true);
        CatchCollision catchCS = _start_player.transform.Find("CatchCollison").GetComponent<CatchCollision>();
        catchCS.CatchThingNull();
        Player playerCS = _start_player.GetComponent<Player>();
        playerCS.Inital();
        //重置IcePlane
        _start_icePlane.SetActive(true);
        IcePlane icePlaneCS = _start_icePlane.GetComponent<IcePlane>();
        icePlaneCS.Inital();
        //生成一个Kid
        GameObject itemKid = Instantiate(_start_kidPrefabs, _start_kid_originalPos, Quaternion.Euler(0, -90, 0), _end_Item);
        ItemBase itemBase = itemKid.GetComponent<ItemBase>();
        itemBase.Initial(_start_kid_originalPos, _start_player.transform);
        //重置SeaWave的位置
        _start_SeaWave.SetActive(true);
        _start_SeaWave.transform.SetPositionAndRotation(_start_SeaWave_originalPos, Quaternion.Euler(0, 90, 0));
        //重置整个冰山地图
        _start_iceMountainManagerCS.ResetMap();
    }

    void Player2Start()
    {
        _start_player2.SetActive(true);
        CatchCollision catchCS = _start_player2.transform.Find("CatchCollison").GetComponent<CatchCollision>();
        catchCS.CatchThingNull();
        Player playerCS = _start_player2.GetComponent<Player>();
        playerCS.Inital();
        _start_player2.transform.position = new Vector3(5, 7, 0) + _start_player.transform.position;
        _playerSlider2.transform.parent.gameObject.SetActive(true);
        _player2JoinMessage.SetActive(false);
    }

    void ScoreUpdate(int score)
    {
        //游戏中界面
        string scoreText = score.ToString("D10");
        _scoreText.text = scoreText;
        _score = score;
    }
    void KidCountUpdate(int count)
    {
        string text = count.ToString("D2");
        _kidNumText.text = "X" + text;
        _kidCount = count;
    }
    void PlayerStaminaUpdate(int playerIndex, float max, float current, bool isUseStaminaOver)
    {
        float value = current / max;
        switch (playerIndex)
        {
            case 1:
                _playerSlider.value = value;
                if (isUseStaminaOver)
                    _playerSliderImage.color = _playerSliderImageColor_over;
                else
                    _playerSliderImage.color = _playerSliderImageColor_original;
                break;
            case 2:
                _playerSlider2.value = value;
                if (isUseStaminaOver)
                    _playerSlider2Image.color = _playerSliderImageColor_over;
                else
                    _playerSlider2Image.color = _playerSliderImageColor_original;
                break;
        }
    }
    IEnumerator EnumTimeObjOff(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (obj != null)
            obj.SetActive(false);
    }
    void PlayAudio(AudioClip clip)
    {
        if (_audio == null)
            return;
        _audio.Stop();
        _audio.clip = clip;
        _audio.Play();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        Player.OnScoreUpdate += ScoreUpdate;
        IcePlane.OnKidCount += KidCountUpdate;
        Player.OnStaminaUpdate += PlayerStaminaUpdate;
        SeaWave.OnGameOver += GameOver_start;
    }
    private void OnDisable()
    {
        _inputActions.Disable();
        Player.OnScoreUpdate -= ScoreUpdate;
        IcePlane.OnKidCount -= KidCountUpdate;
        Player.OnStaminaUpdate -= PlayerStaminaUpdate;
        SeaWave.OnGameOver -= GameOver_start;
    }
}
