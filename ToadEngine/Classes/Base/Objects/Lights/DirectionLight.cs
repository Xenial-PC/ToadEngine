using ToadEngine.Classes.Base.Scripting.Renderer;

namespace ToadEngine.Classes.Base.Objects.Lights;

public class DirectionLight : Light
{
    public ShadowCaster ShadowCaster = null!;
    public LightRenderer LightRenderer = null!;

    public DirectionLight Settings = new()
    {
        Direction = new Vector3(0f, 0f, 0f),
        Ambient = new Vector3(1.8f),
        Diffuse = new Vector3(0.8f),
        Specular = new Vector3(1.0f),
    };

    private void AddShadowCaster()
    {
        ShadowCaster = AddComponent<ShadowCaster>();
        ShadowCaster.IsCastingShadows = true;
        ShadowCaster.Distance = 80f;
        ShadowCaster.SceneSize = 50f;
        ShadowCaster.NearPlane = 1f;
        ShadowCaster.FarPlane = 200f;
    }

    public void Awake()
    {
        LightRenderer = AddComponent<LightRenderer>();
        LightRenderer.Settings = Settings;
        AddShadowCaster();
    }
}
