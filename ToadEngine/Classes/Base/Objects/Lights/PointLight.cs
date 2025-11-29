using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;

namespace ToadEngine.Classes.Base.Objects.Lights;

public class PointLight : GameObject
{
    public static int LightIndex;
    public int CurrentIndex;
    public ShadowCaster ShadowCaster = null!;

    public BaseLight.PointLight Settings = new()
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
        ShadowCaster = Component.Add<ShadowCaster>();
        ShadowCaster.IsCastingShadows = true;
    }

    public override void Setup()
    {
        LightIndex++;
        CurrentIndex = LightIndex - 1;
    }

    public override void Draw(float deltaTime)
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

        CoreShader.SetVector3($"pointLights[{CurrentIndex}].position", Settings.Position);
        
        CoreShader.SetFloat1($"pointLights[{CurrentIndex}].constant", Settings.Constant);
        CoreShader.SetFloat1($"pointLights[{CurrentIndex}].linear", Settings.Linear);
        CoreShader.SetFloat1($"pointLights[{CurrentIndex}].quadratic", Settings.Quadratic);

        CoreShader.SetVector3($"pointLights[{CurrentIndex}].ambient", Settings.Ambient);
        CoreShader.SetVector3($"pointLights[{CurrentIndex}].diffuse", Settings.Diffuse);
        CoreShader.SetVector3($"pointLights[{CurrentIndex}].specular", Settings.Specular);
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Dispose()
    {
        LightIndex--;
    }
}
