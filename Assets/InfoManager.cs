using System.Globalization;
using System.IO;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;

class InfoManager : MonoBehaviour
{
    static private InfoManager instance;
    public Info playerInfo { get; private set; }
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI goldText;
    static public InfoManager Instance { get { return instance; } }

    private InfoManager()
    {
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        playerInfo = LoadJSONFromResources<Info>("info.json");
        UpdateAmount();
    }

    /// <summary>
    /// 포인트를 사용해서 골드로 변경하는 코드 (100 포인트 => 10000 골드)
    /// </summary>
    public void GoldPlus()
    {
        if(playerInfo.point >= 100)
        {
            playerInfo.point -= 100;
            playerInfo.gold += 10000;
            WriteJSONAtResources(playerInfo, "info.json");
            UpdateAmount();
        }
        else
        {
            Debug.Log("잔액이 부족합니다");
        }
    }

    public void DecreaseGold(int amount)
    {
        if (playerInfo.gold >= amount)
        {
            playerInfo.gold -= amount;
            UpdateAmount();
            WriteJSONAtResources(playerInfo, "info.json");
        }
    }

    public void IncreasePoint(int amount)
    {
        playerInfo.point += amount;
        UpdateAmount();
        WriteJSONAtResources(playerInfo, "info.json");
    }

    public void UpdateAmount()
    {
        playerText.text = playerInfo.name;
        pointText.text = playerInfo.point.ToString();
        goldText.text = playerInfo.gold.ToString();
    }

    private string ResourcePath => Path.Combine(Application.dataPath, "Resources");

    public T LoadJSONFromResources<T>(string filePath)
    {
        string fullFilePath = Path.Combine(ResourcePath, filePath);
        if(!Directory.Exists(ResourcePath))
        {
            Directory.CreateDirectory(ResourcePath);
        }
        string parsedJson = File.ReadAllText(fullFilePath);

        return JsonUtility.FromJson<T>(parsedJson);
    }

    public void WriteJSONAtResources<T>(T value, string filePath)
    {
        string fullFilePath = Path.Combine(ResourcePath, filePath);
        if (!Directory.Exists(ResourcePath))
        {
            Directory.CreateDirectory(ResourcePath);
        }
        string json = JsonUtility.ToJson(value);

        File.WriteAllText(fullFilePath, json);
    }
}
