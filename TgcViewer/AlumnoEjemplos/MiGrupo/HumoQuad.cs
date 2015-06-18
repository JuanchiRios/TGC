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
        Random random;
        TgcTexture texturaHumo;
        TgcTexture texturaFuego;

        public EmisorHumo()
        { 
            texturaHumo = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\humo.png");
            texturaFuego = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\fuego.png");
        }


        public void crearQuads(int cantidad)
        
        {
            random = new Random(0);
            quadPool = new List<HumoParticula>();

            for(int i=0;i<cantidad;i++)
            {
            int random1 = random.Next(i)-(i/2);
            int random2 = random.Next(i)-(i/2);
            List<float> posicionesRelativas = new List<float>{-19f+random1,126f+random2};
            HumoParticula unaParticula = new HumoParticula(3.0f - i * 0.15f, posicionesRelativas);
            unaParticula.setTextura(TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\humo.png"));

            quadPool.Add(unaParticula);

            }
        }

        public void update(float elapsedTime, Vector3 normal, float rotacionAuto, Vector3 posicionAuto, float anguloDerrape, float direcGiroDerrape, bool nitro, float autoVelocidad)
        {
            int i = 0;
            foreach (HumoParticula particula in quadPool)
            {            
            float rohumo, alfa_humo;
            float posicion_xhumo;
            float posicion_yhumo;
            rohumo = FastMath.Sqrt(-particula.posX * -particula.posX + particula.posZ * particula.posZ);

            alfa_humo = FastMath.Asin(particula.posX / rohumo);
            posicion_xhumo = FastMath.Sin(alfa_humo + rotacionAuto + (anguloDerrape * direcGiroDerrape)) * rohumo;
            posicion_yhumo = FastMath.Cos(alfa_humo + rotacionAuto + (anguloDerrape * direcGiroDerrape)) * rohumo;

            

            particula.setPosicion(new Vector3(posicion_xhumo, 16.0f, posicion_yhumo) + posicionAuto);
            i++;

            particula.disminuirVida(4 * elapsedTime, FastMath.Max(0.3f, autoVelocidad / 300f));

            if (nitro)
            {
                
                particula.setTextura(TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\fuego.png"));
            }
            else
            {

                particula.setTextura(TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\humo.png"));
            }
            
            if (particula.tiempoDeVida < 0.0f)
                {
                int random1 = random.Next(10) - 5;
                int random2 = random.Next(30) - 15;
                particula.resetear(3.0f,new List<float> { -19f + random1, 126f + random2 });               
                }

            particula.setNormal(new Vector3(0,rotacionAuto,0));

            particula.calcularAlpha();
            particula.calcularTamanio();
            }
        }
        public void render(Vector3 posCamara)
        {
            foreach (HumoParticula particula in quadPool) { 
            particula.render(posCamara);
            }
        }

   }


}

