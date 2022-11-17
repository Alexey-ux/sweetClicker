using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    public static int sugar, money;
    public Text sugarT, moneyT;

    public AudioSource musicSource, soundSource;
    public AudioClip moneyClip, buttonClip, bubbleClip;

    public Slider musicSlider, soundSlider;
    
    public GameObject ExchangerPanel, ClickersPanel, SettingsPanel;
    public Button ExchangerBtn, ClickersBtn, SettingsBtn;

    public int[] sugarTrade = new int[5] { 50, 250, 500, 2500, 5000 };
    public Button Exchange1, Exchange2, Exchange3, Exchange4, Exchange5;

    public int[] clickersPrice = new int[6] { 100, 600, 1500, 4000, 15000, 30000 };
    public Button BuyCandyBtn, BuyCupcakeBtn, BuyChocolateBtn, BuyCheesecakeBtn, BuyJellyBtn, BuyCakeBtn;

    public Button ChooseSugarBtn, ChooseCandyBtn, ChooseCupcakeBtn, ChooseChocolateBtn, ChooseCheesecakeBtn, ChooseJellyBtn, ChooseCakeBtn;

    public ParticleSystem bubblesEffect;

    public int[] sugarProfit = new int[7] { 1, 3, 5, 10, 30, 50, 100 };
    public Sprite[] clickerSprites = new Sprite[7];
    public string[] tags = new string[7] { "Sugar", "Candy", "Cupcake", "Chocolate", "Cheesecake", "Jelly", "Cake" };

    private int chosenClicker;
    private List<int> ownClickers;

    public Button ClickerBtn;
    private int currentProfit;
    public Animator animator;

    public Button ResetBtn;

    private void Awake()
    {
        LoadGame();
        Listeners1();
        Listeners2();
    }

    private void Listeners2()
    {
        ResetBtn.onClick.AddListener(() => ResetProgress());

        musicSlider.onValueChanged.AddListener(delegate { AdjustMusic(); });
        soundSlider.onValueChanged.AddListener(delegate { AdjustSound(); });

        ExchangerBtn.onClick.AddListener(() => Exchanger());
        ClickersBtn.onClick.AddListener(() => Clickers());
        SettingsBtn.onClick.AddListener(() => Settings());

        ClickerBtn.onClick.AddListener(() => OnClicker());
    }

    private void Listeners1()
    {
        Exchange1.onClick.AddListener(() => Exchange(sugarTrade[0]));
        Exchange2.onClick.AddListener(() => Exchange(sugarTrade[1]));
        Exchange3.onClick.AddListener(() => Exchange(sugarTrade[2]));
        Exchange4.onClick.AddListener(() => Exchange(sugarTrade[3]));
        Exchange5.onClick.AddListener(() => Exchange(sugarTrade[4]));

        BuyCandyBtn.onClick.AddListener(() => BuyClicker(BuyCandyBtn, ChooseCandyBtn, clickersPrice[0]));
        BuyCupcakeBtn.onClick.AddListener(() => BuyClicker(BuyCupcakeBtn, ChooseCupcakeBtn, clickersPrice[1]));
        BuyChocolateBtn.onClick.AddListener(() => BuyClicker(BuyChocolateBtn, ChooseChocolateBtn, clickersPrice[2]));
        BuyCheesecakeBtn.onClick.AddListener(() => BuyClicker(BuyCheesecakeBtn, ChooseCheesecakeBtn, clickersPrice[3]));
        BuyJellyBtn.onClick.AddListener(() => BuyClicker(BuyJellyBtn, ChooseJellyBtn, clickersPrice[4]));
        BuyCakeBtn.onClick.AddListener(() => BuyClicker(BuyCakeBtn, ChooseCakeBtn, clickersPrice[5]));

        ChooseSugarBtn.onClick.AddListener(() => ChooseClicker(ChooseSugarBtn));
        ChooseCandyBtn.onClick.AddListener(() => ChooseClicker(ChooseCandyBtn));
        ChooseCupcakeBtn.onClick.AddListener(() => ChooseClicker(ChooseCupcakeBtn));
        ChooseChocolateBtn.onClick.AddListener(() => ChooseClicker(ChooseChocolateBtn));
        ChooseCheesecakeBtn.onClick.AddListener(() => ChooseClicker(ChooseCheesecakeBtn));
        ChooseJellyBtn.onClick.AddListener(() => ChooseClicker(ChooseJellyBtn));
        ChooseCakeBtn.onClick.AddListener(() => ChooseClicker(ChooseCakeBtn));
    }

    private void Update()
    {
        sugarT.text = sugar.ToString();
        moneyT.text = money.ToString();
    }

    private void Exchanger()
    {
        soundSource.PlayOneShot(buttonClip);
        ExchangerPanel.SetActive(!ExchangerPanel.activeSelf);
        ClickersPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    private void Clickers()
    {
        soundSource.PlayOneShot(buttonClip);
        ClickersPanel.SetActive(!ClickersPanel.activeSelf);
        ExchangerPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    private void Settings()
    {
        soundSource.PlayOneShot(buttonClip);
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
        ExchangerPanel.SetActive(false);
        ClickersPanel.SetActive(false);
    }

    private void AdjustMusic()
    {
        musicSource.volume = musicSlider.value;
    }

    private void AdjustSound()
    {
        soundSource.volume = soundSlider.value;
    }

    private void Exchange(int sugarAmount)
    {
        if (sugar >= sugarAmount)
        {
            sugar -= sugarAmount;
            money += sugarAmount * 2;

            soundSource.PlayOneShot(moneyClip);
            soundSource.PlayOneShot(buttonClip);
        }
    }

    private void BuyClicker(Button buyBtn, Button chooseBtn, int clickerPrice)
    {
        if(money >= clickerPrice)
        {
            money -= clickerPrice;

            buyBtn.gameObject.SetActive(false);
            chooseBtn.interactable = true;
            soundSource.PlayOneShot(bubbleClip);
            soundSource.PlayOneShot(buttonClip);

            ChooseClicker(chooseBtn);

            ownClickers.Add(GetClickerIndex(chooseBtn));
        }
    }

    private void ChooseClicker(Button chosenBtn)
    {
        ChooseSugarBtn.GetComponent<Image>().color = Color.white;
        ChooseCandyBtn.GetComponent<Image>().color = Color.white;
        ChooseCupcakeBtn.GetComponent<Image>().color = Color.white;
        ChooseChocolateBtn.GetComponent<Image>().color = Color.white;
        ChooseCheesecakeBtn.GetComponent<Image>().color = Color.white;
        ChooseJellyBtn.GetComponent<Image>().color = Color.white;
        ChooseCakeBtn.GetComponent<Image>().color = Color.white;

        chosenBtn.GetComponent<Image>().color = new Color(125/255f, 183/255f, 51/255f, 1);

        chosenClicker = GetClickerIndex(chosenBtn);
        ClickerBtn.GetComponent<Image>().sprite = clickerSprites[chosenClicker];
        currentProfit = sugarProfit[chosenClicker];
        soundSource.PlayOneShot(buttonClip);
    }

    private int GetClickerIndex(Button btn)
    {
        for (int i = 0; i < tags.Length; i++)
            if (tags[i] == btn.tag)
                return i;
        return 0;
    }

    private Button GetClickerButton(int index)
    {
        switch(index)
        {
            case 0:
                return ChooseSugarBtn;
            case 1:
                return ChooseCandyBtn;
            case 2:
                return ChooseCupcakeBtn;
            case 3:
                return ChooseChocolateBtn;
            case 4:
                return ChooseCheesecakeBtn;
            case 5:
                return ChooseJellyBtn;
            case 6:
                return ChooseCakeBtn;
            default:
                return ChooseSugarBtn;
        }
    }

    private SaveLoad CreateSaveGameObject()
    {
        SaveLoad save = new SaveLoad();

        save.sugar = sugar;
        save.money = money;
        save.chosenClicker = chosenClicker;
        save.musicVolume = musicSource.volume;
        save.soundVolume = soundSource.volume;
        save.ownClickers = ownClickers;
        save.currentProfit = currentProfit;

        return save;
    }

    private void SaveGame()
    {
        SaveLoad save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave");
        bf.Serialize(file, save);
        file.Close();
    }

    private void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave", FileMode.Open);
            SaveLoad save = (SaveLoad)bf.Deserialize(file);
            file.Close();

            sugar = save.sugar;
            money = save.money;
            musicSource.volume = save.musicVolume;
            soundSource.volume = save.soundVolume;
            musicSlider.value = save.musicVolume;
            soundSlider.value = save.soundVolume;
            chosenClicker = save.chosenClicker;
            ownClickers = save.ownClickers;
            currentProfit = save.currentProfit;

            ChooseClicker(GetClickerButton(chosenClicker));
            SetClickers();
        }
        else
        {
            ResetProgress();
        }
    }

    private void ResetProgress()
    {
        sugar = 0;
        money = 0;
        musicSource.volume = 0.5f;
        soundSource.volume = 1f;
        musicSlider.value = 0.5f;
        soundSlider.value = 1f;
        chosenClicker = 0;
        ownClickers = new List<int> { 0 };
        currentProfit = 1;

        ChooseClicker(ChooseSugarBtn);
        SetClickers();
    }

    private void SetClickers()
    {
        if (ownClickers.Contains(GetClickerIndex(ChooseCandyBtn)))
        {
            BuyCandyBtn.gameObject.SetActive(false);
            ChooseCandyBtn.interactable = true;
        }
        else
        {
            BuyCandyBtn.gameObject.SetActive(true);
            ChooseCandyBtn.interactable = false;
        }
        if (ownClickers.Contains(GetClickerIndex(ChooseCupcakeBtn)))
        {
            BuyCupcakeBtn.gameObject.SetActive(false);
            ChooseCupcakeBtn.interactable = true;
        }
        else
        {
            BuyCupcakeBtn.gameObject.SetActive(true);
            ChooseCupcakeBtn.interactable = false;
        }
        if (ownClickers.Contains(GetClickerIndex(ChooseChocolateBtn)))
        {
            BuyChocolateBtn.gameObject.SetActive(false);
            ChooseChocolateBtn.interactable = true;
        }
        else
        {
            BuyChocolateBtn.gameObject.SetActive(true);
            ChooseChocolateBtn.interactable = false;
        }
        if (ownClickers.Contains(GetClickerIndex(ChooseCheesecakeBtn)))
        {
            BuyCheesecakeBtn.gameObject.SetActive(false);
            ChooseCheesecakeBtn.interactable = true;
        }
        else
        {
            BuyCheesecakeBtn.gameObject.SetActive(true);
            ChooseCheesecakeBtn.interactable = false;
        }
        if (ownClickers.Contains(GetClickerIndex(ChooseJellyBtn)))
        {
            BuyJellyBtn.gameObject.SetActive(false);
            ChooseJellyBtn.interactable = true;
        }
        else
        {
            BuyJellyBtn.gameObject.SetActive(true);
            ChooseJellyBtn.interactable = false;
        }
        if (ownClickers.Contains(GetClickerIndex(ChooseCakeBtn)))
        {
            BuyCakeBtn.gameObject.SetActive(false);
            ChooseCakeBtn.interactable = true;
        }
        else
        {
            BuyCakeBtn.gameObject.SetActive(true);
            ChooseCakeBtn.interactable = false;
        }
    }

    private void OnClicker()
    {
        sugar += currentProfit;
        soundSource.PlayOneShot(bubbleClip);
        bubblesEffect.Play();
        animator.Play("Base Layer.Animation", -1, 0);
    }

    private void OnDisable()
    {
        SaveGame();
    }

}

[System.Serializable]
public class SaveLoad
{
    public int sugar, money, chosenClicker, currentProfit;
    public float soundVolume, musicVolume;
    public List<int> ownClickers;
}