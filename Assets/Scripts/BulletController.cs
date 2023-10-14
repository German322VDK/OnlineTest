using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [SerializeField]
    private float speed = 20;

    private Rigidbody2D _rb;

    private int damage = 3;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == $"{Tags.Player}")
            collision.GetComponent<PlayerController>().TakeDamage(damage);

        if (collision.tag == $"{Tags.NPC}")
            collision.GetComponent<NPCController>().TakeDamage(damage);

        Destroy(gameObject);
    }
}
