using System;
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

    [SerializeField]
    private GameObject tutorial;

    [SerializeField] private AudioClip jumpSound;


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
        switch (state)
        {
            case GameState.lose: return;
            case GameState.win:
                if (Input.anyKeyDown) Application.LoadLevel(1);
                return;

            case GameState.play:
            {
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
                break;
        }
    }

    private bool CheckLose()
    {
        if (transform.position.y < firstBox.transform.position.y) return true;

        if (rigidbody.velocity.magnitude < 0.001f && (transform.rotation.z>0.5f || transform.rotation.z < -0.5f))
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
        var direction = 0f;
        var keyDown = false;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction -= shiftForce;
            keyDown = true;

            if (tutorial.activeSelf) tutorial.SetActive(false);
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
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        }
        else
            rigidbody.AddForce(new Vector2(direction*Time.deltaTime, 0f));
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
