using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Scripting.Renderer;

namespace ToadEngine.Classes.Base.Objects.Lights;

public class PointLight : Light
{
    public static int LightIndex;
    public int CurrentIndex;

    public ShadowCaster ShadowCaster = null!;
    public LightRenderer LightRenderer = null!;

    public PointLight Settings = new()
    {
        Position = new Vector3(-0.2f, -1.0f, -0.3f),

        Ambient = new Vector3(0.2f),
        Diffuse = new Vector3(0.5f),
        Specular = new Vector3(1.0f),

        Constant = 1.0f,
        Linear = 0.09f,
        Quadratic = 0.032f
    };

    public void AddShadowCaster()
    {
        ShadowCaster = AddComponent<ShadowCaster>();
        ShadowCaster.IsCastingShadows = false;
    }

    public void Awake()
    {
        LightIndex++;
        CurrentIndex = LightIndex - 1;

        LightRenderer = AddComponent<LightRenderer>();
        LightRenderer.Settings = Settings;
        LightRenderer.CurrentIndex = CurrentIndex;
    }
}
