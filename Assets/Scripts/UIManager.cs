using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("游戏界面")]
    public GameObject _uiStart;
    public GameObject _uiGameOver;
    [Header("游戏中UI")]
    public TextMeshProUGUI _scoreText;
    public TextMeshProUGUI _kidNumText;
    public Slider _playerSlider;
    [Header("游戏开始的对象")]
    public GameObject _start_itemManager;
    public GameObject _start_player;
    public GameObject _start_icePlane;
    public GameObject _start_kidPrefabs;
    public Vector3 _start_kid_originalPos;
    public GameObject _start_SeaWave;
    [Header("游戏结束的对象&UI")]
    public Transform _end_Item;
    public CatchCollision _end_playerCatchCS;
    public TextMeshProUGUI _endUI_highScore;
    public TextMeshProUGUI _endUI_score;

    Vector3 _start_SeaWave_originalPos;
    int _highScore;
    int _score;
    int _kidCount;

    private void Start()
    {
        _uiStart.SetActive(true);
        _uiGameOver.SetActive(false);
        //关闭游戏中的UI
        _scoreText.transform.parent.gameObject.SetActive(false);
        _playerSlider.transform.parent.gameObject.SetActive(false);
        //关闭全部游戏开始时候的对象
        _start_itemManager.SetActive(false);
        _start_player.SetActive(false);
        _start_icePlane.SetActive(false);
        _start_SeaWave.SetActive(false);
        //保存位置用以重新开始游戏
        _start_SeaWave_originalPos = _start_SeaWave.transform.position;
    }

    private void Update()
    {
        //游戏开始
        if (_uiStart.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            GameStart_end();
        }
        //游戏结束
        if (_uiGameOver.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            GameRestart();
            _uiGameOver.SetActive(false);
        }
    }

    void GameStart_end()
    {
        _uiStart.SetActive(false);
        _scoreText.transform.parent.gameObject.SetActive(true);
        _playerSlider.transform.parent.gameObject.SetActive(true);
        _start_itemManager.SetActive(true);
        _start_player.SetActive(true);
        _start_icePlane.SetActive(true);
        Instantiate(_start_kidPrefabs, _start_kid_originalPos, Quaternion.Euler(0, -90, 0), _end_Item);
        _start_SeaWave.SetActive(true);
    }
    void GameOver_start()
    {
        _uiGameOver.SetActive(true);
        _scoreText.transform.parent.gameObject.SetActive(false);
        _playerSlider.transform.parent.gameObject.SetActive(false);
        _start_player.SetActive(false);
        //游戏结束分数
        int resultScore = _score * _kidCount;
        _endUI_score.text = _score + " X " + _kidCount + " = " + resultScore;
        if (resultScore > _highScore)
        {
            _highScore = resultScore;
        }
        _endUI_highScore.text = "" + _highScore;
    }
    void GameRestart()
    {
        //删除所有Item
        for (int i = _end_Item.childCount - 1; i >= 0; i--)
        {
            Transform child = _end_Item.GetChild(i);
            Destroy(child.gameObject);
        }
        //打开UI和重置UI数值
        _scoreText.transform.parent.gameObject.SetActive(true);
        _playerSlider.transform.parent.gameObject.SetActive(true);
        _scoreText.text = "0000000000 m";
        _kidNumText.text = "X00";
        //打开Player和重置
        _start_player.SetActive(true);
        CatchCollision catchCS = _start_player.transform.Find("CatchCollison").GetComponent<CatchCollision>();
        catchCS.CatchThingNull();
        Player playerCS = _start_player.GetComponent<Player>();
        playerCS.Inital();
        //重置IcePlane
        IcePlane icePlaneCS = _start_icePlane.GetComponent<IcePlane>();
        icePlaneCS.Inital();
        //生成一个Kid
        Instantiate(_start_kidPrefabs, _start_kid_originalPos, Quaternion.Euler(0, -90, 0), _end_Item);
        //重置SeaWave的位置
        _start_SeaWave.SetActive(true);
        _start_SeaWave.transform.SetPositionAndRotation(_start_SeaWave_originalPos, Quaternion.Euler(0, 90, 0));
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
        Player.OnScoreUpdate += ScoreUpdate;
        IcePlane.OnKidCount += KidCountUpdate;
        Player.OnStaminaUpdate += PlayerStaminaUpdate;
        SeaWave.OnGameOver += GameOver_start;
    }
    private void OnDisable()
    {
        Player.OnScoreUpdate -= ScoreUpdate;
        IcePlane.OnKidCount -= KidCountUpdate;
        Player.OnStaminaUpdate -= PlayerStaminaUpdate;
        SeaWave.OnGameOver -= GameOver_start;
    }
}
