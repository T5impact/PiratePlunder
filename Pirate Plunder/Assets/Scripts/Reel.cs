using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour
{
    [SerializeField] private Transform reelTransform;
    [SerializeField] private ReelPosition[] reelPositions;
    [SerializeField] private int positionPadding = 1;
    [SerializeField] private float positionHeight = 5f;
    [SerializeField] private float positionSpacing = 0.25f;
    [SerializeField] private int minRandSpin = 7;
    [SerializeField] private int maxRandSpin = 10;
    [SerializeField] private float reelSpeed = 3f;
    [SerializeField] private AnimationCurve reelSpeedCurve;
    [SerializeField] private ParticleSystem highlightParticle;

    public int reelPositionIndex { get; private set; }

    public bool spinningReel { get; private set; }

    public ReelPosition GetCurrentReelPosition()
    {
        return reelPositions[reelPositionIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < reelPositions.Length; i++)
        {
            print(reelPositions[i]);
            Vector3 offset = Vector3.down * (positionHeight + positionSpacing) * i;
            Instantiate(reelPositions[i].gameObject, reelTransform.position + offset, Quaternion.identity, reelTransform);
        }

        //After padding
        for (int i = 0; i < positionPadding; i++)
        {
            Vector3 offset = Vector3.down * (positionHeight + positionSpacing) * (i + reelPositions.Length);
            Instantiate(reelPositions[i].gameObject, reelTransform.position + offset, Quaternion.identity, reelTransform);
        }

        //Before padding
        for (int i = 0; i < positionPadding; i++)
        {
            Vector3 offset = Vector3.up * (positionHeight + positionSpacing) * (i + 1);
            Instantiate(reelPositions[reelPositions.Length - 1 - i].gameObject, reelTransform.position + offset, Quaternion.identity, reelTransform);
        }

        //StartCoroutine(Rotate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRotate(int randSpinMin = -1, int randSpinMax = -1, int setEndIndex = -1)
    {
        StartCoroutine(Rotate(randSpinMin == -1 ? minRandSpin : randSpinMin, randSpinMax == -1 ? maxRandSpin : randSpinMax, setEndIndex));
    }

    IEnumerator Rotate(int randSpinMin = 7, int randSpinMax = 10, int setEndIndex = -1)
    {
        spinningReel = true;

        int endPositionIndex = setEndIndex == -1 ? Random.Range(0, reelPositions.Length) : setEndIndex;
        print($"{name}_EndPositionIndex: {endPositionIndex}");

        reelPositionIndex = endPositionIndex;

        int randSpinAmount = Random.Range(randSpinMin, randSpinMax);
        print($"{name}_RandomSpinAmount: {randSpinAmount}");

        float positionOffset = (positionHeight + positionSpacing) * endPositionIndex + (positionHeight + positionSpacing) * (reelPositions.Length * randSpinAmount) - reelTransform.localPosition.y;
        float currentOffset = 0;
        //print($"Position Offset: {positionOffset}");

        Vector3 endPosition = Vector3.down * positionOffset;

        while(currentOffset < positionOffset)
        {
            yield return null;

            float speed = reelSpeed * reelSpeedCurve.Evaluate(currentOffset / positionOffset);

            float delta = speed * Time.deltaTime;

            reelTransform.localPosition += Vector3.up * delta;
            currentOffset += delta;

            CheckForReelReset();
        }

        reelTransform.localPosition = Vector3.up * (positionHeight + positionSpacing) * endPositionIndex;

        spinningReel = false;
    }

    public ReelPosition GetRandonPosition()
    {
        int endReelIndex = Random.Range(0, reelPositions.Length);
        return reelPositions[endReelIndex];
    }

    void CheckForReelReset()
    {
        float yPos = reelTransform.localPosition.y;

        float maxPos = (positionHeight + positionSpacing) * (reelPositions.Length);
        //print($"Max Position: {maxPos} - yPos: {yPos}");
        if (yPos >= maxPos)
        {
            reelTransform.localPosition = Vector3.up * (yPos - maxPos);
        }
    }

    public void PlayHighlightParticle()
    {
        highlightParticle.Play();
    }
}
