using Enumerables;
using Rewired;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerDashController : MonoBehaviour
{
    #region Private Variables
    [SerializeField]
    private float dashSpeedBoost = 10f;

    [SerializeField]
    private int maxDashCount = 5;

    private int dashCount;

    [SerializeField]
    [Tooltip("Recharge one dash every x seconds")]
    private float groundRecharge = .33333f;

    [SerializeField]
    [Tooltip("Delay until dash recharge begins")]
    private float groundRechargeDelay = .66666f;
    
    private PlayerController pc;
    private Player player;
    private Animator anim;
    private GameManager gm;
    private AudioSource audioSource;

    private float rechargeTimer;
    private float delayTimer;
    #endregion

    #region Monobehaviours
    public void Awake()
    {
        pc = GetComponent<PlayerController>();
        audioSource = GetComponentInChildren<AudioSource>();
        ResetDashController();
    }

    public void Start()
    {
        player = pc.GetPlayer();
    }

    public void OnEnable()
    {
        ResetDashController();
    }

    public void Update()
    {
        HandleDashRecharge();

        if (player.GetButtonDown("Dash") && dashCount > 0)
        {
            Dash();
        }
    }
    #endregion

    #region Public Methods
    public void ResetDashController()
    {
        rechargeTimer = groundRecharge;
        delayTimer = 0;
        dashCount = maxDashCount;
    }
    #endregion

    #region Getters
    public int GetDashCount()
    {
        return dashCount;
    }

    public int GetMaxDashCount()
    {
        return maxDashCount;
    }
    #endregion

    #region Private Helpers
    private void Dash()
    {
        if (gm != null || GameManager.TryGetInstance(out gm))
        {
            audioSource.PlayOneShot(gm.GetCharacterSFX(pc.GetCharacter(), ECharacterAction.Dash));
        }

        Vector3 dashVelocity = pc.GetRightStickDirection() * dashSpeedBoost;

        pc.AddVelocity(dashVelocity);

        dashCount--;
    }

    private void HandleDashRecharge()
    {
        // If not grounded, not recharge, reset timers
        if (!pc.IsGrounded())
        {
            rechargeTimer = groundRecharge;
            delayTimer = 0;
            return;
        }

        // Recharge one dash if grounded longer than groundRechargeDelay
        // Recharge another dash every rechargeRate seconds
        delayTimer += Time.deltaTime;
        if (delayTimer > groundRechargeDelay)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer > groundRecharge)
            {
                rechargeTimer = 0;
                dashCount = Mathf.Clamp(++dashCount, 0, maxDashCount);
            }
        }
    }
    #endregion
}
