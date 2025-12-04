using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Scripting.Renderer;

namespace ToadEngine.Classes.Base.Objects.Lights;

public class DirectionLight : GameObject
{
    public ShadowCaster ShadowCaster = null!;
    public LightRenderer LightRenderer = null!;

    public BaseLight.DirectionLight Settings = new()
    {
        Direction = new Vector3(0f, 0f, 0f),
        Ambient = new Vector3(1.8f),
        Diffuse = new Vector3(0.8f),
        Specular = new Vector3(1.0f),
    };

    private void AddShadowCaster()
    {
        ShadowCaster = Component.Add<ShadowCaster>();
        ShadowCaster.IsCastingShadows = true;
        ShadowCaster.Distance = 80f;
        ShadowCaster.SceneSize = 50f;
        ShadowCaster.NearPlane = 1f;
        ShadowCaster.FarPlane = 200f;
    }

    public override void Setup()
    {
        LightRenderer = AddComponent<LightRenderer>();
        LightRenderer.Settings = Settings;
        AddShadowCaster();
    }
}
