using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourSystem : MonoBehaviour
{
    [SerializeField] private List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;
    [SerializeField] private float autoDropHeightLimit = 1f;

    private EnvironmentScanner environmentScanner;
    private Animator animator;
    private PlayerController playerController;

    private void Awake() 
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update() 
    {
        var hitData = environmentScanner.ObjectCheck();

        if (Input.GetButton("Jump") && !playerController.InAction && !playerController.IsHanging)
        {
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

        if (playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetButton("Jump"))
            {
                shouldJump = false;
            }

            if (shouldJump && playerController.LedgeData.angle <= 50)
            {
                playerController.IsOnLedge = false;
                StartCoroutine(DoParkourAction(jumpDownAction));
            }
        }
    }

    public IEnumerator DoParkourAction(ParkourAction action)
    {
        playerController.SetControl(false);

        MatchTargetParams matchParams = null;
        if (action.EnableTargetMatching)
        {
            matchParams = new MatchTargetParams()
            {
                position = action.MatchPosition,
                bodyPart = action.MatchBodyPart,
                positionWeight = action.MatchPositionWeight,
                startTime = action.MatchStartTime,
                targetTime = action.MatchTargetTime
            };
        }

        yield return playerController.DoAction(action.AnimatorName, matchParams, action.TargetRotation, 
            action.RotateToObjects, action.PostActionDelay, action.MirrorAnimation);

        playerController.SetControl(true);
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