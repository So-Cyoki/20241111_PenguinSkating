using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    Vector3 _start_SeaWave_originalPos;
    int _highScore;
    int _score;
    int _kidCount;
    float _currentArrowGoTime;
    float _currentArrowGoTime_effect;

    bool _isArrowGo;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        _uiStart.SetActive(true);
        _uiGameOver.SetActive(false);
        //关闭游戏中的UI
        _uiHelp.SetActive(false);
        _scoreText.transform.parent.gameObject.SetActive(false);
        _playerSlider.transform.parent.gameObject.SetActive(false);
        _arrowGo.SetActive(false);
        //关闭全部游戏开始时候的对象
        _start_itemManager.SetActive(false);
        _start_player.SetActive(false);
        _start_player2.SetActive(false);
        _start_icePlane.SetActive(false);
        _start_SeaWave.SetActive(false);
        //保存位置用以重新开始游戏
        _start_SeaWave_originalPos = _start_SeaWave.transform.position;
    }

    private void Update()
    {
        //弹出游戏帮助
        if (_uiHelp.activeSelf
        && _inputActions.UI.Enter.WasPressedThisFrame())
        {
            GameRestart();
            _uiHelp.SetActive(false);
        }
        //游戏开始
        if (_uiStart.activeSelf
        && _inputActions.UI.Enter.WasPressedThisFrame())
        {
            _uiHelp.SetActive(true);
            _uiStart.SetActive(false);
        }
        //游戏中
        if (_scoreText.transform.parent.gameObject.activeSelf
        && _start_player2.activeSelf == false
        && _inputActions.GamePlay2.JoinGame.WasPressedThisFrame())
        {
            Player2Start();
        }
        //游戏结束
        if (_uiGameOver.activeSelf
        && _inputActions.UI.Enter.WasPressedThisFrame())
        {
            _uiHelp.SetActive(true);
            _uiGameOver.SetActive(false);
        }
        //ArrowGo的指引
        if (_isArrowGo)
        {
            _currentArrowGoTime += Time.deltaTime;
            if (_currentArrowGoTime > _arrowGoTime)
            {
                _currentArrowGoTime = 0;
                _currentArrowGoTime_effect = 0;
                _arrowGo.SetActive(false);
                _isArrowGo = false;
            }
            _currentArrowGoTime_effect += Time.deltaTime;
            if (_currentArrowGoTime_effect > 0.5f)
            {
                _arrowGo.SetActive(_arrowGo.activeSelf == true ? false : true);
                _currentArrowGoTime_effect = 0;
            }
        }
    }

    void GameOver_start()
    {
        _uiGameOver.SetActive(true);
        _scoreText.transform.parent.gameObject.SetActive(false);
        _playerSlider.transform.parent.gameObject.SetActive(false);
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
        _scoreText.text = "0000000000 m";
        _kidNumText.text = "X00";
        _arrowGo.SetActive(true);
        _isArrowGo = true;
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
    }

    void ScoreUpdate(int score)
    {
        //游戏中界面
        string scoreText = score.ToString("D10");
        _scoreText.text = scoreText + " m";
        _score = score;
    }
    void KidCountUpdate(int count)
    {
        string text = count.ToString("D2");
        _kidNumText.text = "X" + text;
        _kidCount = count;
    }
    void PlayerStaminaUpdate(float max, float current)
    {
        float value = current / max;
        _playerSlider.value = value;
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
