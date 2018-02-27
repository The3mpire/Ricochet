using System;
using System.Collections;
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
    private float rechargeRate = .33333f;

    [SerializeField]
    [Tooltip("Delay until grounded dash recharge rate begins")]
    private float rechargeDelay = .66666f;

    [SerializeField]
    [Tooltip("Multiplier to apply for the grounded recharge rate")]
    private float groundedRechargeRate = 0.333f;

    [Tooltip("How quickly does the mini-dash recharge?")]
    [SerializeField]
    private float miniDashChargeTime;

    [Tooltip("Drag the player's revup ricle here")]
    [SerializeField]
    private GameObject revupCircle;

    private PlayerController pc;
    private Player player;
    private Animator anim;
    private GameManager gm;
    private AudioSource audioSource;

    private bool playerInZone = false;

    private float miniDashCharge;
    private float rechargeTimer;
    private float delayTimer;

    [SerializeField]
    private GameDataSO gameData;
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

        bool grounded = pc.IsGrounded();
        bool charging = (pc.GetAutoJetpack() ? pc.GetLeftTrigger() != 0 : pc.GetLeftTrigger() == 0) && !grounded && !pc.MovementDisabled();
        if (charging)
        {
            miniDashCharge = Mathf.Min(miniDashCharge + Time.deltaTime, miniDashChargeTime);
        }
        else
        {
            if (miniDashCharge >= miniDashChargeTime && !pc.MovementDisabled() && !grounded)
            {
                Dash(0, 0.45f);
            }
            miniDashCharge = 0f;
        }

        if (player.GetButtonDown("Dash") && dashCount > 0 && !pc.MovementDisabled())
        {
            Dash(1,1f);
        }
    }
    #endregion

    #region Public Methods
    public void ResetDashController()
    {
        rechargeTimer = rechargeRate;
        delayTimer = 0;
        dashCount = maxDashCount;
    }

    public void StartingBoost()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine("StartingBoostCoroutine");
        }
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
    private void Dash(int cost, float modifier)
    {
        if (gm != null || GameManager.TryGetInstance(out gm))
        {
            audioSource.PlayOneShot(gm.GetCharacterSFX(pc.GetCharacter(), ECharacterAction.Dash));
        }

        Vector3 dashVelocity = pc.GetShieldDirection() * (dashSpeedBoost * modifier);

        pc.AddVelocity(dashVelocity);

        dashCount -= cost;
    }

    private IEnumerator StartingBoostCoroutine()
    {
        float power = 0f, angle = 0f, radius = 1.5f;
        revupCircle.SetActive(true);
        while (pc.MovementDisabled())
        {
            float revPower = pc.GetAutoJetpack() ? pc.GetLeftStick().magnitude : pc.GetLeftTrigger();
            if (revPower != 0)
            {
                power += revPower * Time.deltaTime;
            }
            else
            {
                power -= 0.5f * Time.deltaTime;
                power = Mathf.Clamp(power - (0.5f * Time.deltaTime), 0, 3f);
            }

            Vector3 tp = transform.position;
            angle += 25f * power * Time.deltaTime;
            Vector2 offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            revupCircle.transform.position = new Vector3(tp.x + offset.x, tp.y + offset.y, tp.z);
            yield return new WaitForEndOfFrame();
        }
        if (power >= 1.5f && power <= 2.0f)
        {
            Dash(0, 0.8f);
        }
        revupCircle.SetActive(false);
    }

    private void HandleDashRecharge()
    {
        bool grounded = pc.IsGrounded();

        if (!grounded)
        {
            Recharge(1f);
            delayTimer = 0f;
        }
        else if (grounded)
        {
            if (delayTimer < rechargeDelay)
            {
                delayTimer += Time.deltaTime;
                Recharge(1f);
            }
            else
            {
                Recharge(groundedRechargeRate);
            }
        }
    }

    private void Recharge(float rate)
    {
        if (dashCount < maxDashCount)
        {
            rechargeTimer += Time.deltaTime * rate;
            if (rechargeTimer >= rechargeRate)
            {
                rechargeTimer = 0f;
                dashCount = Mathf.Clamp(++dashCount, 0, maxDashCount);
            }
        }
    }
    #endregion
}
