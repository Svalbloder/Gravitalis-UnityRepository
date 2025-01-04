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
    public static Cat instance; // ����ʵ��;
    private CatState currentState;

    // Animator ���ڿ��ƶ���
    private Animator animator;

    // ״̬�л���ʱ�������룩
    private float stateChangeInterval = 60f; // 1����
    private float timer;

    // ��Ծ�߶Ⱥ���Ծ�ٶ�
    public float jumpHeight;
    public float jumpSpeed;  // ��Ծ���ٶ�
    private Vector3 jumpStartPos; // ��Ծ��ʼ��
    private Vector3 jumpEndPos;   // ��ԾĿ���
    private bool isJumping = false; // �ж��Ƿ�������Ծ
    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    void Start()
    {
        // ��ȡ Animator ���
        animator = GetComponent<Animator>();

        // ��ʼ״̬����Ϊ����
        currentState = CatState.Idle;

        // ����״̬�л��ļ�ʱ��
        timer = stateChangeInterval;

        // ���ó�ʼ����
        UpdateAnimation();

        // ��ʼִ��״̬�л�Э��
        StartCoroutine(StateChangeRoutine());
        jumpHeight = 3f;
        jumpSpeed = 8f;
    }

    void Update()
    {
        // ���������Ծ��������Ծλ��
        if (isJumping)
        {
            // ��Ծ����
            JumpMovement();
        }
        else
        {
            // ״̬�л���ʱ������
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                // ÿ��stateChangeInterval���л�һ��״̬
                timer = stateChangeInterval;
                ChangeState();
            }

            // ���ݵ�ǰ״̬���¶���
            UpdateAnimation();
        }
    }

    // ����ı�è���״̬
    private void ChangeState()
    {
        int randomState = Random.Range(0, 2);
        currentState = (CatState)randomState;
    }

    // ���� Animator �еĶ���״̬
    private void UpdateAnimation()
    {
        switch (currentState)
        {
            case CatState.Sleeping:
                animator.SetBool("Sleep",true);
                animator.SetBool("Idle", false);// ���� Sleep �� Animator �еĴ�����
                break;

            case CatState.Idle:
                animator.SetBool("Idle",true);
                animator.SetBool("Sleep", false);// ���� Idle �� Animator �еĴ�����
                break;
        }
    }

    // Э�����ڶ�ʱ�ı�è��״̬
    private IEnumerator StateChangeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stateChangeInterval);
            ChangeState();
        }
    }

    // ��Ծ���Ϸ�����ԾĿ���ĸ߶����ӣ�
    public void JumpUp(Vector3 targetPos)
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpStartPos = transform.position;
            jumpEndPos = targetPos;
            jumpEndPos.y += jumpHeight;  // ������Ծ�ĸ߶�
            animator.SetTrigger("JumpUp"); // �������϶���
        }
    }

    // ��Ծ���·�����ԾĿ���ĸ߶ȼ��٣�
    public void JumpDown(Vector3 targetPos)
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpStartPos = transform.position;
            jumpEndPos = targetPos;
            jumpEndPos.y -= jumpHeight;  // ������Ծ�ĸ߶�
            animator.SetTrigger("JumpDown"); // �������¶���
        }
    }

    // ��Ծ���ƶ�����
    private void JumpMovement()
    {
        float step = jumpSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, jumpEndPos, step);

        // ������Ŀ��λ��ʱֹͣ��Ծ
        if (transform.position == jumpEndPos)
        {
            isJumping = false;
            // ȷ��è������Ծ������ص�����״̬�����л�����״̬��
            currentState = CatState.Idle;
        }
    }
}
