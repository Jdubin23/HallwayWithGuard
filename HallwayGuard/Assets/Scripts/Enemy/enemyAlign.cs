using UnityEngine;

public class enemyAlign : MonoBehaviour
{
    private Transform player;
    private Vector3 targetPosition;
    private Vector3 targetDirection;
    public GameObject playerLocation;
    public int lastIndex;

    public SpriteRenderer spriteRenderer;

    public float angle;

    public Sprite[] sprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = playerLocation.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Get Target Position + Direction
        targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDirection = targetPosition - transform.position;

        //Get Angle
        angle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

        Vector3 tempScale = Vector3.one;
        if(angle > 0)
        {
            tempScale.x *= -1f;
        }

        spriteRenderer.transform.localScale = tempScale;

        lastIndex = GetIndex(angle);

        spriteRenderer.sprite = sprites[lastIndex];
    }

    private int GetIndex(float angle)
    {
        //front
        if (angle > -22.5f && angle < 22.6f)
            return 0;
        if (angle >= 22.5f && angle < 67.5f)
            return 7;
        if (angle >= 67.5f && angle < 112.5f)
            return 6;
        if (angle >= 112.5f && angle < 157.5f)
            return 5;

        //back
        if (angle <= -157.5f && angle >= 157.5f)
            return 4;
        if (angle >= -157.5f && angle < 112.5f)
            return 3;
        if (angle >= -112.5f && angle < -67.5f)
            return 2;
        if (angle >= -67.5f && angle < -22.5f)
            return 1;

        return lastIndex;
    }
}
