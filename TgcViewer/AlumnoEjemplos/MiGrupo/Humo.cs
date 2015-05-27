using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using Microsoft.DirectX;

namespace AlumnoEjemplos.MiGrupo
{
    public class Humo
    {
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> humo;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh particulaDeHumo;
        TgcScene scene1;
        Vector3 posicionRelativa= new Vector3(0f,0f,0f);//posicion relativa de la primera


        public Humo(TgcSceneLoader loader){
            for(float y = 0; y<10; y++){
                for(float z = 0; z<10; z++){
                    for(float x = 0; x<10; x++){
                        scene1 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\humo-TgcScene.xml");
                        particulaDeHumo = scene1.Meshes[0];
                        posicionRelativa += new Vector3(x*0.1f, y*0.1f, z*0.1f);
                        
                        humo.Add(particulaDeHumo);
                    }
                }
            }
        }

        public void nitro(){

        }
        
        public void finDeNitro(){

        }

    }
}
