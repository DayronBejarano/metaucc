using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Edificio UCC Villavicencio (modelo SketchUp). Se aplica sola en cada escena (no hay que agregarla a ningún objeto):
///  - Edificio con caras dobles: el modelo de SketchUp tiene caras de un solo lado; desde dentro
///    de salones y pasillos se veía "a través" de muros y techos. Se cambian sus materiales por
///    el shader Tesis/DobleCara (mismo color y textura).
///  - La física también choca con la cara trasera de los muros (Physics.queriesHitBackfaces),
///    para que los puntos de aparición y los paneles detecten las paredes desde cualquier lado.
///  - Cielo, luz ambiente y niebla suave para que el espacio no se vea plano.
///  - Plano cercano de la cámara a 5 cm (menos recorte al acercarse a una pared).
/// </summary>
public static class AjustesVisuales
{
    /// <summary>Nombres de los objetos raíz del edificio en la escena.</summary>
    public static readonly string[] NombresEdificio = { "UCC_CONPUPITRES", "UCC_SINPUPITRES" };
    // Cielo, luz ambiente y niebla al cargar cada escena (ponlo en false si tu escena ya tiene su iluminación)
    public static bool aplicarAmbiente = true;
    static readonly Dictionary<Material, Material> cache = new Dictionary<Material, Material>();
    static Shader dobleCara;
    static Material cielo;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Iniciar()
    {
        SceneManager.sceneLoaded -= AlCargar;
        SceneManager.sceneLoaded += AlCargar;
        Aplicar(aplicarAmbiente);
    }

    static void AlCargar(Scene s, LoadSceneMode m) => Aplicar(aplicarAmbiente);

    public static void Aplicar(bool conAmbiente = false)
    {
        Physics.queriesHitBackfaces = true;
        EdificioDobleCara();
        if (conAmbiente) Ambiente();
        var cam = Camera.main;
        if (cam != null && cam.nearClipPlane > 0.05f) cam.nearClipPlane = 0.05f;
    }

    public static int EdificioDobleCara()
    {
        int total = 0;
        foreach (var nombre in NombresEdificio) total += EdificioDobleCara(GameObject.Find(nombre));
        return total;
    }

    public static int EdificioDobleCara(GameObject edificio)
    {
        if (edificio == null) return 0;
        if (dobleCara == null) dobleCara = Shader.Find("Tesis/DobleCara");
        if (dobleCara == null) { Debug.LogWarning("[AjustesVisuales] Falta el shader Tesis/DobleCara (Assets/Resources)."); return 0; }

        int n = 0;
        foreach (var r in edificio.GetComponentsInChildren<Renderer>(true))
        {
            var mats = r.sharedMaterials;
            bool cambio = false;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                if (m == null || m.shader == dobleCara) continue;
                if (m.renderQueue > 2500) continue; // vidrios/transparentes: se dejan como están
                if (!cache.TryGetValue(m, out var nuevo) || nuevo == null)
                {
                    nuevo = new Material(dobleCara) { name = m.name + " (doble cara)", enableInstancing = true };
                    if (m.HasProperty("_Color")) nuevo.color = m.color;
                    if (m.HasProperty("_MainTex") && m.mainTexture != null)
                    {
                        nuevo.mainTexture = m.mainTexture;
                        nuevo.mainTextureScale = m.mainTextureScale;
                        nuevo.mainTextureOffset = m.mainTextureOffset;
                    }
                    cache[m] = nuevo;
                }
                mats[i] = nuevo; cambio = true;
            }
            if (cambio) { r.sharedMaterials = mats; n++; }
        }
        return n;
    }

    /// <summary>Cielo, luz ambiente en tres tonos y niebla suave.</summary>
    public static void Ambiente()
    {
        if (RenderSettings.skybox == null)
        {
            if (cielo == null)
            {
                var sh = Shader.Find("Skybox/Procedural");
                if (sh != null)
                {
                    cielo = new Material(sh);
                    cielo.SetFloat("_SunSize", 0.03f);
                    cielo.SetFloat("_AtmosphereThickness", 0.9f);
                    cielo.SetColor("_SkyTint", new Color(0.55f, 0.65f, 0.85f));
                    cielo.SetColor("_GroundColor", new Color(0.35f, 0.37f, 0.33f));
                    cielo.SetFloat("_Exposure", 1.1f);
                }
            }
            if (cielo != null) RenderSettings.skybox = cielo;
        }
        // Luz ambiente en tres tonos: techo claro, muros medios, piso más oscuro → da volumen
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.62f, 0.66f, 0.72f);
        RenderSettings.ambientEquatorColor = new Color(0.45f, 0.45f, 0.45f);
        RenderSettings.ambientGroundColor = new Color(0.25f, 0.24f, 0.22f);
        // Niebla suave: ayuda a leer la profundidad de los pasillos largos
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.72f, 0.76f, 0.82f);
        RenderSettings.fogStartDistance = 18f;
        RenderSettings.fogEndDistance = 90f;
        DynamicGI.UpdateEnvironment();
        foreach (var c in Object.FindObjectsOfType<Camera>())
            if (c.clearFlags == CameraClearFlags.SolidColor && c.cameraType == CameraType.Game && c.targetTexture == null)
                c.clearFlags = CameraClearFlags.Skybox;
    }
}
