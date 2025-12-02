using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

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
        ShadowCaster = Component.Add<ShadowCaster>();
        ShadowCaster.IsCastingShadows = true;
        ShadowCaster.Distance = 80f;
        ShadowCaster.SceneSize = 50f;
        ShadowCaster.NearPlane = 1f;
        ShadowCaster.FarPlane = 200f;
    }

    public override void Draw()
    {
        CoreShader.Use();
        CoreShader.SetInt1("spotLightAmount", SpotLight.LightIndex);
        CoreShader.SetInt1("pointLightAmount", PointLight.LightIndex);

        UpdateModelMatrix();

        var camera = Service.Get<Camera>()!;

        CoreShader.SetMatrix4("model", Model);
        CoreShader.SetMatrix4("view", camera.GetViewMatrix());
        CoreShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        CoreShader.SetVector3("viewPos", camera.Transform.LocalPosition);

        CoreShader.SetVector3($"dirLight.direction", Settings.Direction);
        CoreShader.SetVector3($"dirLight.ambient", Settings.Ambient);
        CoreShader.SetVector3($"dirLight.diffuse", Settings.Diffuse);
        CoreShader.SetVector3($"dirLight.specular", Settings.Specular);
    }

    public override void Update()
    {
        
    }

    public override void Dispose()
    {
        
    }
}
