using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering;

namespace ToadEngine.Classes.Base.Objects.Lights;

public class DirectionLight : GameObject
{
    public ShadowCaster ShadowCaster = null!;

    public BaseLight.DirectionLight Settings = new()
    {
        Direction = new Vector3(0f, 0f, 0f),
        Ambient = new Vector3(1.8f),
        Diffuse = new Vector3(0.8f),
        Specular = new Vector3(1.0f),
    };

    public void AddShadowCaster()
    {
        ShadowCaster = AddComponent<ShadowCaster>();
        ShadowCaster.IsCastingShadows = true;
        ShadowCaster.Distance = 80f;
        ShadowCaster.SceneSize = 50f;
        ShadowCaster.NearPlane = 1f;
        ShadowCaster.FarPlane = 200f;
    }

    public override void Setup()
    {
        base.Setup();
    }

    public override void Draw(float deltaTime)
    {
        base.Draw(deltaTime);
        GetCoreShader().Use();
        GetCoreShader().SetInt1("spotLightAmount", SpotLight.LightIndex);
        GetCoreShader().SetInt1("pointLightAmount", PointLight.LightIndex);

        UpdateModelMatrix();

        var camera = GetService<Camera>()!;

        GetCoreShader().SetMatrix4("model", Obj.Model);
        GetCoreShader().SetMatrix4("view", camera.GetViewMatrix());
        GetCoreShader().SetMatrix4("projection", camera.GetProjectionMatrix());
        GetCoreShader().SetVector3("viewPos", camera.Transform.LocalPosition);

        GetCoreShader().SetVector3($"dirLight.direction", Settings.Direction);
        GetCoreShader().SetVector3($"dirLight.ambient", Settings.Ambient);
        GetCoreShader().SetVector3($"dirLight.diffuse", Settings.Diffuse);
        GetCoreShader().SetVector3($"dirLight.specular", Settings.Specular);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
