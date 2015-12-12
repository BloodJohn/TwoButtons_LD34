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
    private Transform[] groundCheck;    // A position marking where to check if the player is grounded.
    [SerializeField]
    private Transform firstBox;
    [SerializeField]
    private Transform sleighPoint;


    private Rigidbody2D rigidbody;

    [SerializeField]
    private float groundedRadius; // Radius of the overlap circle to determine if grounded


    enum GameState
    {
        play,
        lose,
        win
    }

    private GameState state = GameState.play;


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
        if (state != GameState.play) return;

        if (CheckLose())
        {
            state = GameState.lose;
            Application.LoadLevel(Application.loadedLevel);
            return;
        }

        if (CheckWin())
        {
            state = GameState.win;
            Debug.Log("Deer is back to Santa");

            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }

        CheckControls();
    }

    bool CheckLose()
    {
        return (transform.position.y < firstBox.transform.position.y);
    }

    bool CheckWin()
    {
        var colliderList = Physics2D.OverlapCircleAll(sleighPoint.position, groundedRadius, whatIsGround);
        foreach (var collider in colliderList)
            if (collider.gameObject == gameObject)
                return true;

        return false;
    }

    void CheckControls()
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
            rigidbody.AddForce(new Vector2(direction, jumpForce));
        else
            rigidbody.AddForce(new Vector2(direction * Time.deltaTime, 0f));
    }

    bool OnGround()
    {
        foreach (var transformCheck in groundCheck)
        {
            var colliderList = Physics2D.OverlapCircleAll(transformCheck.position, groundedRadius, whatIsGround);
            foreach (var collider in colliderList)
                if (collider.gameObject != gameObject)
                    return true;
        }
        return false;
    }
}
