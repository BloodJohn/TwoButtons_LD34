using UnityEngine;
public class AutoCam : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private Rigidbody2D rigidbodyTarget;
    //private Transform transform;

    [SerializeField]
    private float m_MoveSpeed = 3; // How fast the rig will move to keep up with target's position
    [SerializeField]
    private float m_TurnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
    [SerializeField]
    private bool m_FollowTilt = true; // Whether the rig will tilt (around X axis) with the target.
    [SerializeField]
    private float m_SpinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
    [SerializeField]
    private float m_TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
    [SerializeField]
    private float m_SmoothTurnTime = 0.2f; // the smoothing for the camera's rotation

    private float m_LastFlatAngle; // The relative angle of the target and the rig from the previous frame.
    private float m_CurrentTurnAmount; // How much to turn the camera
    private float m_TurnSpeedVelocityChange; // The change in the turn speed velocity
    private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )

    void Awake()
    {
        rigidbodyTarget = target.GetComponent<Rigidbody2D>();
        //transform = GetComponent<Transform>();
    }


    private void LateUpdate()
    {
        // initialise some vars, we'll be modifying these in a moment
        var targetForward = target.forward;
        var targetUp = target.up;


        // we're in 'follow rotation' mode, where the camera rig's rotation follows the object's rotation.

        // This section allows the camera to stop following the target's rotation when the target is spinning too fast.
        // eg when a car has been knocked into a spin. The camera will resume following the rotation
        // of the target when the target's angular velocity slows below the threshold.
        var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z)*Mathf.Rad2Deg;
        if (m_SpinTurnLimit > 0)
        {
            var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(m_LastFlatAngle, currentFlatAngle))/Time.deltaTime;
            var desiredTurnAmount = Mathf.InverseLerp(m_SpinTurnLimit, m_SpinTurnLimit*0.75f, targetSpinSpeed);
            var turnReactSpeed = (m_CurrentTurnAmount > desiredTurnAmount ? .1f : 1f);
            if (Application.isPlaying)
            {
                m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, desiredTurnAmount,
                    ref m_TurnSpeedVelocityChange, turnReactSpeed);
            }
            else
            {
                // for editor mode, smoothdamp won't work because it uses deltaTime internally
                m_CurrentTurnAmount = desiredTurnAmount;
            }
        }
        else
        {
            m_CurrentTurnAmount = 1;
        }
        m_LastFlatAngle = currentFlatAngle;


        // camera position moves towards target position:
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * m_MoveSpeed);

        // camera's rotation is split into two parts, which can have independend speed settings:
        // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')
        if (!m_FollowTilt)
        {
            targetForward.y = 0;
            if (targetForward.sqrMagnitude < float.Epsilon)
            {
                targetForward = transform.forward;
            }
        }
    }
}