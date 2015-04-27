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
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class ProbandoMovAuto : TgcExample
    {
        const float MOVEMENT_SPEED = 10f;
        float velocidadActual = 0;
        TgcMesh mainMesh;
        TgcBox box;

        public override string getCategory()
        {
            return "Otros";
        }

        public override string getName()
        {
            return "Probando Mov Autos";
        }

        public override string getDescription()
        {
            return "Ejemplo para ir probando el movimiento del auto.";
        }

        public override void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //Cargamos una textura
            //Una textura es una imágen 2D que puede dibujarse arriba de un polígono 3D para darle color.
            //Es muy útil para generar efectos de relieves y superficies.
            //Puede ser cualquier imágen 2D (jpg, png, gif, etc.) y puede ser editada con cualquier editor
            //normal (photoshop, paint, descargada de goole images, etc).
            //El framework viene con un montón de texturas incluidas y organizadas en categorias (texturas de
            //madera, cemento, ladrillo, pasto, etc). Se encuentran en la carpeta del framework:
            //  TgcViewer\Examples\Media\MeshCreator\Textures
            //Podemos acceder al path de la carpeta "Media" utilizando la variable "GuiController.Instance.ExamplesMediaDir".
            //Esto evita que tengamos que hardcodear el path de instalación del framework.

            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Suelo\\adoquin2.jpg");

            //Creamos una caja 3D de color rojo, ubicada en el origen y lado 10
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(3000, 3, 3000);
            box = TgcBox.fromSize(center, size, texture);


            //En este ejemplo primero cargamos una escena 3D entera.
            TgcSceneLoader loader = new TgcSceneLoader();

            //Luego cargamos otro modelo aparte que va a hacer el objeto que controlamos con el teclado
            TgcScene scene2 = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];

            //Vamos a utilizar la cámara en 3ra persona para que siga al objeto principal a medida que se mueve
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(mainMesh.Position, 200, 500);
        }


        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;


            //Procesamos input de teclado para mover el objeto principal en el plano XZ
            TgcD3dInput input = GuiController.Instance.D3dInput;
            Vector3 movement = new Vector3(0, 0, 0);

            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                mainMesh.rotateY(-(velocidadActual / 500) * elapsedTime);
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                mainMesh.rotateY((velocidadActual / 500) * elapsedTime);
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                velocidadActual = velocidadActual + (500 * elapsedTime);
            }
            else if (input.keyDown(Key.L)) //L marcha atrás
            {
                velocidadActual = velocidadActual - (200 * elapsedTime);
            }

            if (velocidadActual > 0 && !input.keyDown(Key.Up) && !(input.keyDown(Key.W)))
                velocidadActual -= (1000f * elapsedTime);
            if (velocidadActual > 0 && (input.keyDown(Key.Down) || (input.keyDown(Key.S)))) //Para frenar en movimiento rápidamente
                velocidadActual -= (2000f * elapsedTime);
            if (velocidadActual < 0 && !input.keyDown(Key.Down) && !input.keyDown(Key.L) && !(input.keyDown(Key.S)))
                velocidadActual += (200f * elapsedTime);
            if (velocidadActual > 2000f)
                velocidadActual = 2000f;
            if (velocidadActual < -500f)
                velocidadActual = -500f;

         mainMesh.moveOrientedY(-velocidadActual * elapsedTime);
         //Aplicar movimiento
         /*
         movement *= MOVEMENT_SPEED * elapsedTime;
         mainMesh.move(movement);
         */
            //Hacer que la cámara en 3ra persona se ajuste a la nueva posición del objeto
            GuiController.Instance.ThirdPersonCamera.Target = mainMesh.Position;


            //Dibujar objeto principal
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            mainMesh.render();
            box.render();
        }

        public override void close()
        {
            box.dispose();
            mainMesh.dispose();
        }

    }
}
