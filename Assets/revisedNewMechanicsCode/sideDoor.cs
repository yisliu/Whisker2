using UnityEngine;

public class sideDoor : MonoBehaviour
{

    public Transform hingePivot;

    public float maxAngle = 90f;

    public float openSpeed = 180f;

    public float closeSpeed = 90f;

    private float currentAngle = 0f;

    private float targetAngle = 0f;

    private bool playerNear = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerNear = true;
        Vector3 toPlayer = other.transform.position - transform.position;
        float side = Vector3.Dot(transform.forward, toPlayer);
        targetAngle = side >= 0?maxAngle:-maxAngle;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerNear = false;
        targetAngle = 0f;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = playerNear ? openSpeed : closeSpeed;
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.deltaTime);
        hingePivot.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }
}
