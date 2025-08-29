using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Object Settings")]
    public Transform firePoint;
    public GameObject arrowPrefab;
    public GameObject firearrowPrefab;
    public GameObject earthquakePrefab;

    public bool E_isRoll;
    [Header("HP Settings")]
    public float CurrentHP { get; private set; } = 100f;
    public float MaxHP { get; private set; } = 100f;

    public float RollDistance = 5f;

    private Transform player;
    private Animator animator;
    private bool isActing = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        StartCoroutine(AIBehaviourLoop());
    }


    private void Update()
    {
        UpdateAnimation();
    }
    private IEnumerator AIBehaviourLoop()
    {
        while (true)
        {
            if (!isActing)
            {
                int action = Random.Range(0, 4);
                switch (action)
                {
                    case 0:
                        yield return StartCoroutine(MoveForSeconds(-1, 1f));
                        break;

                    case 1:
                        yield return StartCoroutine(MoveForSeconds(1, 1f));
                        break;

                    case 2:
                        AttackForSeconds();
                        yield return new WaitForSeconds(0.3f);
                        break;

                    case 3:
                        Roll();
                        yield return new WaitForSeconds(0.3f);
                        break;

                    case 4:
                        AttackForSeconds();
                        yield return new WaitForSeconds(0.3f);
                        //UseRandomSkill();
                        break;
                }
            }
            yield return null;
        }
    }
    public void TakeDamage(float dmg)
    {
        CurrentHP = Mathf.Max(CurrentHP - dmg, 0f);
    }
    public void BowShootEvent()
    {
        GenerateArrow();

    }
    private void GenerateArrow()
    {
        if (player== null) return;

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        ArrowPhysics arrow = arrowObj.GetComponent<ArrowPhysics>();
        if (arrow != null)
        {
            arrow.Setup(firePoint.position, player.position);
        }
    }
    public void IdleEvent()
    {
        isActing = false;
        animator.SetBool("E_IsCharging", false);
        animator.SetBool("E_IsRoll", false);
        E_isRoll = false;

    }
    private IEnumerator MoveForSeconds(int direction, float time)
    {
        isActing = true;
        float elapsed = 0f;

        animator.SetFloat("E_MoveX", direction);

            while (elapsed < time)
            {
                transform.Translate(new Vector3(0, 0, direction * moveSpeed * Time.deltaTime));
                elapsed += Time.deltaTime;
                yield return null;
            }

        animator.SetFloat("E_MoveX", 0f);
    }
    private void AttackForSeconds()
    {
        isActing = true;
        animator.SetBool("E_IsCharging", true);
    }
    private void Roll()
    {
        E_isRoll = true;
        animator.SetBool("E_IsRoll", true);

        StartCoroutine(RollForward());
    }
    private IEnumerator RollForward()
    {
        Vector3 startPos = transform.position;
        Vector3 dir = transform.forward;
        Vector3 targetPos = startPos + transform.forward * RollDistance;
        float elapsed = 0f;
        float duration = 0.8f;
        RaycastHit hit;
        if (Physics.Raycast(startPos, dir, out hit, RollDistance, LayerMask.GetMask("Wall")))
        {
            targetPos = hit.point;
        }
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
    private void UseRandomSkill()
    {
        //if (!playerMove) return;

        //int skillIndex = Random.Range(0, 4); // Q=0, W=1, E=2, R=3
        //playerMove.skillManager.UseSkill(skillIndex, gameObject);
    }

    private void UpdateAnimation()
    {

        if (isActing)
        {
            //isCharging = false;
            //animator.SetBool("IsCharging", false);
        }

    }
}
