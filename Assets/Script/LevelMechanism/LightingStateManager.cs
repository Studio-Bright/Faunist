using UnityEngine;
using UnityEngine.Rendering;

public class LightingStateManager : MonoBehaviour
{
    [Header("Lightmap Switching Tool")]
    public LevelLightmapData levelLightmapData;

    [Header("Realtime Lights")]
    public Light[] dayLights;
    public Light[] nightLights;

    [Header("Emissive Materials (optional direct control)")]
    public Material[] emissiveMaterials;

    [Header("Emissive Renderers (recommended)")]
    public Renderer[] emissiveRenderers;

    [ColorUsage(true, true)]
    public Color dayEmissionColor = Color.black;

    [ColorUsage(true, true)]
    public Color nightEmissionColor = Color.white * 5f;

    [Header("Fog Settings")]
    public Color dayFogColor = Color.cyan;
    public Color nightFogColor = Color.black;

    [Header("Ambient Lighting")]
    public Color dayAmbient = Color.gray;
    public Color nightAmbient = Color.black;

    [Header("Volumes (Optional)")]
    public Volume dayVolume;
    public Volume nightVolume;

    private bool isNight;

    public void SwitchToDay()
    {
        isNight = false;

        ApplyLighting(0, dayLights, nightLights, dayEmissionColor, dayFogColor, dayAmbient,
            dayVolume, nightVolume);
        RefreshLightingProbes();
    }

    public void SwitchToNight()
    {
        isNight = true;

        ApplyLighting(1, dayLights, nightLights, nightEmissionColor, nightFogColor, nightAmbient,
            dayVolume, nightVolume);
        RefreshLightingProbes();
    }

    public void ToggleLighting()
    {
        if (isNight) SwitchToDay();
        else SwitchToNight();

    }

    private void ApplyLighting(
        int lightmapIndex,
        Light[] enableLights,
        Light[] disableLights,
        Color emissionColor,
        Color fogColor,
        Color ambientColor,
        Volume dayVol,
        Volume nightVol)
    {
        // LIGHTMAPS
        if (levelLightmapData != null)
        {
            levelLightmapData.LoadLightingScenario(lightmapIndex);

            // helps prevent stale lighting in some edge cases
            DynamicGI.UpdateEnvironment();

        }

        // LIGHTS
        SetLights(enableLights, true);
        SetLights(disableLights, false);

        // EMISSION
        SetEmission(emissionColor);

        // FOG (GLOBAL)
        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;

        // AMBIENT LIGHT
        RenderSettings.ambientLight = ambientColor;

        // VOLUMES
        if (dayVol) dayVol.weight = (lightmapIndex == 0) ? 1f : 0f;
        if (nightVol) nightVol.weight = (lightmapIndex == 1) ? 1f : 0f;
    }

    private void SetLights(Light[] lights, bool state)
    {
        if (lights == null) return;

        foreach (var light in lights)
        {
            if (light)
                light.enabled = state;
        }
    }

    private void SetEmission(Color color)
    {
        // MATERIAL ARRAY PATH (your original system)
        if (emissiveMaterials != null)
        {
            foreach (var mat in emissiveMaterials)
            {
                if (!mat) continue;
                ApplyEmission(mat, color);
            }
        }

        // RENDERER PATH (more scalable, recommended)
        if (emissiveRenderers != null)
        {
            foreach (var r in emissiveRenderers)
            {
                if (!r) continue;

                foreach (var mat in r.materials)
                {
                    ApplyEmission(mat, color);
                }
            }
        }
    }

    private void ApplyEmission(Material mat, Color color)
    {
        if (!mat) return;

        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color);
    }

    private void RefreshLightingProbes()
    {
        LightProbes.Tetrahedralize();
        DynamicGI.UpdateEnvironment();
    }
    private void RefreshReflectionProbes()
    {
        var probes = FindObjectsOfType<ReflectionProbe>();

        foreach (var probe in probes)
        {
            if (probe.mode == ReflectionProbeMode.Realtime)
            {
                probe.RenderProbe();
            }
        }
    }
}