﻿using System;
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

        
        TgcBox box;
        TgcMesh autoMesh;
        TgcBox obstaculoDePrueba, fronteraDerecha, fronteraIzquierda,fronteraAdelante,fronteraAtras;
        TgcMesh ruedaDerechaDelanteraMesh;
        TgcMesh ruedaDerechaTraseraMesh;
        TgcMesh ruedaIzquierdaDelanteraMesh;
        TgcMesh ruedaIzquierdaTraseraMesh;
        float autoMeshPrevZ;
        float autoMeshPrevX;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        List<float> dx;
        List<float> dy;
        float rotacionVertical;
        float prevCameraRotation=300;
        Auto auto;
        Jugador jugador;
        TgcObb oBBAuto, oBBObstaculoPrueba, oBBfronteraDerecha, oBBfronteraIzquierda, oBBfronteraAdelante, oBBfronteraAtras;
        variablesEnPantalla textoVelocidad = new variablesEnPantalla();
        List<Vector3> posicionesPuntosDeControl;
        TgcCylinder unCilindro;

        TgcD3dInput input = GuiController.Instance.D3dInput;

        //texto
        TgcText2d textPuntosDeControlAlcanzados;
        TgcText2d textPosicionDelAutoActual;
        TgcText2d textTiempo;
        float contadorDeFrames = 0;
        private DateTime horaInicio;

        //Tiempo 
        public float tiempoTrans = 100f; //tiempo transcurrido desde el defasaje de rotacion de camara y rotacion del mesh
        private bool habilitarDecremento = false;
        Tiempo tiempo = new Tiempo();
        private int segundosAuxiliares = 1;

        //Sobre el derrape y las ruedas
        bool giroConDerrape = false;

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
            TgcTexture texturaLadrillo = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\ladrillo\\ladrillo.jpg");
            TgcTexture texturaMetal = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\metal.jpg");

            //Creamos una caja 3D de color rojo, ubicada en el origen y lado 10
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(16000, 3, 7660);
            box = TgcBox.fromSize(center, size, texture);


            //En este ejemplo primero cargamos una escena 3D entera.
            TgcSceneLoader loader = new TgcSceneLoader();

            //Luego cargamos otro modelo aparte que va a hacer el objeto que controlamos con el teclado
            TgcScene scene1 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto-TgcScene.xml");
            TgcScene scene2 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene3 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene4 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");
            TgcScene scene5 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");

            //Creo un obstaculo de prueba de colsiones y demás
            obstaculoDePrueba = TgcBox.fromSize(new Vector3(0f, 0f, -500f), new Vector3(200, 200, 200), texturaMadera);
            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBObstaculoPrueba = TgcObb.computeFromAABB(obstaculoDePrueba.BoundingBox);

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
           
            autoMesh = scene1.Meshes[0];
            ruedaDerechaDelanteraMesh = scene2.Meshes[0];
            ruedaDerechaTraseraMesh = scene3.Meshes[0];
            ruedaIzquierdaDelanteraMesh = scene4.Meshes[0];
            ruedaIzquierdaTraseraMesh = scene5.Meshes[0];

            //creo la lista de ruedas
            ruedas = new List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };


            //posicion del auto
            autoMesh.Position = new Vector3(-1406f, 0f, -2523f);
            //posiciones relativas al auto
            dx = new List<float> { 38, -38, -38, 38 };
            dy = new List<float> { -63, 63, -63, 63 };

            //Vamos a utilizar la cámara en 3ra persona para que siga al objeto principal a medida que se mueve
            GuiController.Instance.ThirdPersonCamera.Enable = true;
    
            GuiController.Instance.ThirdPersonCamera.RotationY = 300;
            GuiController.Instance.ThirdPersonCamera.setCamera(autoMesh.Position, 200, 500);                      
            GuiController.Instance.BackgroundColor = Color.Black;

            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBAuto = TgcObb.computeFromAABB(autoMesh.BoundingBox);
            

            //creo al auto y al jugador
            auto = new Auto(300,ruedas);
            jugador = new Jugador(auto);

            //Creo un punto de control para probarlo
            /*for (int i = 0; i < 10;i++)
            {
                TgcCylinder unCilindro = new TgcCylinder(new Vector3(-300 - (i * 1000), 20, -1000 - (i * 300)), 100, 50);
                trayecto.Add(unCilindro);
                //puntosDelTrayecto.Add(new PuntoDeControl(false));
            }*/
            //Activo el primer punto de control
            //puntosDelTrayecto[0].activarPunto();

            //inicializo puntos de control en prov mov auto.
           posicionesPuntosDeControl = new List<Vector3> { new Vector3 (-1088, 20, -2503), 
                new Vector3 (2377, 20, -2528), new Vector3 (5721, 20, -2547), new Vector3 (7367, 20, -1606),
                new Vector3 (6765, 20, 528), new Vector3 (4586, 20, 458), new Vector3 (3749, 20, 2093),
                new Vector3 (2170, 20, 2743), new Vector3 (2120, 20, 363), new Vector3 (-193, 20, -625),
                new Vector3 (-2067, 20, 981), new Vector3 (-4548, 20, 2366), new Vector3 (-6951, 20, 450),
                new Vector3 (-6210, 20, -2318), new Vector3 (-5490, 20, -248), new Vector3 (-2903, 20, -1212)};
           /*
            posicionesPuntosDeControl = new List<Vector3>();
            posicionesPuntosDeControl.Add(new Vector3(-1088, 20, -2503));
            posicionesPuntosDeControl.Add(new Vector3(2377, 20, -2528));
            posicionesPuntosDeControl.Add(new Vector3(5721, 20, -2547));
            posicionesPuntosDeControl.Add(new Vector3(7367, 20, -1606));
            posicionesPuntosDeControl.Add(new Vector3(2170, 20, 2743));
            posicionesPuntosDeControl.Add(new Vector3(2120, 20, 363));
            posicionesPuntosDeControl.Add(new Vector3(-193, 20, -625));
            posicionesPuntosDeControl.Add(new Vector3 (-2067, 20, 981));
            posicionesPuntosDeControl.Add(new Vector3(-4548, 20, 2366));
            posicionesPuntosDeControl.Add(new Vector3(-6951, 20, 450));
            posicionesPuntosDeControl.Add(new Vector3(-6210, 20, -2318));
            posicionesPuntosDeControl.Add( new Vector3 (-5490, 20, -248));
            posicionesPuntosDeControl.Add(new Vector3(-2903, 20, -1212)); */
           for (int i = 0; i < 16; i++)
            {
                TgcCylinder unCilindro = new TgcCylinder(posicionesPuntosDeControl[i], 100, 50);
                trayecto.Add(unCilindro);
            }
           //Creo un obstaculo de prueba de colsiones y demás
           fronteraDerecha = TgcBox.fromSize(new Vector3(-8000f, 60f, -00f), new Vector3(200, 150, 7500), texturaMetal);
           fronteraIzquierda = TgcBox.fromSize(new Vector3(8100f, 60f, -00f), new Vector3(200, 150, 7500), texturaMetal);
           fronteraAdelante = TgcBox.fromSize(new Vector3(-0f, 60f, -3800f), new Vector3(16000, 150, 200), texturaMetal);
           fronteraAtras = TgcBox.fromSize(new Vector3(-0f, 60f, 3800f), new Vector3(16000, 150, 200), texturaMetal);
         
            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
           oBBfronteraDerecha = TgcObb.computeFromAABB(fronteraDerecha.BoundingBox);
           oBBfronteraIzquierda = TgcObb.computeFromAABB(fronteraIzquierda.BoundingBox);
           oBBfronteraAdelante = TgcObb.computeFromAABB(fronteraAdelante.BoundingBox);
           oBBfronteraAtras = TgcObb.computeFromAABB(fronteraAtras.BoundingBox);


            

            /////////////TEXTOS///////////////////////
              
            textPuntosDeControlAlcanzados = new TgcText2d();
            textPuntosDeControlAlcanzados.Position = new Point(0, 50);
            textPuntosDeControlAlcanzados.Text = "Puntos De Control Alcanzados = ";
            textPuntosDeControlAlcanzados.Color = Color.White;
            
            textPosicionDelAutoActual = new TgcText2d();
            textPosicionDelAutoActual.Text = "Posicion del auto actual = ";
            textPosicionDelAutoActual.Color = Color.White;
            textPosicionDelAutoActual.Position = new Point(100, 450);

            this.horaInicio = DateTime.Now;
            textTiempo = new TgcText2d();
            textTiempo.Position = new Point(50, 20);
            textTiempo.Text = "50";
            textTiempo.Color = Color.White;

            textoVelocidad.inicializarTextoVelocidad(auto.velocidad);
            ///////////////MODIFIERS//////////////////
            GuiController.Instance.Modifiers.addFloat("velocidadMaxima", 1000, 7000, 2200f);
           
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
            autoMesh.Rotation = new Vector3(0f, auto.rotacion, 0f);
            oBBAuto.Center = autoMesh.Position;
            oBBAuto.setRotation(autoMesh.Rotation);

            //Calculo de giro de la rueda
            rotacionVertical -= auto.velocidad * elapsedTime / 60;

            //Calculo el movimiento del mesh dependiendo de la velocidad del auto
            autoMesh.moveOrientedY(-auto.velocidad * elapsedTime);

            //Cosas sobre derrape
            autoMesh.Rotation = new Vector3(0f, auto.rotacion - 0.4f, 0f);
            oBBAuto.setRotation(new Vector3(autoMesh.Rotation.X, autoMesh.Rotation.Y - 0.1f, autoMesh.Rotation.Z));
           
            //funcionMagica
            for (int i = 0; i < 4; i++)
            {

                float ro, alfa_rueda;
                float posicion_x;
                float posicion_y;
                ro = FastMath.Sqrt(dx[i] * dx[i] + dy[i] * dy[i]);
                
                alfa_rueda = FastMath.Asin(dx[i] / ro);
                if (i == 0 || i == 2)
                {
                    alfa_rueda += FastMath.PI;
                }
                posicion_x = FastMath.Sin(alfa_rueda + auto.rotacion - 0.4f) * ro;
                posicion_y = FastMath.Cos(alfa_rueda + auto.rotacion - 0.4f) * ro;

                ruedas[i].Position = (new Vector3(posicion_x, 15.5f, posicion_y) + autoMesh.Position);
                //Si no aprieta para los costados, dejo la rueda derecha (por ahora, esto se puede modificar)
                if (input.keyDown(Key.Left) || input.keyDown(Key.A) || input.keyDown(Key.Right) || input.keyDown(Key.D))
                {

                    ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion + auto.rotarRueda(i) - 0.4f, 0f);
                }
                else
                    ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion - 0.4f, 0f);

                //ruedas[i].move(autoMesh.Position.X - autoMeshPrevX, 0, autoMesh.Position.Z-autoMeshPrevZ);
            }

            autoMeshPrevX = autoMesh.Position.X;
            autoMeshPrevZ = autoMesh.Position.Z;

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


            //La camara no rota exactamente a la par del auto, hay un pequeño retraso
            GuiController.Instance.ThirdPersonCamera.RotationY += 5 * (auto.rotacion - prevCameraRotation) * elapsedTime;
            while (prevCameraRotation > 360)
            {
                prevCameraRotation -= 360;
            }
            prevCameraRotation = GuiController.Instance.ThirdPersonCamera.RotationY;

            //Dibujar objeto principal
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].render();
            }
            autoMesh.render();
            box.render();

            fronteraDerecha.render();
            fronteraIzquierda.render();
            fronteraAdelante.render();
            fronteraAtras.render();
            obstaculoDePrueba.render();
            //Hago visibles los obb
            oBBAuto.render();
            oBBObstaculoPrueba.render();
            oBBfronteraDerecha.render();


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

            //Cosas del tiempo
            tiempo.incrementarTiempo(this, elapsedTime, habilitarDecremento);

            //Actualizo y dibujo el relops
            if ((DateTime.Now.Subtract(this.horaInicio).TotalSeconds) > segundosAuxiliares)
            {
                this.textTiempo.Text = (Convert.ToDouble(textTiempo.Text) - 1).ToString();
                segundosAuxiliares++;
            }

            textTiempo.render();
            contadorDeFrames++;
        }
         
        public override void close()
        {
            box.dispose();
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].dispose();
            }
            autoMesh.dispose();

            obstaculoDePrueba.dispose();
            oBBObstaculoPrueba.dispose();
            oBBAuto.dispose();
            fronteraDerecha.dispose();
            fronteraIzquierda.dispose();
            fronteraAdelante.dispose();
            fronteraAtras.dispose();

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
    

