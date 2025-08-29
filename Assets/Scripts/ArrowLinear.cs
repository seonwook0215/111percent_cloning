using UnityEngine;

public class ArrowLinear : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private Vector3 moveDirection;
    private float spawnTime;

    public void Setup(Vector3 start, Vector3 target)
    {
        transform.position = start;
        target.y = start.y;
        moveDirection = (target - start).normalized;
        spawnTime = Time.time;

        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
    }

    private void Update()
    {
        // 이동
        transform.position += moveDirection * speed * Time.deltaTime;

        // 5초 후 파괴
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
            if (player != null)
            {
                if (!player.isRoll) {
                    player.TakeDamage(8);
                    Destroy(gameObject); 
                }
                
            }

        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                if (!enemy.E_isRoll)
                {
                    enemy.TakeDamage(8);
                    Destroy(gameObject);
                }

            }

        }
    }


}
