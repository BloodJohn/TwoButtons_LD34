using HECS.Systems;
using UnityEngine;

[DefaultExecutionOrder(-5000)]
public class GameController : MonoBehaviour
{
    private EntityManager entityManager;
    private SaveManager saveManager;

    private void Awake()
    {
        entityManager = new EntityManager();
        saveManager = new SaveManager();
    }

    private void OnDestroy()
    {
        entityManager.Dispose();
    }
}