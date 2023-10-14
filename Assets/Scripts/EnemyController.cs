using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour 
{
    public bool IsTriggered { get; set; } = false;

    [SerializeField]
    private float speed = 80f;

    [SerializeField]
    private float jumpForce = 30f;

    [SerializeField]
    private float healthPoint = 250f;

    [SerializeField]
    private Image _bar;
    [SerializeField]
    private Image _upBar;

    [SerializeField]
    private Transform _pointAttack11;
    [SerializeField]
    private Transform _pointAttack12;
    [SerializeField]
    private Transform _pointAttack2;

    [SerializeField]
    private Transform[] _pointAttacks3;

    [SerializeField]
    private GameObject fireBall;
    [SerializeField]
    private GameObject player;

    private GameObject upHPBar;
    private Rigidbody2D _rb;
    private SpriteRenderer _spite;

    private bool _endabledAttack = true;
    private bool raged = false;
    private int _attack2Count = 3;

    private int[] attacks = new int[] { 2, 1, 2, 3, 1, 2, 3, 2, 1, 3, 2, 3, 1, 3, 1, 2, 3, 1, 3, 2, 1 };
    int n = 0;

    private void OpenBorder()
    {
        GameObject border = GameObject.Find("OpenBorder");
        Vector3 position = border.transform.position;
        border.transform.position = new Vector3(position.x, (float)5.61, position.z);
    }

    private void CloseBorder()
    {
        GameObject border = GameObject.Find("CloseBorder");
        Vector3 position = border.transform.position;
        border.transform.position = new Vector3(position.x, (float)2.7, position.z);
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        upHPBar = GameObject.Find("BossHPBar");
        upHPBar.SetActive(false);
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (IsTriggered && n == 0)
        {
            upHPBar.SetActive(true);
            CloseBorder();
            StartCoroutine(Attack());
            n++;
        }
    }


    public void TakeDamage(int damage)
    {
        healthPoint -= damage;

        if (healthPoint <= 0)
        {
            Destroy(gameObject);
            upHPBar.SetActive(false);
            OpenBorder();
        }
        else
        {
            _bar.fillAmount = healthPoint / 100;
            _upBar.fillAmount = healthPoint / 100;
        }

    }

    private IEnumerator Attack()
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            switch (attacks[i])
            {
                case 1:
                    yield return StartCoroutine(Attak1());
                    break;
                case 2:
                    yield return StartCoroutine(Attak2());
                    break;
                case 3:
                    yield return StartCoroutine(Attak3());
                    break;
            }
        }
    }

    private IEnumerator Attak1()
    {
        _endabledAttack = false;
        raged = true;

        var plPos = player.transform.position;

        var forV = new Vector3(_pointAttack11.position.x, plPos.y, plPos.z);

        while (Vector2.Distance(transform.position, forV) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, forV, speed * Time.deltaTime);
            yield return null;
        }

        while (Mathf.Abs(transform.position.x - _pointAttack12.position.x) > 1f)
        {
            var newPos = new Vector3(_pointAttack12.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
            yield return null;
        }

        raged = false;
        yield return new WaitForSeconds(1);
        _endabledAttack = true;
    }

    private IEnumerator Attak2()
    {
        _endabledAttack = false;

        _attack2Count = 3;
        while (Vector2.Distance(transform.position, _pointAttack2.position) > 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _pointAttack2.position, speed * Time.deltaTime * 4);
            yield return null;
        }

        yield return new WaitWhile(() => Vector2.Distance(transform.position, _pointAttack2.position) > 0.2f);

        for (int i = 0; i <= 2; i++)
            Invoke("Salvo", i);

        yield return new WaitWhile(() => _attack2Count > 0);
        yield return new WaitForSeconds(1);
        _endabledAttack = true;
    }

    private IEnumerator Attak3()
    {
        _endabledAttack = false;
        var newSpeed = speed;
        for (int i = 0; i <= _pointAttacks3.Length; i++)
        {
            Vector3 vector;
            if (i == _pointAttacks3.Length)
                vector = _pointAttacks3[0].position;
            else
                vector = _pointAttacks3[i].position;

            if (i == 1)
                newSpeed /= 2;

            while (Vector2.Distance(transform.position, vector) > 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, vector, newSpeed * Time.deltaTime);
                yield return null;
            }

            var g = Instantiate(fireBall, transform.position, transform.rotation);
            var gScr = g.GetComponent<FireBallContoller>();
            var gRB = g.GetComponent<Rigidbody2D>();

            gRB.velocity = (player.transform.position - transform.position) * gScr.speed / 3;

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1);
        _endabledAttack = true;
    }

    private void Salvo()
    {
        if (_attack2Count > 0)
        {
            for (float i = 0; i < 360; i += 15)
            {
                var ang = ToRad(i);
                Vector2 v = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
                var g = Instantiate(fireBall, transform.position, transform.rotation);
                var gScr = g.GetComponent<FireBallContoller>();
                var gRB = g.GetComponent<Rigidbody2D>();

                gRB.velocity = v * gScr.speed;
            }

            _attack2Count--;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == $"{Tags.Player}")
        {
            int damage;

            if (raged)
                damage = 20;
            else
                damage = 10;

            collision.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    private float ToRad(float val) => val * Mathf.PI / 180;

}
