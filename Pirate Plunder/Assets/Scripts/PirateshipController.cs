using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateshipController : MonoBehaviour
{
    public static event Action pirateArrived;
    public static event Action pirateLeft;
    public static event Action pirateDestroyed;

    private int currentLife;

    private float currentHitChance;

    private bool spawning;
    private bool despawning;
    private bool spawned;

    private bool destroyed;
    public bool SpawnedIn { get => spawned; }

    private Animator animator;

    [SerializeField] private int lifeMin;
    [SerializeField] private int lifeMax;
    [SerializeField] private float spawnDurationMin;
    [SerializeField] private float spawnDurationMax;
    [SerializeField] private float spawnDelayMin;
    [SerializeField] private float spawnDelayMax;
    [SerializeField] private float minHitChance;
    [SerializeField] private float maxHitChance;
    [SerializeField] private float respawnDurationMin;
    [SerializeField] private float respawnDurationMax;

    Coroutine spawnDuration;
    Coroutine spawnDelay;
    Coroutine spawnIn;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = UnityEngine.Random.Range(lifeMin, lifeMax);

        animator = GetComponent<Animator>();

        spawnDelay = StartCoroutine(SpawnDelay(UnityEngine.Random.Range(spawnDelayMin, spawnDelayMax)));
    }

    public void ShipReset()
    {
        currentLife = UnityEngine.Random.Range(lifeMin, lifeMax);
    }

    public int GetRandomLife()
    {
        return UnityEngine.Random.Range(lifeMin, lifeMax);
    }

    public float GetRandomHitChance()
    {
        return UnityEngine.Random.Range(minHitChance, maxHitChance);
    }

    public IEnumerator SpawnDelay(float timer)
    {
        yield return new WaitForSeconds(timer);

        if (spawnIn != null)
        {
            StopCoroutine(spawnIn);
        }
        spawnIn = StartCoroutine(SpawnIn());
    }

    public IEnumerator SpawnIn()
    {
        currentHitChance = UnityEngine.Random.Range(minHitChance, maxHitChance);

        spawning = true;

        animator.SetTrigger("Spawn");
        yield return new WaitForSeconds(5f);

        spawning = false;


        float duration = UnityEngine.Random.Range(spawnDurationMin, spawnDurationMax);

        if(spawnDuration != null)
        {
            StopCoroutine(spawnDuration);
        }
        spawnDuration = StartCoroutine(SpawnDuration(duration));
    }

    public IEnumerator SpawnDuration(float timer)
    {
        spawned = true;

        pirateArrived?.Invoke();

        yield return new WaitForSeconds(timer);

        spawned = false;

        despawning = true;

        pirateLeft?.Invoke();

        animator.SetTrigger("Despawn");
        yield return new WaitForSeconds(5f);

        despawning = false;

        float delay = UnityEngine.Random.Range(spawnDelayMin, spawnDelayMax);

        if (spawnDelay != null) 
        { 
            StopCoroutine(spawnDelay);
        }
        spawnDelay = StartCoroutine(SpawnDelay(delay));
    }

    public bool CheckForHit()
    {
        if (!spawned) return false;

        float hit = UnityEngine.Random.Range(0f, 1f);
        if (hit < currentHitChance)
        {
            //Hit!
            currentLife--;

            if(currentLife == 0)
            {
                StartCoroutine(DestroyShip());
            }

            return true;
        }
        else
        {
            //Miss!
            return false;
        }
    }

    public void StartDestroyShip()
    {
        if(!destroyed)
        {
            StartCoroutine(DestroyShip());
        }
    }

    public IEnumerator DestroyShip()
    {
        spawned = false;

        if(spawnDuration != null)
        {
            StopCoroutine(spawnDuration);
        }
        if (spawnDelay != null)
        {
            StopCoroutine(spawnDelay);
        }
        if (spawnIn != null)
        {
            StopCoroutine(spawnIn);
        }

        yield return new WaitForSeconds(0.4f);

        pirateDestroyed?.Invoke();

        destroyed = true;

        animator.SetTrigger("Destroy");

        float respawn = UnityEngine.Random.Range(respawnDurationMin, respawnDurationMax);

        yield return new WaitForSeconds(respawn);

        destroyed = false;

        currentLife = UnityEngine.Random.Range(lifeMin, lifeMax);

        float delay = UnityEngine.Random.Range(spawnDelayMin, spawnDelayMax);

        if (spawnDelay != null)
        {
            StopCoroutine(spawnDelay);
        }
        spawnDelay = StartCoroutine(SpawnDelay(delay));
    }
}
