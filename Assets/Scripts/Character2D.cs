using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private GameObject tutorial;

    [SerializeField]
    private AudioClip jumpSound;


    private Rigidbody2D rigidbody;
    private int halfWidth;

    [SerializeField]
    private float groundedRadius; // Radius of the overlap circle to determine if grounded


    enum GameState
    {
        play,
        lose,
        win,
        touchwin
    }

    private GameState state = GameState.play;


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        halfWidth = Screen.width / 2;
    }

    // Use this for initialization
    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        GoogleAnalyticsV3.instance.LogScreen("Level " + SceneManager.GetActiveScene().buildIndex);
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.lose: return;
            case GameState.touchwin:
                if (Input.GetMouseButtonUp(0)) state = GameState.win;
                return;
            case GameState.win:
                int index = SceneManager.GetActiveScene().buildIndex;
                if (Input.anyKeyDown) SceneManager.LoadScene(index+1);
                else if (Input.touchCount > 0) SceneManager.LoadScene(0);
                return;

            case GameState.play:
                {
                    if (CheckLose())
                    {
                        state = GameState.lose;
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                        return;
                    }

                    if (CheckWin())
                    {
                        state = Input.GetMouseButton(0) ? GameState.touchwin : GameState.win;

                        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                        return;
                    }

                    CheckControls();
                }
                break;
        }
    }

    private bool CheckLose()
    {
        if (transform.position.y < firstBox.transform.position.y) return true;

        if (rigidbody.velocity.magnitude < 0.001f && (transform.rotation.z > 0.5f || transform.rotation.z < -0.5f))
            return true;

        return false;
    }

    private bool CheckWin()
    {
        var colliderList = Physics2D.OverlapCircleAll(sleighPoint.position, groundedRadius, whatIsGround);
        foreach (var collider in colliderList)
            if (collider.gameObject == gameObject)
                return true;

        return false;
    }

    private void CheckControls()
    {
        var isGround = OnGround();

        var direction = GetKeyDirection(isGround);

        if (0f==direction)
        {
            direction = GetTouchDirection(isGround);
            if (0f == direction) return;
        }

        if (isGround)
        {
            rigidbody.AddForce(new Vector2(direction, jumpForce));
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        }
        else
            rigidbody.AddForce(new Vector2(direction * Time.deltaTime, 0f));
    }

    private float GetKeyDirection(bool isGround)
    {
        if (!Input.anyKey) return 0f;
        if (isGround && !Input.anyKeyDown) return 0f;

        var direction = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction -= shiftForce;

            if (tutorial.activeSelf) tutorial.SetActive(false);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += shiftForce;
        }

        return direction;
    }

    private float GetTouchDirection(bool isGround)
    {
        float xPos = 0f;
        float direction = 0f;


        if (isGround)
        {
            if (Input.GetMouseButtonDown(0)) xPos = Input.mousePosition.x;
        }
        else
        {
            if (Input.GetMouseButton(0)) xPos = Input.mousePosition.x;
        }

        foreach (var touchKey in Input.touches)
        {
            if (touchKey.phase != TouchPhase.Ended) continue;
            if (isGround && touchKey.phase != TouchPhase.Began) continue;

            xPos = touchKey.position.x;
        }

        if (xPos == 0f) return 0f;

        if (xPos < halfWidth && direction >= 0)
        {
            direction -= shiftForce;
            if (tutorial.activeSelf) tutorial.SetActive(false);
        }
        else if (xPos >= halfWidth && direction <= 0)
        {
            direction += shiftForce;
        }
        
        return direction;
    }

    private bool OnGround()
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
