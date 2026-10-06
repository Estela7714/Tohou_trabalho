using UnityEngine;

public class BossBaseBehaviour : StateMachineBehaviour
{
    protected BossController boss;
    protected Transform bossTransform;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<BossController>();
        bossTransform = animator.transform;
    }
}