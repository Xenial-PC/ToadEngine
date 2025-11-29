using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering;

namespace ToadEngine.Classes.Base.Objects.Lights;

public class SpotLight : GameObject
{
    public static int LightIndex;
    public int CurrentIndex;
    public ShadowCaster ShadowCaster = null!;

    public BaseLight.SpotLight Settings = new()
    {
        Position = new Vector3(-0.2f, -1.0f, -0.3f),
        Direction = new Vector3(0f, 0f, 0f),

        Ambient = new Vector3(0.2f),
        Diffuse = new Vector3(0.5f),
        Specular = new Vector3(1.0f),

        CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
        OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(17.5f)),

        Constant = 1.0f,
        Linear = 0.09f,
        Quadratic = 0.032f
    };

    public void AddShadowCaster()
    {
        ShadowCaster = AddComponent<ShadowCaster>();
        ShadowCaster.IsCastingShadows = true;
    }

    public override void Setup()
    {
        LightIndex++;
        CurrentIndex = LightIndex - 1;
    }

    public override void Draw(float deltaTime)
    {
        GetCoreShader().Use();
        GetCoreShader().SetInt1("spotLightAmount", SpotLight.LightIndex);
        GetCoreShader().SetInt1("pointLightAmount", PointLight.LightIndex);

        UpdateModelMatrix();

        var camera = Service.MainCamera;

        GetCoreShader().SetMatrix4("model", Obj.Model);
        GetCoreShader().SetMatrix4("view", camera.GetViewMatrix());
        GetCoreShader().SetMatrix4("projection", camera.GetProjectionMatrix());
        GetCoreShader().SetVector3("viewPos", camera.Transform.LocalPosition);

        GetCoreShader().SetVector3($"spotLights[{CurrentIndex}].position", Settings.Position);
        GetCoreShader().SetVector3($"spotLights[{CurrentIndex}].direction", Settings.Direction);

        GetCoreShader().SetFloat1($"spotLights[{CurrentIndex}].cutOff", Settings.CutOff);
        GetCoreShader().SetFloat1($"spotLights[{CurrentIndex}].outerCutOff", Settings.OuterCutOff);

        GetCoreShader().SetFloat1($"spotLights[{CurrentIndex}].constant", Settings.Constant);
        GetCoreShader().SetFloat1($"spotLights[{CurrentIndex}].linear", Settings.Linear);
        GetCoreShader().SetFloat1($"spotLights[{CurrentIndex}].quadratic", Settings.Quadratic);

        GetCoreShader().SetVector3($"spotLights[{CurrentIndex}].ambient", Settings.Ambient);
        GetCoreShader().SetVector3($"spotLights[{CurrentIndex}].diffuse", Settings.Diffuse);
        GetCoreShader().SetVector3($"spotLights[{CurrentIndex}].specular", Settings.Specular);
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Dispose()
    {
        LightIndex--;
    }
}
