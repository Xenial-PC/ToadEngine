using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

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
        ShadowCaster = Component.Add<ShadowCaster>();
        ShadowCaster.IsCastingShadows = true;
    }

    public override void Setup()
    {
        LightIndex++;
        CurrentIndex = LightIndex - 1;
    }

    public override void Draw()
    {
        CoreShader.Use();
        CoreShader.SetInt1("spotLightAmount", SpotLight.LightIndex);
        CoreShader.SetInt1("pointLightAmount", PointLight.LightIndex);

        UpdateModelMatrix();

        var camera = Service.MainCamera;

        CoreShader.SetMatrix4("model", Model);
        CoreShader.SetMatrix4("view", camera.GetViewMatrix());
        CoreShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        CoreShader.SetVector3("viewPos", camera.Transform.LocalPosition);

        CoreShader.SetVector3($"spotLights[{CurrentIndex}].position", Settings.Position);
        CoreShader.SetVector3($"spotLights[{CurrentIndex}].direction", Settings.Direction);

        CoreShader.SetFloat1($"spotLights[{CurrentIndex}].cutOff", Settings.CutOff);
        CoreShader.SetFloat1($"spotLights[{CurrentIndex}].outerCutOff", Settings.OuterCutOff);

        CoreShader.SetFloat1($"spotLights[{CurrentIndex}].constant", Settings.Constant);
        CoreShader.SetFloat1($"spotLights[{CurrentIndex}].linear", Settings.Linear);
        CoreShader.SetFloat1($"spotLights[{CurrentIndex}].quadratic", Settings.Quadratic);

        CoreShader.SetVector3($"spotLights[{CurrentIndex}].ambient", Settings.Ambient);
        CoreShader.SetVector3($"spotLights[{CurrentIndex}].diffuse", Settings.Diffuse);
        CoreShader.SetVector3($"spotLights[{CurrentIndex}].specular", Settings.Specular);
    }

    public override void Update()
    {
    }

    public override void Dispose()
    {
        LightIndex--;
    }
}
