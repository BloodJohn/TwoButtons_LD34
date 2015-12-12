using UnityEngine;

public class Character2D : MonoBehaviour
{
    [SerializeField]
    private float jumpForce = 400f;
    [SerializeField]
    private float shiftForce = 40f;
    [SerializeField]
    private LayerMask whatIsGround;                  // A mask determining what is ground to the character
    [SerializeField]
    private Transform groundCheck;    // A position marking where to check if the player is grounded.
    [SerializeField]
    private Transform ceilingCheck;   // A position marking where to check for ceilings

    private Rigidbody2D rigidbody;

    [SerializeField]
    private float groundedRadius; // Radius of the overlap circle to determine if grounded

    



    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var direction = 0f;
        var keyDown = false;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction -= shiftForce;
            keyDown = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += shiftForce;
            keyDown = true;
        }

        if (!keyDown) return;

        if (Input.anyKeyDown && OnGround())
        {
            rigidbody.AddForce(new Vector2(direction, jumpForce));
        }
        else
        {
            Debug.LogFormat("AddForce {0} {1}", direction * Time.deltaTime, Time.deltaTime);
            rigidbody.AddForce(new Vector2(direction * Time.deltaTime, 0f));
        }
    }

    bool OnGround()
    {
        var colliderList = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        foreach (var collider in colliderList)
            if (collider.gameObject != gameObject) return true;

        return false;
    }
}
