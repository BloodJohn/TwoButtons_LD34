using UnityEngine;

public class Character2D : MonoBehaviour
{
    [SerializeField]
    private float jumpForce = 400f;

    private Rigidbody2D rigidbody;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("space key was pressed");
            rigidbody.AddForce(new Vector2(0f, jumpForce));
        }
    }
}
