using UnityEngine;
using UnityEngine.EventSystems;

public class BallAimer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform ball;
    public Transform ballAimCursor;
    public float targetPosX;
    public float ballDragSpeed;
    private bool canMove = false;

    Vector3 ballAimPos;
    Vector3 ballAimArrowPos;
    Vector3 ballAimCursorPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        canMove = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canMove = false;
    }

    void Start()
    {
        ballAimPos = ball.position;
        ballAimCursorPos = ballAimCursor.localPosition;
    }

    private void Update()
    {
        if (canMove)
        {
            ballAimPos.x = targetPosX;
            ball.position = Vector3.MoveTowards(ball.position, ballAimPos, ballDragSpeed * Time.deltaTime);
            ballAimArrowPos.x = ball.localPosition.x;
            ballAimCursorPos.x = ball.localPosition.x;
            ballAimCursor.localPosition = ballAimCursorPos;
        }
    }
}
