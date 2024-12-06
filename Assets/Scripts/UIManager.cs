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

    Vector3 _start_player_originalPos;
    Vector3 _start_icePlane_originalPos;
    Vector3 _start_icePlane_originalScale;
    Vector3 _start_SeaWave_originalPos;
    int _highScore;

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
        _start_player_originalPos = _start_player.transform.position;
        _start_icePlane_originalPos = _start_icePlane.transform.position;
        _start_icePlane_originalScale = _start_icePlane.transform.localScale;
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
    }
    void GameRestart()
    {
        //删除所有Item
        for (int i = _end_Item.childCount - 1; i >= 0; i--)
        {
            Transform child = _end_Item.GetChild(i);
            Destroy(child.gameObject);
        }
        //打开UI
        _scoreText.transform.parent.gameObject.SetActive(true);
        _playerSlider.transform.parent.gameObject.SetActive(true);
        //打开Player和删除Player手上的东西
        _start_player.SetActive(true);
        CatchCollision catchCS = _start_player.transform.Find("CatchCollison").GetComponent<CatchCollision>();
        catchCS.CatchThingNull();
        //重置Player位置和刚体
        _start_player.transform.SetPositionAndRotation(_start_player_originalPos, Quaternion.Euler(0, -90, 0));
        Rigidbody rb = _start_player.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //重置IcePlane位置和刚体
        _start_icePlane.transform.SetPositionAndRotation(_start_icePlane_originalPos, Quaternion.Euler(0, 0, 0));
        _start_icePlane.transform.localScale = _start_icePlane_originalScale;
        rb = _start_icePlane.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //生成一个Kid
        Instantiate(_start_kidPrefabs, _start_kid_originalPos, Quaternion.Euler(0, -90, 0), _end_Item);
        _start_SeaWave.SetActive(true);
        //重置SeaWave的位置
        _start_SeaWave.transform.SetPositionAndRotation(_start_SeaWave_originalPos, Quaternion.Euler(0, 90, 0));
    }

    void ScoreUpdate(int score)
    {
        //游戏中界面
        string scoreText = score.ToString("D10");
        _scoreText.text = scoreText + " m";
        //游戏结束界面
        _endUI_score.text = "" + score;
        if (score > _highScore)
        {
            _highScore = score;
            _endUI_highScore.text = "" + _highScore;
        }
    }
    void KidCountUpdate(int count)
    {
        string text = count.ToString("D2");
        _kidNumText.text = "X" + text;
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
