using UnityEngine;

public class TriggerBos : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    public string musicEvent;

    private EnemyController _enemyController;


    // Start is called before the first frame update
    void Start()
    {
        _enemyController = enemy.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == $"{Tags.Player}")
        {
            _enemyController.IsTriggered = true;
        }
    }

    private void OnDestroy()
    {
    }
}
