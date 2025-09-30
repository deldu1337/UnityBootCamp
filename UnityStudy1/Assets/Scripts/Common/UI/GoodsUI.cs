using TMPro;
using UnityEngine;

public class GoodsUI : MonoBehaviour
{
    public TextMeshProUGUI GoldAmountText;
    public TextMeshProUGUI GemAmountText;

    public void SetValues()
    {
        UserGoodsData userGoodsData = UserDataManager.Instance.GetUserData<UserGoodsData>();
        if (userGoodsData == null)
        {
            Logger.LogError("No user Goods Data");
        }
        else
        {
            GoldAmountText.text = userGoodsData.Gold.ToString("N0"); //1000 => 1,000
            GemAmountText.text = userGoodsData.Gem.ToString("N0");
        }
    }
}
