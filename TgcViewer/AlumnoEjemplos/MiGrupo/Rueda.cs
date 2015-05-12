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
    /// Probando Rueda
    /// </summary>
    public class ProbandoRueda : TgcExample
    {
        TgcMesh mainMesh;
        TgcBox box;
        TgcBox obstaculoDePrueba;
        float prevCameraRotation = 90;
        Auto auto;
        Jugador jugador;
        TgcObb oBBAuto, oBBObstaculoPrueba;


        public override string getCategory()
        {
            return "Otros";
        }

        public override string getName()
        {
            return "Probando Rueda";
        }

        public override string getDescription()
        {
            return "Ejemplo para ir probando el movimiento de la rueda del auto.";
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

            //Inicializamos texturas
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Pista\\pistaCarreras.png");
            TgcTexture texturaMadera = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Madera\\A3d-Fl3.jpg");

            //Creamos una caja 3D de color rojo, ubicada en el origen y lado 10
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(16000, 3, 7660);
            box = TgcBox.fromSize(center, size, texture);


            //En este ejemplo primero cargamos una escena 3D entera.
            TgcSceneLoader loader = new TgcSceneLoader();

            //Luego cargamos otro modelo aparte que va a hacer el objeto que controlamos con el teclado
            TgcScene scene2 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda-TgcScene.xml");

            //Creo un obstaculo de prueba de colsiones y demás
            obstaculoDePrueba = TgcBox.fromSize(new Vector3(0f, 0f, -500f), new Vector3(200, 200, 200), texturaMadera);
            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBObstaculoPrueba = TgcObb.computeFromAABB(obstaculoDePrueba.BoundingBox);

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];

            //Vamos a utilizar la cámara en 3ra persona para que siga al objeto principal a medida que se mueve
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            mainMesh.Position = new Vector3(0f, 0f, -900f);
            mainMesh.rotateY(90);
            GuiController.Instance.ThirdPersonCamera.RotationY = 90;
            GuiController.Instance.ThirdPersonCamera.setCamera(mainMesh.Position, 200, 500);
            GuiController.Instance.BackgroundColor = Color.Black;

            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBAuto = TgcObb.computeFromAABB(mainMesh.BoundingBox);


            //creo al auto y al jugador
            //auto = new Auto(90);
            auto = new Auto(90);
            jugador = new Jugador(auto);

            ///////////////MODIFIERS//////////////////
            GuiController.Instance.Modifiers.addFloat("velocidadMaxima", 1000, 7000, 1000f);

        }




        public override void render(float elapsedTime)
        {
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Pista\\pistaCarreras.png");

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Le paso el elapsed time al auto porque sus metodos no deben depender de los FPS
            auto.elapsedTime = elapsedTime;

            //Varío la velocidad Máxima del vehículo con el modifier "velocidadMáxima" 
            auto.establecerVelocidadMáximaEn((float)GuiController.Instance.Modifiers["velocidadMaxima"]);

            //El jugador envia mensajes al auto dependiendo de que tecla presiono
            jugador.jugar();

            //Transfiero la rotacion del auto abstracto al mesh, y su obb
            mainMesh.Rotation = new Vector3(0f, auto.rotacion, 0f);
            oBBAuto.Center = mainMesh.Position;
            oBBAuto.setRotation(mainMesh.Rotation);

            //Calculo el movimiento del mesh dependiendo de la velocidad del auto
            mainMesh.moveOrientedY(-auto.velocidad * elapsedTime);

            //Detección de colisiones
            bool collisionFound = false;

            //Hubo colisión con un objeto. Guardar resultado y abortar loop.
            if (Colisiones.testObbObb2(oBBAuto, oBBObstaculoPrueba))
            {
                collisionFound = true;
            }


            //Si hubo alguna colisión, hacer esto:
            if (collisionFound)
            {
                mainMesh.moveOrientedY(20 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                auto.velocidad = -(auto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
            }

            GuiController.Instance.ThirdPersonCamera.Target = mainMesh.Position;

            //Ajusto la camara a menos de 360 porque voy a necesitar hacer calculos entre angulos
            while (prevCameraRotation > 360)
            {
                prevCameraRotation -= -360;
            }

            //La camara no rota exactamente a la par del auto, hay un pequeño retraso
            GuiController.Instance.ThirdPersonCamera.RotationY += 5 * (mainMesh.Rotation.Y - prevCameraRotation) * elapsedTime;
            prevCameraRotation = GuiController.Instance.ThirdPersonCamera.RotationY;

            //Dibujar objeto principal
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            mainMesh.render();
            box.render();

            obstaculoDePrueba.render();
            //Hago visibles los obb
            oBBAuto.render();
            oBBObstaculoPrueba.render();


        }

        public override void close()
        {
            box.dispose();
            mainMesh.dispose();

            obstaculoDePrueba.dispose();
            oBBObstaculoPrueba.dispose();
            oBBAuto.dispose();

        }

    }
}


