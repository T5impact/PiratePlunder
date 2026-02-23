using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    [System.Serializable]
    public struct PayTableRow
    {
        public int[] amounts;
    }

    [SerializeField] private PayTableRow[] payTable;
    [SerializeField] private Reel[] reels;
    [SerializeField] private SlotUIController uiController;
    [SerializeField] private PirateshipController pirateshipController;
    [SerializeField] private CannonballController cannonballController;
    [SerializeField] private int defaultBetAmount = 1;
    [SerializeField] private int shipDestroyedPayout = 250;

    [Header("Gold")]
    [SerializeField] private Transform normalGoldSpawn;
    [SerializeField] private GameObject goldSmall;
    [SerializeField] private GameObject goldMedium;
    [SerializeField] private GameObject goldLarge;
    [SerializeField] private Transform grandGoldSpawn;
    [SerializeField] private GameObject grandGold;

    [Header("Preset Winnings")]
    [SerializeField] int[] lemonPreset;
    [SerializeField] int[] barPreset;
    [SerializeField] int[] sevenPreset;

    [Header("Cheat")]
    [SerializeField] GameObject cheatMenu;

    int payoutAmount;
    int cashAmount;

    int actualSpentAmount;

    int betAmount;
    int multiplier = 1;

    int cannonballCount;

    bool spinningReels = false;
    bool payingOut = false;
    bool cashingOut = false;

    bool addingCannonballs = false;
    bool fireMode = false;
    bool firing = false;

    // Start is called before the first frame update
    void Start()
    {
        Reset();

        CalculateTheoreticalRTP();
    }

    private void OnEnable()
    {
        PirateshipController.pirateArrived += TryEnterFireMode;
        PirateshipController.pirateLeft += TryExitFireMode;
        PirateshipController.pirateDestroyed += PirateshipDestroyed;
    }

    private void Reset()
    {
        payoutAmount = 0;
        uiController.SetPayoutText(payoutAmount);

        cashAmount = 0;
        uiController.SetCashText(cashAmount);

        multiplier = 1;
        uiController.SetMultiplier(multiplier);

        betAmount = defaultBetAmount * multiplier;
        uiController.SetBetText(betAmount);
    }

    public void IncreaseCash()
    {
        cashAmount += betAmount;
        uiController.SetCashText(cashAmount);
    }

    public void SetMultiplier(int multiplier)
    {
        if (!spinningReels && !payingOut && !cashingOut && !addingCannonballs && !fireMode)
        {
            this.multiplier = Mathf.Max(1, multiplier);

            uiController.SetMultiplier(this.multiplier);

            betAmount = defaultBetAmount * multiplier;
            uiController.SetBetText(betAmount);

            uiController.SetMinorPayoutTexts(payTable, this.multiplier);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpinReel();
        }
    }

    public void TryEnterFireMode()
    {
        if(pirateshipController.SpawnedIn && cannonballCount > 0 && !spinningReels)
        {
            fireMode = true;

            uiController.ShowFireButton();

            cannonballController.EnterFireMode();
        }
    }

    public void TryExitFireMode()
    {
        if (fireMode)
        {
            fireMode = false;

            uiController.HideFireButton();

            cannonballController.ExitFireMode();
        }
    }

    public void PirateshipDestroyed()
    {
        TryExitFireMode();

        print("Pirateship destroyed!");

        print($"Payout: {shipDestroyedPayout}");

        StartCoroutine(Payout(shipDestroyedPayout, 3));
    }

    public void StartSpinReel()
    {
        if (cashAmount >= betAmount && !spinningReels && !payingOut && !cashingOut && !fireMode && !addingCannonballs)
        {
            cashAmount -= betAmount;
            actualSpentAmount += betAmount;
            uiController.SetCashText(cashAmount);

            StartCoroutine(SpinReel());
        }
    }

    public IEnumerator SpinReel(int[] preset = null)
    {
        spinningReels = true;

        if (preset != null)
        {
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].StartRotate(setEndIndex: preset[i]);
                yield return new WaitForSeconds(0.25f);
            }
        }
        else
        {
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].StartRotate();
                yield return new WaitForSeconds(0.25f);
            }
        }

        yield return new WaitUntil(() =>
        {
            for (int i = 0; i < reels.Length; i++)
            {
                if (reels[i].spinningReel)
                {
                    return false;
                }
            }
            return true;
        }
        );

        List<Reel> cannonballReels = new List<Reel>();

        int[] counts = new int[3];
        for (int i = 0; i < reels.Length; i++)
        {
            counts[(int)reels[i].GetCurrentReelPosition().ReelPositionType] += 1;

            if(reels[i].GetCurrentReelPosition().ReelPositionType == ReelPositionType.Seven)
            {
                cannonballReels.Add(reels[i]);
            }
        }

        int payout = 0;
        int type = -1;
        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] > 0)
            {
                payout += payTable[i].amounts[counts[i] - 1] * multiplier;
                if(payout > 0)
                {
                    type = i;
                }
            }
        }

        print($"Payout: {payout}");

        StartCoroutine(Payout(payout, type));

        //Skip a frame
        yield return null;

        yield return new WaitUntil(() => payingOut == false);

        if(cannonballReels.Count > 0)
        {
            StartCoroutine(AddCannonballs(cannonballReels.ToArray()));
        }

        //Skip a frame
        yield return null;

        yield return new WaitUntil(() => addingCannonballs == false);

        //payoutAmount += payout;
        //uiController.SetPayoutText(payoutAmount);

        spinningReels = false;

        TryEnterFireMode();

    }

    public IEnumerator Payout(int payout, int type = -1)
    {
        print($"Type: {type}");
        payingOut = true;

        switch(type)
        {
            case 0:
                GameObject gold = Instantiate(goldLarge, normalGoldSpawn.position, Quaternion.identity);
                Destroy(gold, 10f);
                break;
            case 1:
                gold = Instantiate(goldMedium, normalGoldSpawn.position, Quaternion.identity);
                Destroy(gold, 10f);
                break;
            case 2:
                gold = Instantiate(goldSmall, normalGoldSpawn.position, Quaternion.identity);
                Destroy(gold, 10f);
                break;
            case 3:
                gold = Instantiate(grandGold, grandGoldSpawn.position, Quaternion.identity);
                Destroy(gold, 10f);
                break;
        }

        float payoutMultiplier = Mathf.Max(1f, payout / 50f);

        float payoutStep = 0.05f * payoutMultiplier;
        float numOfSteps = payout / payoutStep;
        WaitForSeconds interval = new WaitForSeconds(0.01f);

        for (int i = 0; i < numOfSteps; i++)
        {
            uiController.SetPayoutText(payoutAmount + payoutStep * i);

            yield return interval;
        }

        payoutAmount += payout;
        uiController.SetPayoutText(payoutAmount);

        payingOut = false;
    }

    public IEnumerator AddCannonballs(Reel[] cannonballReels)
    {
        addingCannonballs = true;

        WaitForSeconds interval = new WaitForSeconds(0.5f);

        for(int i = 0; i < cannonballReels.Length; i++)
        {
            if (cannonballReels.Length == 3)
            {
                //Do special effect on reel
                for (int j = 0; j < 3; j++)
                {
                    cannonballReels[i].PlayHighlightParticle();

                    cannonballCount++;
                    uiController.SetCannonBallCount(cannonballCount);
                    yield return interval;
                }
            }
            else
            {
                //Do effect on reel
                cannonballReels[i].PlayHighlightParticle();

                cannonballCount++;
                uiController.SetCannonBallCount(cannonballCount);

                yield return interval;
            }
        }

        addingCannonballs = false;
    }

    public void StartFireCannonball()
    {
        if (!firing)
        {
            StartCoroutine(FireCannonball());
        }
    }

    public IEnumerator FireCannonball()
    {
        firing = true;

        cannonballCount--;
        uiController.SetCannonBallCount(cannonballCount);

        
        bool hit = pirateshipController.CheckForHit();

        cannonballController.Fire(hit);

        if (cannonballCount == 0)
        {
            TryExitFireMode();
        }

        yield return new WaitForSeconds(1.5f);

        firing = false;
    }

    public void StartCashout()
    {
        StartCoroutine(Cashout());
    }
    public IEnumerator Cashout()
    {
        cannonballCount = 0;
        uiController.SetCannonBallCount(cannonballCount);

        pirateshipController.ShipReset();

        cashingOut = true;

        float cashoutMultiplier = Mathf.Max(1f, payoutAmount / 50f);

        float cashoutStep = 0.05f * cashoutMultiplier;
        float numOfSteps = payoutAmount / cashoutStep;
        WaitForSeconds interval = new WaitForSeconds(0.005f);

        for (int i = 0; i < numOfSteps; i++)
        {
            uiController.SetPayoutText(payoutAmount - cashoutStep * i);

            yield return interval;
        }

        payoutAmount = 0;
        uiController.SetPayoutText(payoutAmount);

        cashoutMultiplier = Mathf.Max(1f, cashAmount / 50f);
        cashoutStep = 0.05f * cashoutMultiplier;
        numOfSteps = cashAmount / cashoutStep;
        for (int i = 0; i < numOfSteps; i++)
        {
            uiController.SetCashText(cashAmount - cashoutStep * i);

            yield return interval;
        }

        Reset();



        cashingOut = false;
    }

    public void StartSevenPreset()
    {
        if (cashAmount >= betAmount && !spinningReels && !payingOut && !cashingOut && !fireMode && !addingCannonballs)
        {
            cashAmount -= betAmount;
            uiController.SetCashText(cashAmount);

            StartCoroutine(SpinReel(sevenPreset));
        }
    }
    public void StartBarPreset()
    {
        if (cashAmount >= betAmount && !spinningReels && !payingOut && !cashingOut && !fireMode && !addingCannonballs)
        {
            cashAmount -= betAmount;
            uiController.SetCashText(cashAmount);

            StartCoroutine(SpinReel(barPreset));
        }
    }
    public void StartLemonPreset()
    {
        if (cashAmount >= betAmount && !spinningReels && !payingOut && !cashingOut && !fireMode && !addingCannonballs)
        {
            cashAmount -= betAmount;
            uiController.SetCashText(cashAmount);

            StartCoroutine(SpinReel(lemonPreset));
        }
    }

    public void ToggleCheatMenu()
    {
        cheatMenu.SetActive(!cheatMenu.activeSelf);
    }

    public void CalculateTheoreticalRTP()
    {
        int betAmount = 1;
        int multiplier = 1;
        int totalSpent = 0;
        int winningAmount = 0;

        ReelPosition[] reelPositions = new ReelPosition[3];


        for (int i = 0; i < 20000000; i++)
        {
            totalSpent += betAmount * multiplier;

            for (int j = 0; j < reels.Length; j++)
            {
                reelPositions[j] = reels[j].GetRandonPosition();
            }

            int[] counts = new int[3];
            for (int j = 0; j < reelPositions.Length; j++)
            {
                counts[(int)reelPositions[j].ReelPositionType] += 1;
            }

            int payout = 0;
            for (int j = 0; j < counts.Length; j++)
            {
                if (counts[j] > 0)
                {
                    payout += payTable[j].amounts[counts[j] - 1] * multiplier;
                }
            }

            //print($"Payout: {payout}");
            winningAmount += payout;
        }

        print((float)totalSpent / winningAmount);
    }

    public void OnDisable()
    {
        PirateshipController.pirateArrived -= TryEnterFireMode;
        PirateshipController.pirateDestroyed -= PirateshipDestroyed;
        PirateshipController.pirateLeft -= TryExitFireMode;
    }
}
