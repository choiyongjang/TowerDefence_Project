using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UI_LobbyScene : MonoBehaviour
{
    public Button Setting;
    public Button Title;
    public Button TowerLoadout;
    public Button Backpack;
    public Button Talents;
    public GameObject SettingPopup;
    public GameObject TowerLoadoutPopup;
    public GameObject BackpackPopup;
    //public GameObject TalentsPopup;
    public TextMeshProUGUI EmberAmountText;
    public TextMeshProUGUI ExpAmountText;
    public TextMeshProUGUI HpAmountText;
    public GameObject EmptyCard;
    public Transform TowerCardLocation;

    private void Awake()
    {
        Setting.onClick.AddListener(PopupSetting);
        Title.onClick.AddListener(BackToTitle);
        TowerLoadout.onClick.AddListener(PopupTowerLoadout);
        Backpack.onClick.AddListener(PopupBackpack);
        //Talents.onClick.AddListener(PopupTalents);
    }

    private void Start()
    {
        SoundManager.Instance.Play("MBGM", SoundManager.Sound.Bgm);
        TowerCard();
    }

    private void Update()
    {
        EmberAmountText.text = $"{GameManager.Instance.CurrentEmber}";
        ExpAmountText.text = $"{GameManager.Instance.CurrentExp}";
        HpAmountText.text = $"{GameManager.Instance.CurrentHp}/15";
    }

    private void PopupSetting()
    {
        SettingPopup.SetActive(true);
    }
    private void BackToTitle()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        SceneManager.LoadScene("TitleScene");
    }
    private void PopupTowerLoadout()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        TowerLoadoutPopup.SetActive(true);
    }
    private void PopupBackpack()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        BackpackPopup.SetActive(true);
    }
    //private void PopupTalents()
    //{
    //    TalentsPopup.SetActive(true);
    //}

    public List<GameObject> towerCards = new List<GameObject>();

    public void TowerCard()
    {
        foreach (var item in towerCards)
        {
            Destroy(item.gameObject); 
        }
        towerCards = new List<GameObject>();
        for (int i = 0; i<8; i++)
        {

            if (GameManager.Instance.EquipTowerList[i] == ""||GameManager.Instance.EquipTowerList[i]==null)
            {
                towerCards.Add(Instantiate(EmptyCard, TowerCardLocation));
            }
            else
            {
                towerCards.Add((GameObject)Instantiate(Resources.Load($"AcademyTowerCard/{GameManager.Instance.EquipTowerList[i]}"), TowerCardLocation));
            }
            
        }
    }
} 
