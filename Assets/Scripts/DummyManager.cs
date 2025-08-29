using UnityEngine;

public class DummyManager : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform firePoint;
    public GameObject arrowPrefab;
    public float fireRate = 1f;

    private string targetTag;
    private Transform enemy;


    public void Init(GameObject caster)
    {
        if (caster.CompareTag("Player"))
            targetTag = "Enemy";
        else if (caster.CompareTag("Enemy"))
            targetTag = "Player";

        FindTarget();
    }
    private void Update()
    {
        if (enemy == null)
        {
            FindTarget();
            return;
        }

    }

    public void GenerateArrow()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        ArrowPhysics arrow = arrowObj.GetComponent<ArrowPhysics>();
        if (arrow != null)
        {
            arrow.Setup(firePoint.position, enemy.position);
        }
    }
    private void FindTarget()
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObj != null)
            enemy = targetObj.transform;
    }
}
