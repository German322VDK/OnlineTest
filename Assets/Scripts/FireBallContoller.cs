using UnityEngine;

public class FireBallContoller : MonoBehaviour
{
    public float speed = 2.5f; 

    [SerializeField]
    private GameObject enemy;

    private Rigidbody2D _rb;

    private int damage = 10;

    private float timeDestroy = 5f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, timeDestroy);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == $"{Tags.Player}")
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
