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
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class ProbandoMovAuto : TgcExample
    {
        TgcMesh autoMesh;
        TgcMesh ruedaDerechaDelanteraMesh;
        TgcMesh ruedaDerechaTraseraMesh;
        TgcMesh ruedaIzquierdaDelanteraMesh;
        TgcMesh ruedaIzquierdaTraseraMesh;
        Vector3 ruedaDerechaDelanteraPos;
        Vector3 ruedaDerechaTraseraPos;
        Vector3 ruedaIzquierdaDelanteraPos;
        Vector3 ruedaIzquierdaTraseraPos;
        float rotacionVertical;
        float autoMeshPrevZ;
        float autoMeshPrevX;
        TgcBox box;
        TgcBox obstaculoDePrueba;
        float prevCameraRotation=90;
        Auto auto;
        Jugador jugador;
        TgcObb oBBAuto, oBBObstaculoPrueba;
        variablesEnPantalla textoVelocidad = new variablesEnPantalla();
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        List<Vector3> posiciones;

        //texto
        TgcText2d textPuntosDeControlAlcanzados;
        TgcText2d textPosicionDelAutoActual;
        //Creo un listado de puntos de control
        List<TgcCylinder> trayecto = new List<TgcCylinder>();
        //List<PuntoDeControl> puntosDelTrayecto = new List<PuntoDeControl>();
        int contadorDeActivacionesDePuntosDeControl = 0;
        
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

            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Pista\\pistaCarreras.png");
            TgcTexture texturaMadera = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Madera\\A3d-Fl3.jpg");

            //Creamos una caja 3D de color rojo, ubicada en el origen y lado 10
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(16000, 3, 7660);
            box = TgcBox.fromSize(center, size, texture);


            //En este ejemplo primero cargamos una escena 3D entera.
            TgcSceneLoader loader = new TgcSceneLoader();

            //Luego cargamos el auto y las ruedas aparte que va a hacer el objeto que controlamos con el teclado
            TgcScene scene1 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto-TgcScene.xml");
            TgcScene scene2 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene3 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene4 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");
            TgcScene scene5 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");



            //Creo un obstaculo de prueba de colisiones y demás
            obstaculoDePrueba = TgcBox.fromSize(new Vector3(0f, 0f, -500f), new Vector3(200, 200, 200), texturaMadera);
            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBObstaculoPrueba = TgcObb.computeFromAABB(obstaculoDePrueba.BoundingBox);

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            autoMesh = scene1.Meshes[0];
            ruedaDerechaDelanteraMesh = scene2.Meshes[0];
            ruedaDerechaTraseraMesh = scene3.Meshes[0];
            ruedaIzquierdaDelanteraMesh = scene4.Meshes[0];
            ruedaIzquierdaTraseraMesh = scene5.Meshes[0];
            //Los posicionamos
            autoMesh.Position = new Vector3(0f, 0f, -900f);
            //Inicilizo vectores posicion iniciales innecesarios
            ruedaDerechaDelanteraPos = new Vector3(0f, 44.5f, -700f);
            ruedaDerechaTraseraPos = new Vector3(-500f, 44.5f, -700f);
            ruedaIzquierdaDelanteraPos = new Vector3(0f, 44.5f, -1100f);
            ruedaIzquierdaTraseraPos = new Vector3(-500f, 44.5f, -1100f);

            //creo la lista de ruedas
            ruedas = new List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };
            posiciones = new List<Vector3> { ruedaDerechaDelanteraPos, ruedaDerechaTraseraPos, ruedaIzquierdaDelanteraPos, ruedaIzquierdaTraseraPos };
            //inicializamos posicion de ruedas
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].Position = posiciones[i];
                ruedas[i].rotateY(90);

            }

            //Vamos a utilizar la cámara en 3ra persona para que siga al objeto principal a medida que se mueve
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.RotationY = 90;
            GuiController.Instance.ThirdPersonCamera.setCamera(autoMesh.Position, 200, 500);                      
            GuiController.Instance.BackgroundColor = Color.Black;

            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBAuto = TgcObb.computeFromAABB(autoMesh.BoundingBox);
            

            //creo al auto y al jugador
            auto = new Auto(90);
            jugador = new Jugador(auto);

            //Creo un punto de control para probarlo
            for (int i = 0; i < 10;i++)
            {
                TgcCylinder unCilindro = new TgcCylinder(new Vector3(-300 - (i * 1000), 20, -1000 - (i * 300)), 100, 50);
                trayecto.Add(unCilindro);
                //puntosDelTrayecto.Add(new PuntoDeControl(false));
            }
            //Activo el primer punto de control
            //puntosDelTrayecto[0].activarPunto();

            /////////////TEXTOS///////////////////////
              
            textPuntosDeControlAlcanzados = new TgcText2d();
            textPuntosDeControlAlcanzados.Position = new Point(0, 50);
            textPuntosDeControlAlcanzados.Text = "Puntos De Control Alcanzados = ";
            textPuntosDeControlAlcanzados.Color = Color.White;

            textPosicionDelAutoActual = new TgcText2d();
            textPosicionDelAutoActual.Text = "Posicion del auto actual = ";
            textPosicionDelAutoActual.Color = Color.White;
            textPosicionDelAutoActual.Position = new Point(100, 450);


            textoVelocidad.inicializarTextoVelocidad(auto.velocidad);
           
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

            rotacionVertical -= auto.velocidad * elapsedTime / 20;


            //Transfiero la rotacion del auto abstracto al mesh, y su obb
            autoMesh.Rotation = new Vector3(0f, auto.rotacion, 0f);
            oBBAuto.Center = autoMesh.Position;
            oBBAuto.setRotation(autoMesh.Rotation);

            for (int i = 0; i < 4; i++)
            {
                ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion, 0f);
                ruedas[i].move((autoMesh.Position.X - autoMeshPrevX), 0, (autoMesh.Position.Z - autoMeshPrevZ));
            }
            autoMeshPrevX = autoMesh.Position.X;
            autoMeshPrevZ = autoMesh.Position.Z;

            

            //Calculo el movimiento del mesh dependiendo de la velocidad del auto
            autoMesh.moveOrientedY(-auto.velocidad * elapsedTime);

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
                autoMesh.moveOrientedY(20 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                auto.velocidad = -(auto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
            }

            GuiController.Instance.ThirdPersonCamera.Target = autoMesh.Position;

            //Ajusto la camara a menos de 360 porque voy a necesitar hacer calculos entre angulos
            while (prevCameraRotation > 360)
            {
                prevCameraRotation -= -360;
            }

            //La camara no rota exactamente a la par del auto, hay un pequeño retraso
            GuiController.Instance.ThirdPersonCamera.RotationY += 5 * (autoMesh.Rotation.Y - prevCameraRotation) * elapsedTime;
            prevCameraRotation = GuiController.Instance.ThirdPersonCamera.RotationY;

            //Dibujar objeto principal
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            autoMesh.render();
            box.render();
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].render();
            }
            obstaculoDePrueba.render();
            //Hago visibles los obb
            oBBAuto.render();
            oBBObstaculoPrueba.render();

            //Muestro todo el trayecto de puntos de control
            /*for(int i=0;i<trayecto.Count;i++)
            {
                trayecto[i].render();
                trayecto[i].BoundingCylinder.render();
            }*/
            //Muestro el punto siguiente
            trayecto[0].render();
            

            //Colision con puntos de control
            for (int i = 0; i < trayecto.Count; i++)
            {
                //Pregunto si colisiona con un punto de control activado
                if ( (i == 0) && TgcCollisionUtils.testPointCylinder(oBBAuto.Position, trayecto[i].BoundingCylinder))
                {
                    TgcCylinder cilindroModificado = new TgcCylinder(trayecto[i].Center, 200, 30);

                    trayecto[1].UseTexture = true;

                    trayecto.RemoveAt(i);
                    trayecto.Add(cilindroModificado);
                    contadorDeActivacionesDePuntosDeControl++;
                    textPuntosDeControlAlcanzados.Text = "Puntos De Control Alcanzados = " + contadorDeActivacionesDePuntosDeControl.ToString();
                }
            }
            textPosicionDelAutoActual.Text = autoMesh.Position.ToString();

            //Renderizar los tres textoss

            textoVelocidad.mostrarVelocidad(auto.velocidad/10).render(); //renderiza la velocidad 
          
            textPuntosDeControlAlcanzados.render();
            textPosicionDelAutoActual.render();

        }
         
        public override void close()
        {
            box.dispose();
            autoMesh.dispose();
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].dispose();
            }
            obstaculoDePrueba.dispose();
            oBBObstaculoPrueba.dispose();
            oBBAuto.dispose();

            //borro los puntos de control del trayecto
            for (int i = 0; i < trayecto.Count; i++)
            {
                trayecto[i].dispose();
                trayecto[i].BoundingCylinder.dispose();
            }
            trayecto.Clear();
   
            //Liberar textos

            textPuntosDeControlAlcanzados.dispose();
            textPosicionDelAutoActual.dispose();

        }
    }
}
    

