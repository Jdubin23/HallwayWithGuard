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

        lastIndex = GetIndex(angle);

        spriteRenderer.sprite = sprites[lastIndex];
    }

    private int GetIndex(float angle)
    {
        //front
        if (angle > -90f && angle < 90f)
            return 0;

        //back
        if (angle <= -90f && angle >= 90f)
            return 1;

        return lastIndex;
    }
}
