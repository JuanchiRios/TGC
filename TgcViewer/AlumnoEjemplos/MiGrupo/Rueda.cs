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
        TgcMesh ruedaDerechaDelanteraMesh;
        TgcMesh ruedaDerechaTraseraMesh;
        TgcMesh ruedaIzquierdaDelanteraMesh;
        TgcMesh ruedaIzquierdaTraseraMesh;
        TgcMesh autoMesh;
        TgcBox box;
        TgcBox obstaculoDePrueba;
        float prevCameraRotation = 90;
        Auto auto;
        Jugador jugador;
        TgcObb oBBAuto, oBBObstaculoPrueba;
        float rotacionVertical;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;

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
            TgcScene scene3 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda-TgcScene.xml");
            TgcScene scene4 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda-TgcScene.xml");
            TgcScene scene5 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda-TgcScene.xml");
            TgcScene scene6 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto-TgcScene.xml");

            //Creo un obstaculo de prueba de colsiones y demás
            obstaculoDePrueba = TgcBox.fromSize(new Vector3(0f, 0f, -500f), new Vector3(200, 200, 200), texturaMadera);
            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBObstaculoPrueba = TgcObb.computeFromAABB(obstaculoDePrueba.BoundingBox);

            //El modelo de las cuatro ruedas
            ruedaDerechaDelanteraMesh = scene2.Meshes[0];
            ruedaDerechaTraseraMesh = scene3.Meshes[0];
            ruedaIzquierdaDelanteraMesh = scene4.Meshes[0];
            ruedaIzquierdaTraseraMesh = scene5.Meshes[0];
            autoMesh = scene6.Meshes[0];
            //Son ruedas izquierdas asi que las roto
            ruedaIzquierdaDelanteraMesh.rotateY(180);
            ruedaIzquierdaTraseraMesh.rotateY(180);

            //creo la lista de ruedas
            ruedas = new List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };
           
            //inicializamos posicion de ruedas
            ruedaDerechaDelanteraMesh.Position = new Vector3(0f, 44.5f, -900f);
            ruedaDerechaTraseraMesh.Position = new Vector3(-500f, 44.5f, -900f);
            ruedaIzquierdaDelanteraMesh.Position = new Vector3(0f, 44.5f, -1200f);
            ruedaIzquierdaTraseraMesh.Position = new Vector3(-500f, 44.5f, -1200f);
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].rotateY(90);
            }
            //Vamos a utilizar la cámara en 1ra persona
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 100, -700), (ruedaDerechaDelanteraMesh.Position));
            GuiController.Instance.BackgroundColor = Color.Black;

            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBAuto = TgcObb.computeFromAABB(ruedaDerechaDelanteraMesh.BoundingBox);


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



            rotacionVertical -= auto.velocidad * elapsedTime/20;
            //Transfiero la rotacion del auto abstracto al mesh, y su obb
            //Calculo el movimiento del mesh dependiendo de la velocidad del auto
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion, 0f); //El coeficienteDeZurdez es para que las ruedas de la izquierda miren para la izquierda y aun asi se muevan para adelante
                ruedas[i].moveOrientedY(-auto.velocidad *elapsedTime);
                if (i == 0)
                {
                    oBBAuto.Center = ruedaDerechaDelanteraMesh.Position;
                    oBBAuto.setRotation(ruedaDerechaDelanteraMesh.Rotation);
                }
            }


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
                ruedaDerechaDelanteraMesh.moveOrientedY(20 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                auto.velocidad = -(auto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
            }

            //GuiController.Instance.FpsCamera.Position = mainMesh.Position;
            /*
            //Ajusto la camara a menos de 360 porque voy a necesitar hacer calculos entre angulos
            while (prevCameraRotation > 360)
            {
                prevCameraRotation -= -360;
            }

            //La camara no rota exactamente a la par del auto, hay un pequeño retraso
            GuiController.Instance.ThirdPersonCamera.RotationY += 5 * (mainMesh.Rotation.Y - prevCameraRotation) * elapsedTime;
            prevCameraRotation = GuiController.Instance.ThirdPersonCamera.RotationY;
            */
            //Dibujar objeto principal
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].render();
            }
            box.render();

            obstaculoDePrueba.render();
            //Hago visibles los obb
            oBBAuto.render();
            oBBObstaculoPrueba.render();


        }

        public override void close()
        {
            box.dispose();
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].dispose();
            }

            obstaculoDePrueba.dispose();
            oBBObstaculoPrueba.dispose();
            oBBAuto.dispose();

        }

    }
}


