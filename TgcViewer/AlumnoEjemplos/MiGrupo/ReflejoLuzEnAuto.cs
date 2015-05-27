using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class ReflejoLuzEnAuto
    {
        private static bool isConfigInitialize;
        private TgcMesh meshDeAuto;
        private Effect effect;

        public ReflejoLuzEnAuto(TgcMesh autoMesh)
        {
            meshDeAuto = autoMesh;
            effect = TgcShaders.loadEffect(GuiController.Instance.ExamplesDir + "Media\\Shaders\\BumpMapping.fx");
                ConfigReflection();
            isConfigInitialize = true;
        }
        public void Render()
        {
            SetValuesToMesh();
        }


        private void ConfigReflection()
        {
            meshDeAuto.Effect = effect;
            meshDeAuto.Technique = "BumpMappingTechnique";
            
            if (!isConfigInitialize)
            {
                GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-4000, -300, -5000),
                                                             new Vector3(4000, 1500, 4000), new Vector3(2000, 700, -4000));
                GuiController.Instance.Modifiers.addFloat("bumpiness", 0, 1, 1f);
                GuiController.Instance.Modifiers.addColor("lightColor", Color.White);
                GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 1500, 700);
                GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.3f);
                GuiController.Instance.Modifiers.addFloat("specularEx", 0, 20, 9f);
            }
        }

        private void SetValuesToMesh()
        {

            Vector3 lightPos = (Vector3)GuiController.Instance.Modifiers["LightPosition"];
            Vector3 eyePosition = GuiController.Instance.FpsCamera.getPosition();
            /*
             * 
             * 
             */
            //Cargar variables shader de la luz
            try
            {
                meshDeAuto.Effect.SetValue("lightColor",
                                          ColorValue.FromColor((Color) GuiController.Instance.Modifiers["lightColor"]));
                meshDeAuto.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightPos));
                meshDeAuto.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(eyePosition));
                meshDeAuto.Effect.SetValue("lightIntensity", (float)GuiController.Instance.Modifiers["lightIntensity"]);
                meshDeAuto.Effect.SetValue("lightAttenuation",
                                          (float) GuiController.Instance.Modifiers["lightAttenuation"]);
                meshDeAuto.Effect.SetValue("bumpiness", (float)GuiController.Instance.Modifiers["bumpiness"]);

                //Material
                meshDeAuto.Effect.SetValue("materialSpecularExp", (float)GuiController.Instance.Modifiers["specularEx"]);
                meshDeAuto.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Gray));
                meshDeAuto.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                meshDeAuto.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                meshDeAuto.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            }
            catch (Exception e)
            {
                //No tiro ninguna excepción específica
            }

        }
    }
}
