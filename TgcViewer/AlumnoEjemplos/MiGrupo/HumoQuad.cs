using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class EmisorHumo
    {
        List<HumoParticula> quadPool;
        
        public void crearQuads(int cantidad)
        {

            quadPool = new List<HumoParticula>();

            for(int i=0;i<cantidad;i++)
            {

            HumoParticula unaParticula = new HumoParticula(20.0f-i);

            quadPool.Add(unaParticula);

            }
        }

        public void update(float elapsedTime, Vector3 normal, float rotacionAuto, Vector3 posicionAuto, float anguloDerrape, float direcGiroDerrape)
        {

            foreach (HumoParticula particula in quadPool)
            {            
            float rohumo, alfa_humo;
            float posicion_xhumo;
            float posicion_yhumo;
            rohumo = FastMath.Sqrt(-19f * -19f + 126f * 126f);

            alfa_humo = FastMath.Asin(-19f / rohumo);
            posicion_xhumo = FastMath.Sin(alfa_humo + rotacionAuto + (anguloDerrape * direcGiroDerrape)) * rohumo;
            posicion_yhumo = FastMath.Cos(alfa_humo + rotacionAuto + (anguloDerrape * direcGiroDerrape)) * rohumo;

            particula.setPosicion (new Vector3(posicion_xhumo, 15.5f, posicion_yhumo) + posicionAuto);
  

            particula.disminuirVida(4*elapsedTime);
            
            if (particula.tiempoDeVida < 0.0f)
                {
                particula.resetear(20.0f);
                }

            particula.setNormal(normal);

            particula.calcularAlpha();
            }
        }
        public void render()
        {
            foreach (HumoParticula particula in quadPool) { 
            particula.render();
            }
        }

   }


}

