using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //set up game states
    public enum GameState { StartMenu, Playing, LevelTransition, GameOver }
    //only GameManager can change Current State, others can check, but not change
    public GameState CurrentState { get; private set; } = GameState.StartMenu;

    //set up time keeping
    public TMP_Text TimeKeeper;
    public int maxTime = 50 * 300;
    int time = 0;
    public int TimeTicks => time;

    //set up world number
    public TMP_Text WorldNumber;
    //need to set this up to update if new worlds added
    public string currentWorld = "1-1";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        //count down time if playing
        if (CurrentState != GameState.Playing) return;

        time++;
        if (time > maxTime) CurrentState = GameState.GameOver;

        TimeKeeper.text = ((maxTime - time) / 50).ToString();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        SceneManager.LoadScene("Level1");
    }

    //subscribes OnSceneLoaded after awake()
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //unsubscribes OnSceneLoaded on GameManager disable
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Re-sets TimeKeeper and WorldNumber to GameManager on new scene loading
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {       
        GameObject timeObj = GameObject.Find("TimeKeeper");
        if (timeObj != null) TimeKeeper = timeObj.GetComponent<TMP_Text>();

        GameObject worldObj = GameObject.Find("WorldNumber");
        if (worldObj != null && CurrentState == GameState.Playing)
        {
            WorldNumber = worldObj.GetComponent<TMP_Text>();
            WorldNumber.text = currentWorld;
        }
    }
}