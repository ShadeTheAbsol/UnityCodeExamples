using UnityEngine;

public class ParallaxBehavior : MonoBehaviour
{
    [SerializeField]
    private Vector2 parallaxEffectMultiplier;

    private Transform playerCamTransform;
    private Vector3 lastCamPos;
    private float textureUnitSizeX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamTransform = Camera.main.transform;
        lastCamPos = playerCamTransform.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        //Calculate Sprite Texture Size On X Per Unity World Unit Based On Sprite Pixels Per Unit
        Texture2D texture = sprite.texture;
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        //Calculate How Far Player Camera Moved Between Frames Since Last Camera Position And In What Direction
        Vector3 deltaMovement = playerCamTransform.position - lastCamPos;

        //Offset Background By Using ParallaxEffectMultiplier To Move Background.
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
        lastCamPos = playerCamTransform.position;

        //Use Mathf.Abs To Ensure Calculation Works For Positive And Negative Values
        if (Mathf.Abs(playerCamTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            //Offset Background By Using Code Below When Repositioning Background For Infinite Background Effect
            //Calculates Remainder Before End Of Texture In World Units Reached
            //Offset Added To Position Background Seamlessly Before End Of Texture In World Is Reached By Player
            float offsetPositionX = (playerCamTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(playerCamTransform.position.x + offsetPositionX, transform.position.y);
        }
    }
}
