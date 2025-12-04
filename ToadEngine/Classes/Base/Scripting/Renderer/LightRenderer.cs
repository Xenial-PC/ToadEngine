using System.Reflection.PortableExecutable;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Scripting.Renderer
{
    public class LightRenderer : Behavior, IRenderObject
    {
        public dynamic Settings = null!;
        public int CurrentIndex;

        public void Draw()
        {
            if (GameObject == null) return;

            CoreShader.Use();
            CoreShader.SetInt1("spotLightAmount", SpotLight.LightIndex);
            CoreShader.SetInt1("pointLightAmount", PointLight.LightIndex);

            GameObject.UpdateModelMatrix();

            var camera = Service.MainCamera;

            CoreShader.SetMatrix4("model", GameObject.Model);
            CoreShader.SetMatrix4("view", camera.GetViewMatrix());
            CoreShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            CoreShader.SetVector3("viewPos", camera.Transform.LocalPosition);

            switch (GameObject)
            {
                case DirectionLight directionLight:
                    CoreShader.SetVector3($"dirLight.direction", Settings.Direction);
                    CoreShader.SetVector3($"dirLight.ambient", Settings.Ambient);
                    CoreShader.SetVector3($"dirLight.diffuse", Settings.Diffuse);
                    CoreShader.SetVector3($"dirLight.specular", Settings.Specular);
                    break;
                case PointLight pointLight:
                    CoreShader.SetVector3($"pointLights[{CurrentIndex}].position", Settings.Position);

                    CoreShader.SetFloat1($"pointLights[{CurrentIndex}].constant", Settings.Constant);
                    CoreShader.SetFloat1($"pointLights[{CurrentIndex}].linear", Settings.Linear);
                    CoreShader.SetFloat1($"pointLights[{CurrentIndex}].quadratic", Settings.Quadratic);

                    CoreShader.SetVector3($"pointLights[{CurrentIndex}].ambient", Settings.Ambient);
                    CoreShader.SetVector3($"pointLights[{CurrentIndex}].diffuse", Settings.Diffuse);
                    CoreShader.SetVector3($"pointLights[{CurrentIndex}].specular", Settings.Specular);
                    break;
                case SpotLight spotLight:

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
                    break;
            }
        }

        public override void OnDispose()
        {
            switch (GameObject)
            {
                case PointLight pointLight:
                    PointLight.LightIndex--;
                    break;
                case SpotLight spotLight:
                    SpotLight.LightIndex--;
                    break;
            }
        }
    }
}
