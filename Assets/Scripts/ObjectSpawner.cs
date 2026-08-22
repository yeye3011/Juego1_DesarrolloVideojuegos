using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Objetos disponibles")]
    public GameObject[] fallingObjectPrefabs;

    [Header("Tiempo entre objetos")]
    public float spawnInterval = 1.5f;

    [Header("Zona de aparición")]
    public float minX = -4f;
    public float maxX = 4f;

    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    void SpawnObject()
    {
        if (fallingObjectPrefabs.Length == 0)
        {
            return;
        }

        // Elegir un objeto aleatoriamente
        int randomIndex = Random.Range(
            0,
            fallingObjectPrefabs.Length
        );

        GameObject selectedPrefab =
            fallingObjectPrefabs[randomIndex];

        // Elegir posición horizontal aleatoria
        float randomX = Random.Range(minX, maxX);

        Vector3 spawnPosition = new Vector3(
            randomX,
            transform.position.y,
            transform.position.z
        );

        Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}