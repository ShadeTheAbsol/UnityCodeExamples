using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HoopMover : MonoBehaviour
{
    public enum Direction {left,right};
    public Direction moveDirection;
    public float hoopMoveSpeed;
    public Ease movementType;
    public Transform hoopFrontNet;

    Vector3 hoopStartPos;
    Vector3 hoopLeftSideSpawnPos;
    Vector3 hoopRightSideSpawnPos;
    float distanceToTarget;

    // Start is called before the first frame update
    void Start()
    {
        hoopStartPos = transform.localPosition;
        hoopLeftSideSpawnPos = hoopStartPos;
        hoopLeftSideSpawnPos.x = -1016f;
        hoopRightSideSpawnPos = hoopStartPos;
        hoopRightSideSpawnPos.x = 2104f;
        MoveHoop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveHoop()
    {
        if (moveDirection == Direction.left)
        {
            distanceToTarget = Vector3.Distance(transform.localPosition, hoopLeftSideSpawnPos);
            transform.DOLocalMoveX(hoopLeftSideSpawnPos.x, distanceToTarget / hoopMoveSpeed).SetEase(movementType).OnComplete(MoveHoopToOtherSide);
            hoopFrontNet.DOLocalMoveX(hoopLeftSideSpawnPos.x, distanceToTarget/hoopMoveSpeed).SetEase(movementType).OnComplete(MoveHoopToOtherSide);
        }
        else
        {
            distanceToTarget = Vector3.Distance(transform.localPosition, hoopRightSideSpawnPos);
            transform.DOLocalMoveX(hoopRightSideSpawnPos.x, distanceToTarget / hoopMoveSpeed).SetEase(movementType).OnComplete(MoveHoopToOtherSide);
            hoopFrontNet.DOLocalMoveX(hoopRightSideSpawnPos.x, distanceToTarget / hoopMoveSpeed).SetEase(movementType).OnComplete(MoveHoopToOtherSide);
        }
    }

    void MoveHoopToOtherSide() 
    {
        if (moveDirection == Direction.left)
        {
            transform.localPosition = hoopRightSideSpawnPos;
            hoopFrontNet.localPosition = hoopRightSideSpawnPos;
            KillHoopMovementOnly();

            MoveHoop();
        }
        else
        {
            transform.localPosition = hoopLeftSideSpawnPos;
            hoopFrontNet.localPosition = hoopLeftSideSpawnPos;
            KillHoopMovementOnly();

            MoveHoop();
        }
    }

    public void PauseHoop()
    {
        transform.DOPause();
        hoopFrontNet.DOPause();
    }

    public void PlayHoop()
    {
        transform.DOPlay();
        hoopFrontNet.DOPlay();
    }

    public void ResetHoop()
    {
        transform.DOKill();
        hoopFrontNet.DOKill();
        transform.localPosition = hoopStartPos;
        hoopFrontNet.localPosition = hoopStartPos;

        MoveHoop();
    }

    public void EndHoopMovement()
    {
        transform.DOKill();
        hoopFrontNet.DOKill();
        transform.localPosition = hoopStartPos;
        hoopFrontNet.localPosition = hoopStartPos;
    }

    public void KillHoopMovementOnly()
    {
        transform.DOKill();
        hoopFrontNet.DOKill();
    }
}
