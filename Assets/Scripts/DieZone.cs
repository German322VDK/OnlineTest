using UnityEngine;

public class DieZone : MonoBehaviour
{
    [SerializeField]
    private GameObject respawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == $"{Tags.Player}")
        {
            //make funk?
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            collision.transform.position = respawn.transform.position;
        }
    }
}
