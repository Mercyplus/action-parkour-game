using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourSystem : MonoBehaviour
{
    [SerializeField] private List<ParkourAction> parkourActions;

    private EnvironmentScanner environmentScanner;
    private Animator animator;
    private PlayerController playerController;

    private bool inAction;

    private void Awake() 
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update() 
    {
        if (Input.GetButton("Jump") && !inAction)
        {
           var hitData = environmentScanner.ObjectCheck();
           if (hitData.forwardHitFound)
           {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
           }
        }
    }

    public IEnumerator DoParkourAction(ParkourAction action)
    {
        inAction = true;
        playerController.SetControl(false);

        animator.SetBool("mirrorAction", action.MirrorAnimation);
        animator.CrossFadeInFixedTime(action.AnimatorName, 0.2f);
        yield return null;

        var animatorState = animator.GetNextAnimatorStateInfo(0);
        if (!animatorState.IsName(action.AnimatorName))
        {
            Debug.Log("неправильная анимация паркура ");
        }

        // yield return new WaitForSeconds(animatorState.length);

        float timer = 0;
        while (timer <= animatorState.length)
        {
            timer += Time.deltaTime;

            // Поворот игрока в сторону препятствия
            if (action.RotateToObjects)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, playerController.RotationSpeed * Time.deltaTime);
            }

            if (action.EnableTargetMatching)
            {
                MatchTarget(action);
            }

            if (animator.IsInTransition(0) && timer > 0.5f) break;
            yield return null;
        }

        yield return new WaitForSeconds(action.PostActionDelay);

        playerController.SetControl(true);
        inAction = false;
    }

    private void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget || animator.IsInTransition(0)) return;

        animator.MatchTarget(action.MatchPosition,
                            transform.rotation, 
                            action.MatchBodyPart, 
                            new MatchTargetWeightMask(action.MatchPositionWeight, 0), 
                            action.MatchStartTime, 
                            action.MatchTargetTime);
    }
}