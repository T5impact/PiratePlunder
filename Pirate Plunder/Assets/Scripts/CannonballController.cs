using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballController : MonoBehaviour
{
    [SerializeField] private AudioController audioController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform debrisSpawn;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private Transform waterSplashSpawn;
    [SerializeField] private GameObject waterPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterFireMode()
    {
        animator.SetBool("Covered", true);
        audioController.PlayMetalClose();
    }

    public void ExitFireMode()
    {
        animator.SetBool("Covered", false);
        audioController.PlayMetalOpen();
    }


    public void Fire(bool hit)
    {
        audioController.PlayCannonFire();

        if (hit)
        {
            print("Hit Ship!");
            animator.SetTrigger("Fire");

            StartCoroutine(SpawnDebris());
        }
        else
        {
            print("Missed Ship!");
            animator.SetTrigger("Miss");

            StartCoroutine(SpawnWaterSplash());
        }
    }

    public IEnumerator SpawnDebris()
    {
        yield return new WaitForSeconds(0.3f);

        audioController.PlayCannonHit();

        GameObject debris = Instantiate(debrisPrefab, debrisSpawn.position, Quaternion.identity);
        Destroy(debris, 5f);
    }

    public IEnumerator SpawnWaterSplash()
    {
        yield return new WaitForSeconds(0.3f);

        audioController.PlayCannonMiss();

        GameObject water = Instantiate(waterPrefab, waterSplashSpawn.position, Quaternion.identity);
        Destroy(water, 5f);
    }
}
