using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class HUD
    {
        //
        List<TgcSprite> sprites;
        TgcSprite velocimetro;
        TgcSprite agujaVelocimetro;
        TgcSprite llenadoNitro;
        TgcSprite barraNitro;
        TgcSprite agujaTacometro;
        //
        Size screenSize;
        Size textureSizeVelocimetro;
        Size textureSizeAgujaVelocimetro;
        

        public HUD()
        {
            
            screenSize = GuiController.Instance.Panel3d.Size;
            velocimetro = new TgcSprite();
            agujaVelocimetro = new TgcSprite();
            barraNitro = new TgcSprite();
            llenadoNitro = new TgcSprite();            
            agujaTacometro = new TgcSprite();
            sprites = new List<TgcSprite> { velocimetro, agujaVelocimetro, agujaTacometro, llenadoNitro, barraNitro };

        }

        public void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            
            velocimetro.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Sprites\\velocimetro.png");
            velocimetro.Position = new Vector2(screenSize.Width - 260, screenSize.Height - 260);
            
            agujaVelocimetro.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Sprites\\agujaVelocidad.png");
            agujaVelocimetro.Position = new Vector2(screenSize.Width - 260, screenSize.Height - 260);
            agujaVelocimetro.RotationCenter = new Vector2(128, 128);
            
            agujaTacometro.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Sprites\\agujaTacometro.png");
            agujaTacometro.Position = new Vector2(screenSize.Width - 260, screenSize.Height - 260);
            agujaTacometro.RotationCenter = new Vector2(68, 183.5f);

            barraNitro.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Sprites\\barraNitro.png");
            barraNitro.Position = new Vector2(10f, 12f);
            barraNitro.Scaling = new Vector2(0.75f, 1f);

            llenadoNitro.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Sprites\\llenadoNitro.png");
            llenadoNitro.Position = new Vector2(20f,20f);
            llenadoNitro.Scaling = new Vector2(0.75f,1f); 


                                   
            

        }

        public void render(float velocidad,float cantNitro)
        {
            
            agujaVelocimetro.Rotation = FastMath.Abs(velocidad) / 1000 * FastMath.PI/2;
            llenadoNitro.Scaling = new Vector2(0.60f*cantNitro,0.50f);
            //llenadoNitro.Color = Color.Black;

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            foreach(TgcSprite sprite in sprites){
                sprite.render();
            }
            llenadoNitro.render();
            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();
        }
    }
}
