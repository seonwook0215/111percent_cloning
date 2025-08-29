using UnityEngine;

public class ArrowPhysics : MonoBehaviour
{
    public float flightTime = 3f;
    public float gravity = 9.81f;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 velocity;
    private float startTime;
    private bool initialized = false;
    private bool isDamaged = false;
    public void Setup(Vector3 start, Vector3 target)
    {
        startPos = start;
        targetPos = target;
        startTime = Time.time;

        Vector3 displacement = target - start;
        float dx = displacement.x;
        float dy = displacement.y;

        // x방향 속도
        float vx = dx / flightTime;
        // y방향 속도 (중력 고려)
        float vy = (dy + 0.5f * gravity * flightTime * flightTime) / flightTime;

        velocity = new Vector3(vx, vy, 0);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        float t = Time.time - startTime;

        if (t >= flightTime)
        {
            isDamaged = true;
            transform.position = targetPos;
            Destroy(gameObject, 2f);
            return;
        }

        // 포물선 공식
        float x = velocity.x * t;
        float y = velocity.y * t - 0.5f * gravity * t * t;

        transform.position = startPos + new Vector3(x, y, 0);


        Vector2 dir = new Vector2(velocity.x, velocity.y - gravity * t);
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (!isDamaged)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
                if (player != null)
                {
                    player.TakeDamage(10);
                }
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10);
                }
                Destroy(gameObject);
            }
        }

    }
}
