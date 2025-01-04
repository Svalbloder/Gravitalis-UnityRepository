using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public enum ObjectState
{
    draging,
    snaping,
    falling,
    sticking,
}
public enum GravityDirection
{
    Null,
    Up,
    Down,
    Left,
    Right
}
public enum BuildState
{
    idle,
    wrong,
    correct,
    fine
}
public class SnapAble : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Rigidbody2D rb;
    private float gravityStrength = 9.8f;
    private float rotationSpeed = 5f;
    public ObjectState currentState = ObjectState.falling;
    public Collider2D gravityRegion; // 重力区域
    public float stickRayLength;
    public float blockRayLength;
    public float powerLostRateStill;
    public float powerLostRateMove;
    public float powerGainRate;
    public GameObject stickObj;
    public BuildState currentBuildState;
    public SpriteRenderer spriteRenderer;
    private Vector3 initialPosition; // 用于存储按下时的位置
    public GameObject leftPoint;
    public GameObject rightPoint;
    public Collider2D coll;
    private bool isRight;
    private bool isLeft;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("未找到 Rigidbody2D 组件！");
        }
        coll = GetComponent<Collider2D>();
        if (coll == null)
        {
            Debug.LogError("未找到 Collider2D 组件！");
        }
    }

    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Update()
    {
        if (gravityRegion != null)
        {
            // Debug 显示到区域表面最近点
            Vector2 closestPoint = gravityRegion.ClosestPoint(transform.position);
            Debug.DrawLine(transform.position, closestPoint, Color.blue);
        }
    }

    public virtual void FixedUpdate()
    {
        UpdateState();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (!CarManager.instance.snapAbleList.Contains(this))
                CarManager.instance.snapAbleList.Add(this);

            if (collision == gravityRegion && currentState != ObjectState.draging && currentState != ObjectState.sticking)
            {
                ChangeState(ObjectState.snaping);
            }
        }
        if (collision != null && !collision.isTrigger && currentState == ObjectState.draging)
        {
            currentBuildState = BuildState.wrong;
        }
    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CarManager.instance.snapAbleList.Contains(this))
            CarManager.instance.snapAbleList.Remove(this);
        if (collision != null)
        {
            if (collision.GetComponent<GravitationalField>() != null && currentState != ObjectState.draging)
            {
                ChangeState(ObjectState.falling);
            }
        }
        if (collision != null && !collision.isTrigger && currentState == ObjectState.draging)
        {
            currentBuildState = BuildState.correct;
        }
    }

    public void ChangeState(ObjectState state)
    {
        if(state == ObjectState.draging)
        {
            coll.isTrigger = true;
        }
        else
        {
            coll.isTrigger = false;
        }
        switch (state)
        {
            case ObjectState.draging:
                rb.constraints = RigidbodyConstraints2D.None;
                currentState = ObjectState.draging;
                break;
            case ObjectState.snaping:
                currentState = ObjectState.snaping;
                break;
            case ObjectState.falling:
                currentState = ObjectState.falling;
                break;
            case ObjectState.sticking:
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                currentState = ObjectState.sticking;
                break;
        }
    }

    public void UpdateState()
    {
        switch (currentState)
        {
            case ObjectState.draging:
                Drag();
                StickCheck();
                GravityRotate();
                break;
            case ObjectState.snaping:
                Snap();
                GravityRotate();
                StickCheck();
                break;
            case ObjectState.falling:
                Snap();
                GravityRotate();
                break;
            case ObjectState.sticking:
                Snap();
                break;
        }

        switch (currentBuildState)
        {
            case BuildState.idle:
                spriteRenderer.color = Color.white;
                break;

            case BuildState.wrong:
                spriteRenderer.color = Color.red;
                break;

            case BuildState.correct:
                spriteRenderer.color = Color.green;
                break;

            case BuildState.fine:
                spriteRenderer.color = Color.yellow;
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ChangeState(ObjectState.draging);
            initialPosition = transform.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (currentBuildState == BuildState.wrong)
            {
                transform.position = initialPosition;
            }
            currentBuildState = BuildState.idle;
            ChangeState(currentState == ObjectState.snaping ? ObjectState.snaping : ObjectState.falling);
        }
    }

    public void StickCheck()
    {
        if (gravityRegion == null) return;

        Vector2 rayDirection = GetGravityDirection();

        Vector2 leftRayOrigin = leftPoint.transform.position;
        Vector2 rightRayOrigin = rightPoint.transform.position;

        Debug.DrawRay(leftRayOrigin, rayDirection * stickRayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, rayDirection * stickRayLength, Color.red);

        RaycastHit2D[] leftHit = Physics2D.RaycastAll(leftRayOrigin, rayDirection, stickRayLength);
        RaycastHit2D[] rightHit = Physics2D.RaycastAll(rightRayOrigin, rayDirection, stickRayLength);

        foreach (RaycastHit2D hit in leftHit)
        {
            if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject != gameObject)
            {
                isLeft = true;
                break;
            }
        }
        foreach (RaycastHit2D hit in rightHit)
        {
            if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject != gameObject)
            {
                isRight = true;
                break;
            }
        }

        if (isLeft && isRight)
        {
            if (currentState != ObjectState.draging)
            {
                ChangeState(ObjectState.sticking);
            }
        }

        isRight = false;
        isLeft = false;
    }

    private void Snap()
    {
        if (gravityRegion == null) return;

        Vector2 direction = GetGravityDirection();
        rb.AddForce(direction * gravityStrength, ForceMode2D.Force);
    }

    private void Drag()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        transform.position = worldPosition;
    }

    private void GravityRotate()
    {
        if (gravityRegion == null) return;

        Vector2 direction = GetGravityDirection();
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        float currentAngle = transform.eulerAngles.z;
        float smoothAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }

    public void SwitchGravityRegion(Collider2D coll)
    {
        gravityRegion = coll;
    }

    private Vector2 GetGravityDirection()
    {
        if (gravityRegion == null) return Vector2.zero;

        Vector2 closestPoint = gravityRegion.ClosestPoint(transform.position);
        return (closestPoint - (Vector2)transform.position).normalized;
    }
}
