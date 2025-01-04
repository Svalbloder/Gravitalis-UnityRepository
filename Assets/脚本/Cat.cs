using UnityEngine;
using System.Collections;
public enum CatState
{
    Sleeping,
    Idle,
    //Walking
}
public class Cat : MonoBehaviour
{
    public static Cat instance; // 单例实例;
    private CatState currentState;

    // Animator 用于控制动画
    private Animator animator;

    // 状态切换的时间间隔（秒）
    private float stateChangeInterval = 60f; // 1分钟
    private float timer;

    // 跳跃高度和跳跃速度
    public float jumpHeight;
    public float jumpSpeed;  // 跳跃的速度
    private Vector3 jumpStartPos; // 跳跃起始点
    private Vector3 jumpEndPos;   // 跳跃目标点
    private bool isJumping = false; // 判断是否正在跳跃
    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    void Start()
    {
        // 获取 Animator 组件
        animator = GetComponent<Animator>();

        // 初始状态设置为空闲
        currentState = CatState.Idle;

        // 启动状态切换的计时器
        timer = stateChangeInterval;

        // 设置初始动画
        UpdateAnimation();

        // 开始执行状态切换协程
        StartCoroutine(StateChangeRoutine());
        jumpHeight = 3f;
        jumpSpeed = 8f;
    }

    void Update()
    {
        // 如果正在跳跃，更新跳跃位置
        if (isJumping)
        {
            // 跳跃计算
            JumpMovement();
        }
        else
        {
            // 状态切换计时器更新
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                // 每隔stateChangeInterval秒切换一次状态
                timer = stateChangeInterval;
                ChangeState();
            }

            // 根据当前状态更新动画
            UpdateAnimation();
        }
    }

    // 随机改变猫咪的状态
    private void ChangeState()
    {
        int randomState = Random.Range(0, 2);
        currentState = (CatState)randomState;
    }

    // 更新 Animator 中的动画状态
    private void UpdateAnimation()
    {
        switch (currentState)
        {
            case CatState.Sleeping:
                animator.SetBool("Sleep",true);
                animator.SetBool("Idle", false);// 假设 Sleep 是 Animator 中的触发器
                break;

            case CatState.Idle:
                animator.SetBool("Idle",true);
                animator.SetBool("Sleep", false);// 假设 Idle 是 Animator 中的触发器
                break;
        }
    }

    // 协程用于定时改变猫咪状态
    private IEnumerator StateChangeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stateChangeInterval);
            ChangeState();
        }
    }

    // 跳跃到上方（跳跃目标点的高度增加）
    public void JumpUp(Vector3 targetPos)
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpStartPos = transform.position;
            jumpEndPos = targetPos;
            jumpEndPos.y += jumpHeight;  // 增加跳跃的高度
            animator.SetTrigger("JumpUp"); // 播放跳上动画
        }
    }

    // 跳跃到下方（跳跃目标点的高度减少）
    public void JumpDown(Vector3 targetPos)
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpStartPos = transform.position;
            jumpEndPos = targetPos;
            jumpEndPos.y -= jumpHeight;  // 减少跳跃的高度
            animator.SetTrigger("JumpDown"); // 播放跳下动画
        }
    }

    // 跳跃的移动计算
    private void JumpMovement()
    {
        float step = jumpSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, jumpEndPos, step);

        // 当到达目标位置时停止跳跃
        if (transform.position == jumpEndPos)
        {
            isJumping = false;
            // 确保猫咪在跳跃结束后回到正常状态（空闲或其他状态）
            currentState = CatState.Idle;
        }
    }
}
