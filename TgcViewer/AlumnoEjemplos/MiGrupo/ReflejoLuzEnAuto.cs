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
            
        }

        private void SetValuesToMesh()
        {

            Vector3 eyePosition = GuiController.Instance.FpsCamera.getPosition();
            //Cargar variables shader de la luz
            try
            { 
                meshDeAuto.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                meshDeAuto.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(new Vector3(-10000, 700, 11880)));
                meshDeAuto.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(eyePosition));
                meshDeAuto.Effect.SetValue("lightIntensity", (float)4500);
                meshDeAuto.Effect.SetValue("lightAttenuation", (float)0.9f);
                meshDeAuto.Effect.SetValue("bumpiness", (float)1f);
                
                meshDeAuto.Effect.SetValue("materialSpecularExp", 9f);
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
