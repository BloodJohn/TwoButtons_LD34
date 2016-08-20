using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
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
    private int attempts = 1;

    [SerializeField]
    private float groundedRadius; // Radius of the overlap circle to determine if grounded


    enum GameState
    {
        play,
        lose,
        win,
        touchwin
    }

    private GameState _state = GameState.play;


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        halfWidth = Screen.width / 2;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case GameState.lose: return;
            case GameState.touchwin:
                if (Input.GetMouseButtonUp(0)) _state = GameState.win;
                return;
            case GameState.win:
                var index = SceneManager.GetActiveScene().buildIndex;
                if (Input.anyKeyDown)
                {
                    Analytics.CustomEvent("level complete", new Dictionary<string, object>
                      {
                        { "level", index },
                        { "attempts", attempts },
                      });
                    SceneManager.LoadScene(index + 1);
                }
                return;

            case GameState.play:
                {
                    if (CheckLose())
                    {
                        _state = GameState.lose;
                        attempts++;
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                        return;
                    }

                    if (CheckWin())
                    {
                        _state = Input.GetMouseButton(0) ? GameState.touchwin : GameState.win;

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
        foreach (var itemCheck in colliderList)
            if (itemCheck.gameObject == gameObject)
                return true;

        return false;
    }

    private void CheckControls()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();

        var isGround = OnGround();
        var direction = GetKeyDirection(isGround);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (0f == direction)
        {
            direction = GetTouchDirection(isGround);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
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
        var xPos = 0f;
        var direction = 0f;


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
            foreach (var checkItem in colliderList)
                if (checkItem.gameObject != gameObject)
                    return true;
        }
        return false;
    }
}
