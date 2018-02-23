using System.Collections;
using System.Collections.Generic;
using Enumerables;
using UnityEngine;

public class RechargeZone : MonoBehaviour {
    private List<PlayerDashController> _dashControllers = new List<PlayerDashController>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var dashController = other.GetComponent<PlayerDashController>();
            if (dashController != null)
            {
                _dashControllers.Add(dashController);
                dashController.SetInZone(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var dashController = other.GetComponent<PlayerDashController>();
            if (dashController != null)
            {
                if (_dashControllers.Contains(dashController))
                {
                    _dashControllers.Remove(dashController);
                    dashController.SetInZone(false);
                }
            }
        }
    }
}
