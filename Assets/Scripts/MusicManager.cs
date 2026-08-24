using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Si todavía no existe un MusicManager
        if (instance == null)
        {
            instance = this;

            // Evita que este objeto se destruya
            // cuando cambiamos de escena
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe uno, elimina el duplicado
            Destroy(gameObject);
        }
    }
}