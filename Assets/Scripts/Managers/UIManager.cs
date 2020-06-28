using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /*[SerializeField]
    Slider slider;
    [SerializeField]
    Slider slider2;

    [SerializeField]
    Text text;
    [SerializeField]
    Text text2;*/

    [SerializeField]
    GameObject canvas;
    [SerializeField]
    Text generalText;
    [SerializeField]
    Text[] startText;
    [SerializeField]
    Text Timer;
    [SerializeField]
    Text[] Points;
    [SerializeField]
    Text[] NewPoints;
    [SerializeField]
    Text ScaleText;
    [SerializeField]
    Text ThresholdText;
    [SerializeField]
    Text OffsetText;
    [SerializeField]
    Text DelayText;
    [SerializeField]
    Text ModelSelectedText;
    [SerializeField]
    Slider Threshold;
    [SerializeField]
    Slider Scale;
    [SerializeField]
    Slider Offset;
    [SerializeField]
    Slider Delay;
    [SerializeField]
    GameObject Again;
    [SerializeField]
    GameObject Exit;
    [SerializeField]
    Dropdown TimeSelection;
    [SerializeField]
    Dropdown SpawnTimeSelection;
    [SerializeField]
    Dropdown ModelSelection;
    [SerializeField]
    GameObject congrats;
    [SerializeField]
    GameObject MusicSyncPanel;
    [SerializeField]
    GameObject LevelsPanel;
    [SerializeField]
    Toggle MusicSync;
    [SerializeField]
    Slider playerHealthBar;



    private float pointsTimer = 3;
    private void Start()
    {
        GamePlayManager.obj.gameEnded += GameEnded;
        GamePlayManager.obj.gameStarted += GameStarted;

        /*slider.value = PlayerPrefs.GetFloat(GameManager.GetLevelTag("speed"));
        text.text = "VAsL: " + slider.value;
        slider2.value = PlayerPrefs.GetFloat(GameManager.GetLevelTag("delay"));
        text2.text = "VAsL: " + slider2.value;*/
        /*foreach (SparkModel food in GameManager.obj.configure.audioSpectrumCreator.sparkDataModel.models)
        {
            FoodSelection.options.Add(new Dropdown.OptionData() { text = food.type.ToString() });
        }
        FoodSelection.value = 1;
        FoodSelection.value = 0;
        FoodSelection.onValueChanged.AddListener(delegate
        {
            SetFoodSelection();
        });
        SparkModel model = GameManager.obj.configure.GetFoodModel(FoodSelection.value);
        SetValues(model);
        canvas.transform.position = new Vector3(canvas.transform.position.x, GameManager.obj.player.position.y + 0.5f, canvas.transform.position.z);
        */
    }
    private void OnDestroy()
    {
        GamePlayManager.obj.gameEnded -= GameEnded;
        GamePlayManager.obj.gameStarted -= GameStarted;
    }
    private void Update()
    {
        if (GamePlayManager.obj.gameStatus != GamePlayManager.GameStatus.Started) return;


        if (pointsTimer < 3)
        {
            if (!Points[0].enabled)
            {
                for (int i = 0; i < Points.Length; i++)
                {

                    Points[i].enabled = true;
                    NewPoints[i].enabled = true;
                }
            }
            pointsTimer += Time.deltaTime;
        }
        else if (Points[0].enabled)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i].enabled = false;
                NewPoints[i].enabled = false;
            }
        }
    }
    IEnumerator StartText()
    {
        yield return new WaitForSeconds(1);
        foreach (Text s in startText) {
            s.text = "2";
        }
        yield return new WaitForSeconds(1);

        foreach (Text s in startText)
        {
            s.text = "3";
        }
        yield return new WaitForSeconds(1);

        foreach (Text s in startText)
        {
            s.text = "GO";
        }
        yield return new WaitForSeconds(1);

        foreach (Text s in startText)
        {
            Destroy(s);
        }
    }
    public void ShowInterface()
    {
        canvas.SetActive(!canvas.activeSelf);
    }

    /*public void SpeedChange()
    {
        PlayerPrefs.SetFloat(GameManager.GetLevelTag("speed"), slider.value);
        text.text = "VAL: " + slider.value;
    }
    public void DelayChange()
    {
        PlayerPrefs.SetFloat(GameManager.GetLevelTag("delay"), slider2.value);
        text2.text = "VAL: " + slider2.value;
    }*/
    public void UpdateHealthBar(float playerHealth)
    {
        playerHealthBar.value = playerHealth;
    }
    public void UpdateTimer(int timer)
    {
        int sec = timer % 60;
        int min = timer / 60;
        // Timer.text = (min < 10 ? "0" + min : "" + min) + ":" + (sec < 10 ? "0" + sec : "" + sec);
    }

    public void UpdatePoints(int points, int newpoints)
    {
        for (int i = 0; i < Points.Length; i++)
        {
            Points[i].text = points + "pts";
            if (newpoints < 0)
            {
                NewPoints[i].text = "";
                NewPoints[i].color = Color.red;
            }
            else
            {
                NewPoints[i].text = "+";
                NewPoints[i].color = Color.green;
            }
            NewPoints[i].text += newpoints + "pts";
        }
        pointsTimer = 0;
    }
    public void GameStarted()
    {
        if (!GamePlayManager.obj.isDebug)
        debug.gameObject.SetActive(false);
        Timer.enabled = true;
        //TimeSelection.gameObject.SetActive(false);
        //FoodSelection.gameObject.SetActive(false);

        foreach (Text s in startText)
            s.enabled = true;
        //LevelsPanel.SetActive(false);
        StartCoroutine(StartText());
    }
    public void GameEnded()
    {
        Again.SetActive(true);
        //Exit.SetActive(true);
        congrats.SetActive(true);
        for (int i = 0; i < Points.Length; i++)
        {
            Points[i].transform.position +=  Vector3.up * 5;
            Points[i].transform.Rotate(-90, 0, 0);
            Points[i].enabled = true;
        }
    }
    public void ChangegameDuration()
    {
        /*switch (TimeSelection.value)
        {
            case 0:
            case 1:
                GameManager.obj.duration = (TimeSelection.value + 1) * 60;
                break;
            case 2:
                GameManager.obj.duration = 5 * 60;
                break;
        }*/
    }

    public void Restart()
    {
        /*GameManager.obj.configure.Restart();
        SparkModel model = GameManager.obj.configure.GetFoodModel(ModelSelection.value);
        SetValues(model);*/
    }
    private bool lockValues;
    private void SetValues(SparkModel model)
    {
        lockValues = true;
        string food_name = model.type.ToString();

        Threshold.value = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag(food_name + "_Th"));
        ThresholdText.text = Threshold.value + "";

        Scale.value = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag(food_name));
        ScaleText.text = Scale.value + "";

        Delay.value = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag("Delay"));
        DelayText.text = "" + Delay.value;

        Offset.value = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag("Offset"));
        OffsetText.text = "" + Offset.value;

        MusicSync.isOn = PlayerPrefs.GetInt(GamePlayManager.GetLevelTag("MusicSyc")) == 1;
        RefreshMusicOnUI();

        ModelSelectedText.text = food_name;
        generalText.text = "Configuracion General Nivel: " + GamePlayManager.obj.level;

        Invoke("Unlock", 1);
    }
    private void Unlock()
    {
        lockValues = false;
    }
    public void SetLevel(int level)
    {
        GamePlayManager.obj.level = level;
    }
    /*public void SetFoodSelection()
    {
        lockValues = true;
        SparkModel model = GameManager.obj.configure.InstantiateModel(ModelSelection.value);

        Threshold.maxValue = model.maxThreshold;
        Threshold.minValue = model.minThreshold;

        Scale.maxValue = model.maxScale;
        Scale.minValue = model.minScale;

        SetValues(model);
    }*/
    public void SetThreshold()
    {
        if (lockValues) return;
        ThresholdText.text = "" + Threshold.value;
        //GameManager.obj.configure.SetThreshold(ModelSelection.value, Threshold.value);
    }
    public void SetScale()
    {
        if (lockValues) return;
        ScaleText.text = "" + Scale.value;
        //GameManager.obj.configure.SetScale(ModelSelection.value, Scale.value);
    }
    public void SetOffset()
    {
        if (lockValues) return;
        OffsetText.text = "" + Offset.value;
        //GameManager.obj.configure.SetRadiusOffset(Offset.value);
    }
    public void SetDelay()
    {
        if (lockValues) return;
        DelayText.text = "" + Delay.value;
        //GameManager.obj.configure.SetSpawnTime(Delay.value);
    }
    public void SetMusicSync()
    {
        if (lockValues) return;
        //GameManager.obj.configure.SetMusicSync(MusicSync.isOn ? 1 : 0);
        RefreshMusicOnUI();
    }
    private void RefreshMusicOnUI()
    {
        if (lockValues) return;
        MusicSyncPanel.SetActive(!MusicSync.isOn);
        Delay.gameObject.SetActive(MusicSync.isOn);
        if (MusicSync.isOn)
        {
            Delay.value = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag("Delay"), 0.2f);
            DelayText.text = "" + Delay.value;
        }
        else
        {
            float spawnTime = PlayerPrefs.GetFloat(GamePlayManager.GetLevelTag("Delay"), 1.0f);
            SpawnTimeSelection.value = spawnTime <= 1.0f ? 0 : (spawnTime <= 2.0f ? 1 : (spawnTime <= 5.0f ? 2 : 3));
        }
    }
    public void ChangeSpawnTime()
    {
        if (lockValues) return;
        float time = 1f;
        switch (SpawnTimeSelection.value)
        {
            case 0:
            case 1:
                time = (TimeSelection.value + 1f);
                break;
            case 2:
                time = 5f;
                break;
            case 3:
                time = 10f;
                break;
        }
        //GameManager.obj.configure.SetSpawnTime(time);
    }

    /*private string GetConfigurations()
    {
        string config = "SpawnTime: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag("Delay"), 1.0f) + "\n";
        config += "MusicSyc: " + PlayerPrefs.GetInt(GameManager.GetLevelTag("MusicSyc"), 1) + "\n";

        //config += "Speed: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag("speed")); <- it is not being used
        //config += "Delay: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag("delay")); <- it is not being used
        config += "Offset: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag("Offset"), 0.05f) + "\n";
        for (int i = 0; i < ModelSelection.options.Count; i++)
        {
            SparkModel model = GameManager.obj.configure.GetFoodModel(i);
            string food = model.type.ToString();
            config += "**** [" + food + "] **** \n";
            config += "Scale: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag(food), model.scale) + "\n";
            config += "Threshold: " + PlayerPrefs.GetFloat(GameManager.GetLevelTag(model.type.ToString() + "_Th"), model.life) + "\n";
        }
        return config;
    }*/
    public void DisplayConfigurations(Text dataText)
    {
        //dataText.text = GetConfigurations();
    }
    public void TooglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
    [SerializeField]
    Text debug;
    public void UIDebug(string message)
    {
        debug.text += message + "\n";
    }
}
