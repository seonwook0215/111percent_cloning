using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform enemy;
    private Animator animator;
    public AudioClip loadingbow;
    public AudioClip releasebow;
    public AudioClip earthquakeAudio;

    [Header("Move Settings")]
    public float moveSpeed = 5f;

    [Header("Object Settings")]
    public Transform firePoint;
    public GameObject arrowPrefab;
    public GameObject firearrowPrefab;
    public GameObject earthquakePrefab;

    [Header("Attack Settings")]
    public float fireRate = 1f;
    public float stopDelay = 1f;

    private bool isMoving;
    public bool isRoll;
    private bool isQ;
    private float curFireTime = 0f;
    private float nextFireTime = 0f;

    [Header("HP Settings")]
    public float CurrentHP { get; private set; } = 100f;
    public float MaxHP { get; private set; } = 100f;

    [Header("Skill Settings")]
    public SkillManager skillManager;
    public float RollDistance = 5f;

    private bool isESkillActive = false;
    private float originalMoveSpeed;
    private float originalFireRate;
    private float originalAnimatorSpeed;
    private float originalRollDistance;

    private bool isCharging = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;

    }



    private void Update()
    {
        HandleAttack();
        HandleSkill();
        UpdateAnimation();
    }
    private void FixedUpdate()
    {
        Move();
    }
    public void TakeDamage(float dmg)
    {
        CurrentHP = Mathf.Max(CurrentHP - dmg, 0f);
    }
    private void HandleAttack()
    {
        if (enemy == null)
            return;
        if (!isMoving && !isRoll && !isQ)
        {
            isCharging = true;
            animator.SetBool("IsCharging", true);
        }
    }
    public void BowShootEvent()
    {
        GenerateArrow();
    }
    private void HandleSkill()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isRoll && !isQ)
            {
                isRoll = true;
                animator.SetBool("IsRoll", true);

                StartCoroutine(RollForward());

                if (isESkillActive)
                {
                    StartCoroutine(EndRollAfterDelay(0.4f));
                }
                else
                {
                    StartCoroutine(EndRollAfterDelay(0.8f));
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            skillManager.UseSkill(0, gameObject);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            skillManager.UseSkill(1, gameObject);
        }
        if (Input.GetKeyDown(KeyCode.E))
            skillManager.UseSkill(2, gameObject);
        if (Input.GetKeyDown(KeyCode.R))
            skillManager.UseSkill(3, gameObject);
    }
    public void QEvent()
    {
        GenerateFireArrow();
    }
    public void ActivateQSkill()
    {
        isQ = true;
        animator.SetBool("IsQ", true);
    }
    public void ActivateESkill(float multiplier, float duration)
    {
        if (isESkillActive) return;
        isESkillActive = true;
        originalMoveSpeed = moveSpeed;
        originalFireRate = fireRate;
        originalRollDistance = RollDistance;
        originalAnimatorSpeed = animator.speed;

        moveSpeed *= multiplier;
        fireRate /= multiplier;
        RollDistance *= multiplier;
        animator.speed *= multiplier;

        StartCoroutine(ResetESkillAfter(duration));
    }

    public void ResetQSkillAfter()
    {
        isQ = false;
        animator.SetBool("IsQ", false);
    }
    private IEnumerator ResetESkillAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        fireRate = originalFireRate;
        RollDistance = originalRollDistance;
        animator.speed = originalAnimatorSpeed;
        isESkillActive = false;
    }
    private IEnumerator EndRollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isRoll = false;
        animator.SetBool("IsRoll", false);
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
    public void LoadingBow()
    {
        AudioSource.PlayClipAtPoint(loadingbow, Camera.main.transform.position, 0.2f);

    }
    public void ReleaseBow()
    {
        AudioSource.PlayClipAtPoint(releasebow, Camera.main.transform.position, 0.1f);
    }
    private void Move()
    {
        if (isRoll || isQ)
            return;
        float playerMove = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime * (1f);
        this.transform.Translate(new Vector3(0, 0, playerMove));

        bool currentlyMoving = (playerMove != 0f);

        if (currentlyMoving && !isMoving)
        {
            curFireTime = 0f;

            isCharging = false;
            animator.SetBool("IsCharging", false);
        }
        if (!currentlyMoving && isMoving)
        {
            curFireTime = Time.time;
        }
        isMoving = currentlyMoving;


    }

    private void GenerateArrow()
    {
        if (enemy == null) return;

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        ArrowPhysics arrow = arrowObj.GetComponent<ArrowPhysics>();
        if (arrow != null)
        {
            arrow.Setup(firePoint.position, enemy.position);
        }
    }

    public void GenerateEarthquake()
    {

        if (enemy == null) return;
        EnemyAI enemy2 = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyAI>();
        AudioSource.PlayClipAtPoint(earthquakeAudio, Camera.main.transform.position, 0.6f);
        StartCoroutine(DelayedDamage(enemy2, 20f, 1f));
        Vector3 spawnPos = firePoint.position;
        spawnPos.y = -2.5f;
        Quaternion rot = Quaternion.Euler(0f, -90f, 0f);
        GameObject earthquakeObj = Instantiate(earthquakePrefab, spawnPos, rot);
        Destroy(earthquakeObj, 3f);
    }
    private IEnumerator DelayedDamage(EnemyAI target, float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
            target.TakeDamage(damage);
    }
    private void GenerateFireArrow()
    {
        if (enemy == null) return;

        GameObject firearrowObj = Instantiate(firearrowPrefab, firePoint.position, Quaternion.identity);
        ArrowLinear arrow = firearrowObj.GetComponent<ArrowLinear>();
        if (arrow != null)
        {
            arrow.Setup(firePoint.position, enemy.position);
        }
    }
    private void UpdateAnimation()
    {
        float moveX = Input.GetAxis("Horizontal");

        animator.SetFloat("MoveX", moveX);

        if (isMoving && isCharging)
        {
            isCharging = false;
            animator.SetBool("IsCharging", false);
        }

    }

}
