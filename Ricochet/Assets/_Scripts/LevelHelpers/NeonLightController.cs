using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonLightController : MonoBehaviour {

    [System.Serializable]
    private struct TeamMaterials
    {
        public Material outerBand;
        public Material innerBand;
        public Gradient aura;
        public Gradient flash;
    }

    #region Fields & Monobehaviour
    [SerializeField] TeamMaterials redMaterials;

    [Space]

    [SerializeField] TeamMaterials blueMaterials;

    private Neon[] lights;
    private List<Neon> redLights;
    private List<Neon> blueLights;

	void Start () {
        redLights = new List<Neon>();
        blueLights = new List<Neon>();

        int count = transform.childCount;
        lights = new Neon[count];
        for (int i = 0; i < count; i++)
        {
            lights[i] = transform.GetChild(i).GetComponent<Neon>();
            if (lights[i].GetTeam() == ETeam.RedTeam)
                redLights.Add(transform.GetChild(i).GetComponent<Neon>());
            else
                blueLights.Add(transform.GetChild(i).GetComponent<Neon>());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            HitTheLights();
        else if (Input.GetKeyDown(KeyCode.X))
            HitTheTeamLights(ETeam.BlueTeam);
        else if (Input.GetKeyDown(KeyCode.C))
            HitTheTeamLights(ETeam.RedTeam);
        else if (Input.GetKeyDown(KeyCode.R))
            HitAllTheLightsAsTeam(ETeam.RedTeam);
        else if (Input.GetKeyDown(KeyCode.B))
            HitAllTheLightsAsTeam(ETeam.BlueTeam);
        else if (Input.GetKeyDown(KeyCode.V))
            ResetLightsMaterials();
    }
    #endregion

    public void HitTheLights()
    {
        foreach (Neon light in lights)
        {
            light.Flash();
        }
    }

    public void HitTheTeamLights(ETeam t)
    {
        List<Neon> teamLights = t == ETeam.RedTeam ? redLights : blueLights;
        foreach (Neon light in teamLights)
        {
            light.Flash();
        }
    }

    public void HitAllTheLightsAsTeam(ETeam t)
    {
        List<Neon> otherTeamLights = t == ETeam.RedTeam ? blueLights : redLights;
        TeamMaterials thisTeamMats = t == ETeam.RedTeam ? redMaterials : blueMaterials;
        foreach (Neon light in otherTeamLights)
        {
            light.SetOuterBandMaterial(thisTeamMats.outerBand);
            light.SetInnerBandMaterial(thisTeamMats.innerBand);
            light.SetAuraBandGradient(thisTeamMats.aura);
            light.SetFlashGradient(thisTeamMats.flash);
            light.ReInitialize();
        }
        HitTheLights();
    }

    public void ResetLightsMaterials()
    {
        foreach (Neon light in redLights)
        {
            light.SetOuterBandMaterial(redMaterials.outerBand);
            light.SetInnerBandMaterial(redMaterials.innerBand);
            light.SetAuraBandGradient(redMaterials.aura);
            light.SetFlashGradient(redMaterials.flash);
            light.ReInitialize();
        }
        foreach (Neon light in blueLights)
        {
            light.SetOuterBandMaterial(blueMaterials.outerBand);
            light.SetInnerBandMaterial(blueMaterials.innerBand);
            light.SetAuraBandGradient(blueMaterials.aura);
            light.SetFlashGradient(blueMaterials.flash);
            light.ReInitialize();
        }
    }
}
