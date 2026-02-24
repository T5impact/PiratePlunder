using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text trtpText;
    [SerializeField] private TMP_Text artpText;
    [SerializeField] private TMP_Text payoutText;
    [SerializeField] private TMP_Text cashText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text cannonballText;
    [SerializeField] private GameObject cannonballCount;
    [SerializeField] private GameObject fireButton;
    [SerializeField] private TMP_Text[] minorPayoutTexts;
    [SerializeField] private Image[] multiplierImages;
    [SerializeField] private Color activeMultiplierColor = Color.red;
    [SerializeField] private Color defaultMultiplierColor = Color.white;

    public void SetPayoutText(float amount)
    {
        if (amount > 0)
        {
            payoutText.enabled = true;
            payoutText.text = $"${amount.ToString("0.00")}";
        }
        else
        {
            payoutText.enabled = false;
        }
    }

    public void SetTRTP(float rtp)
    {
        trtpText.text = $"T-RTP: {rtp.ToString("0.00")}";
    }
    public void SetARTP(float rtp)
    {
        artpText.text = $"A-RTP: {rtp.ToString("0.00")}";
    }

    public void SetMinorPayoutTexts(SlotController.PayTableRow[] payTable, int mulitplier = 1)
    {
        for (int i = 0; i < payTable.Length; i++)
        {
            minorPayoutTexts[i].text = $"= ${payTable[i].amounts[payTable[i].amounts.Length - 1] * mulitplier}";
        }
    }

    public void SetCashText(float amount)
    {
        cashText.text = $"${amount.ToString("0.00")}";
    }

    public void SetBetText(int amount)
    {
        betText.text = $"${amount.ToString("0.00")}";
    }

    // Start is called before the first frame update
    void Awake()
    {
        payoutText.text = $"$0.00";
        payoutText.enabled = false;
        cashText.text = $"$0.00";
        betText.text = $"$0.00";

        fireButton.SetActive(false);
        cannonballCount.SetActive(false);
    }

    public void SetMultiplier(int multiplier)
    {
        for (int i = 0; i < multiplierImages.Length; i++)
        {
            if(i == multiplier - 1)
            {
                multiplierImages[i].color = activeMultiplierColor;
            }
            else
            {
                multiplierImages[i].color = defaultMultiplierColor;
            }
        }
    }

    public void ShowFireButton()
    {
        fireButton.SetActive(true);
    }
    public void HideFireButton()
    {
        fireButton.SetActive(false);
    }

    public void SetCannonBallCount(int count)
    {
        cannonballText.text = $"x{count.ToString()}";
        if(count == 0)
        {
            cannonballCount.SetActive(false);
        }
        else
        {
            cannonballCount.SetActive(true);
        }
    }
}
