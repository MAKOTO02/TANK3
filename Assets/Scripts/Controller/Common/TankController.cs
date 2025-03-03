using UnityEngine;
using TANK3.Data.BulletManagement;
using TANK3.Data.VehicleManagement;
using TANK3.Data.CannonDataManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TANK3.Controller
{
    [RequireComponent(typeof(CannonController), typeof(BulletController), typeof(VehicleController))]
    public class TankController : MonoBehaviour
    {
        private BulletController bulletController;
        private CannonController cannonController;
        private VehicleController vehicleController;
        [SerializeField] GameObject BulletMark;
        [SerializeField] Rigidbody Cannon;

        public BulletDataStore bulletDataStore;
        public CannonDataStore cannonDataStore;
        public VehicleControlDataStore vehicleControlDataStore;

        public string bulletId;
        public string vehicleId;
        public string cannonId;

        private void Awake()
        {
            InitializeController();
            // èáî‘Ç…íçà”.
            bulletController.OnAwake();
            cannonController.OnAwake();
            vehicleController.OnAwake();
        }
        // Start is called before the first frame update
        void Start()
        {
            bulletController.OnStart();
            cannonController.OnStart();
            vehicleController.OnStart();
        }

        // Update is called once per frame
        void Update()
        {
            bulletController.OnUpdate();
            cannonController.OnUpdate();
            vehicleController.OnUpdate();
        }

        private void LateUpdate()
        {
            bulletController.OnLateUpdate();
            cannonController.OnLateUpdate();
            vehicleController.OnLateUpdate();
        }

        void OnEnable()
        {
            GameManager.Instance.OnActiveStateChanged += HandleActiveStateChanged;
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnActiveStateChanged -= HandleActiveStateChanged;
            }
        }

        private void HandleActiveStateChanged(bool isActive)
        {
            // GameManager Ç©ÇÁìnÇ≥ÇÍÇΩèÛë‘Ç…âûÇ∂Çƒ enabled ÇçXêV
            enabled = isActive;
        }

        void InitializeController()
        {
            bulletController = GetComponent<BulletController>();
            cannonController = GetComponent<CannonController>();
            vehicleController = GetComponent<VehicleController>();

            bulletController.bulletId = bulletId;
            bulletController.bulletDataStore = bulletDataStore;
            cannonController.cannonId = cannonId;
            cannonController.CannonDataStore = cannonDataStore;
            vehicleController.vehicleId = vehicleId;
            vehicleController.VehicleControlDataStore = vehicleControlDataStore;

            cannonController.CannonRb = Cannon;
            bulletController.bulletMark = BulletMark;
        }
    }
}