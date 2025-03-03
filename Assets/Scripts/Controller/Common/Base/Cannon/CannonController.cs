using TANK3.Data.CannonDataManagement;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(HingeJoint))]
public class CannonController : MonoBehaviour
{
    //----- PUBLIC VARIABLES -----//
    public Vector3 CannonForward {  get; private set; }
    public CannonDataStore CannonDataStore;
    public string cannonId;
    public Rigidbody CannonRb;
    public float drag = 5.0f;

    //----- PROTECTED VARIABLES -----//
    protected float turnSpeed;

    //----- PRIVATE VARIABLES -----//
    [SerializeField] protected Vector3 AimDirection;
    private HingeJoint HingeJoint;
    
 
    public virtual void OnAwake()
    {
        // DO NOTHIMG
    }
    // Start is called before the first frame update
    public virtual void OnStart()
    {
        HingeJoint = GetComponent<HingeJoint>();
        HingeJoint.axis = new Vector3(0, 1, 0);
        HingeJoint.useLimits = true;
        JointLimits limits = HingeJoint.limits;
        limits.max = 90;
        limits.min = -90;
        CannonRb.angularDrag =drag;
        CannonRb.useGravity = false;
        AimDirection = -CannonRb.transform.up;   // ê≥ñ ÇÃï˚å¸Çç≈èâÇ…Ç…ì«Ç›éÊÇ¡ÇƒÇ®Ç≠.
        

        if (CannonDataStore.CannonDataDictionary[cannonId] == null) Debug.LogWarning("CannonController: CannonIdÇÃê›íËÇ™ä‘à·ÇƒÇ¢Ç‹Ç∑.");
        turnSpeed = CannonDataStore.CannonDataDictionary[cannonId].CannonTurnSpeed;
    }

    // Update is called once per frame
    public virtual void OnUpdate()
    {
        CannonForward = -CannonRb.transform.up; // BlenderÇ©ÇÁÇÃì«Ç›çûÇ›ÇÃñ‚ëËÇ≈ê≥ñ Ç™upÇ…Ç»ÇÈ.
    }

    public virtual void OnLateUpdate()
    {
        // DO NOTHING
    }

    protected  virtual void TurnCannon()
    {
        Vector3 torque = Vector3.Cross(CannonForward, AimDirection.normalized) * turnSpeed;
        CannonRb.AddTorque(torque);
    }
}
