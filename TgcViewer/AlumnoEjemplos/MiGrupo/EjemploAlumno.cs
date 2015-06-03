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
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class ProbandoMovAuto : TgcExample
    {
        //Vector3 posicionRelativaHumo = new Vector3(-19f, 13f, 126f);
        //TgcMesh humo;
        TgcBox humo;
        TgcBox fuego;
        TgcBox boxPista;
        TgcMesh autoMesh;
        TgcBox obstaculoDePrueba, fronteraDerecha, fronteraIzquierda, fronteraAdelante, fronteraAtras;
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
        float prevCameraRotation = 300;
        Auto auto;
        Jugador jugador;
        TgcObb oBBAuto, oBBObstaculoPrueba, oBBfronteraDerecha, oBBfronteraIzquierda, oBBfronteraAdelante, oBBfronteraAtras;
        variablesEnPantalla textoVelocidad = new variablesEnPantalla();
        List<Vector3> posicionesPuntosDeControl;
        Boolean gano;
        float tiempoHumo = 0f;
        TgcTexture texturaHumo;
        TgcTexture texturaFuego;
        int flagInicio = 0;

        TgcD3dInput input = GuiController.Instance.D3dInput;
        //Para la pantalla de inicio
        TgcSprite sprite;

        //Texturas de escenario
        List<TgcBox> escenario = new List<TgcBox>();
        //Obb de los tgcBox del escenario
        List<TgcObb> oBBsEscenario = new List<TgcObb>();

        //texto
        TgcText2d textPuntosDeControlAlcanzados;
        TgcText2d textIngreseTecla;
        TgcText2d textPosicionDelAutoActual;
        TgcText2d textTiempo;
        TgcText2d textPerdiste;
        TgcText2d textGanaste;
        float contadorDeFrames = 0;
        private DateTime horaInicio;

        //Tiempo 
        public float tiempoTrans = 100f; //tiempo transcurrido desde el defasaje de rotacion de camara y rotacion del mesh
        private bool habilitarDecremento = false;
        Tiempo tiempo = new Tiempo();
        private int segundosAuxiliares = 1;

        //Sobre el derrape y las ruedas
        float anguloDerrape = 0.1f;
        float anguloMaximoDeDerrape = 0.35f;
        float velocidadDeDerrape = 0.4f;

        //Creo un listado de puntos de control
        List<TgcCylinder> trayecto = new List<TgcCylinder>();
        //List<PuntoDeControl> puntosDelTrayecto = new List<PuntoDeControl>();
        int contadorDeActivacionesDePuntosDeControl;

        //Lineas de Frenado
        LineaDeFrenado[] lineaDeFrenado = new LineaDeFrenado[4];

        //Reflejo de luz en el auto
        ReflejoLuzEnAuto reflejo;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "TheC#s";
        }

        public override string getDescription()
        {
            return "Juego de carreras. El auto se mueve con las flechas o wasd, el nitro es con shift o control.";
        }
        
        
        public override void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
                //Crear Sprite
                sprite = new TgcSprite();
                sprite.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Inicio\\imagenInicio.jpg");

                //Ubicarlo centrado en la pantalla
                Size screenSize = GuiController.Instance.Panel3d.Size;
                Size textureSize = sprite.Texture.Size;
                //Modifiers para variar parametros del sprite
                /*GuiController.Instance.Modifiers.addVertex2f("position", new Vector2(0, 0), new Vector2(screenSize.Width, screenSize.Height), sprite.Position);
                GuiController.Instance.Modifiers.addVertex2f("scaling", new Vector2(0, 0), new Vector2(4, 4),new Vector2(1.4f,1.6f));// sprite.Scaling);
                GuiController.Instance.Modifiers.addFloat("rotation", 0, 360, 0);*/
                sprite.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
               
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Pista\\pistaCarreras.png");
            TgcTexture texturaMadera = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\Madera\\A3d-Fl3.jpg");
            TgcTexture texturaLadrillo = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\ladrillo\\ladrillo.jpg");
            TgcTexture texturaMetal = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Texturas\\paredlarga.jpg");
            texturaHumo = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\humo.png");
            texturaFuego = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\fuego.png");
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(16000, 3, 7660);
            boxPista = TgcBox.fromSize(center, size, texture);

            Vector3 centerHumo = new Vector3(0, 0, 0);
            Vector3 sizeHumo = new Vector3(7, 3, 10);
            humo = TgcBox.fromSize(centerHumo, sizeHumo, texturaHumo);
            humo.AlphaBlendEnable = true;
            fuego = TgcBox.fromSize(centerHumo, sizeHumo, texturaFuego);
            fuego.AlphaBlendEnable = true;


            // cosas del tiempo
            tiempoTrans = 100f; //tiempo transcurrido desde el defasaje de rotacion de camara y rotacion del mesh
            habilitarDecremento = false;
            segundosAuxiliares = 1;
            //En este ejemplo primero cargamos una escena 3D entera.
            TgcSceneLoader loader = new TgcSceneLoader();

            //Luego cargamos otro modelo aparte que va a hacer el objeto que controlamos con el teclado
            TgcScene scene1 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\autoRojo-TgcScene.xml");
            TgcScene scene2 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene3 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene4 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");
            TgcScene scene5 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");
            //TgcScene scene6 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\humoAuto-TgcScene.xml");
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
            //humo = scene6.Meshes[0];

            //creo la lista de ruedas
            ruedas = new List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };

            //creo la lineas de frenado
            for (int i = 0; i < 4; i++)
            {
                lineaDeFrenado[i] = new LineaDeFrenado(12, 25, 3, 250, Color.Black);
            }

            //posicion del auto
            autoMesh.Position = new Vector3(-1406f, 0f, -2523f);
            //posiciones relativas al auto
            dx = new List<float> { 45, -45, -45, 45 };
            dy = new List<float> { -61, 71, -61, 71 };

            //Vamos a utilizar la cámara en 3ra persona para que siga al objeto principal a medida que se mueve
            GuiController.Instance.ThirdPersonCamera.Enable = true;

            GuiController.Instance.ThirdPersonCamera.RotationY = 300;
            GuiController.Instance.ThirdPersonCamera.setCamera(autoMesh.Position, 200, 500);
            GuiController.Instance.BackgroundColor = Color.LightSkyBlue;// Black;

            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBAuto = TgcObb.computeFromAABB(autoMesh.BoundingBox);


            //creo al auto y al jugador
            auto = new Auto(300, ruedas);
            jugador = new Jugador(auto);

            posicionesPuntosDeControl = new List<Vector3> { new Vector3 (-1088, 20, -2503), 
                new Vector3 (2377, 20, -2528), new Vector3 (5721, 20, -2547), new Vector3 (7367, 20, -1606),
                new Vector3 (6765, 20, 528), new Vector3 (4586, 20, 458), new Vector3 (3749, 20, 2093),
                new Vector3 (2170, 20, 2743), new Vector3 (2120, 20, 363), new Vector3 (-193, 20, -625),
                new Vector3 (-2067, 20, 981), new Vector3 (-4548, 20, 2366), new Vector3 (-6951, 20, 450),
                new Vector3 (-6210, 20, -2318), new Vector3 (-5490, 20, -248), new Vector3 (-2903, 20, -1212)};

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
            //lo cargo a escenario los TgcBox
            escenario.Add(boxPista);
            escenario.Add(fronteraDerecha);
            escenario.Add(fronteraIzquierda);
            escenario.Add(fronteraAdelante);
            escenario.Add(fronteraAtras);

            
            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBfronteraDerecha = TgcObb.computeFromAABB(fronteraDerecha.BoundingBox);
            oBBfronteraIzquierda = TgcObb.computeFromAABB(fronteraIzquierda.BoundingBox);
            oBBfronteraAdelante = TgcObb.computeFromAABB(fronteraAdelante.BoundingBox);
            oBBfronteraAtras = TgcObb.computeFromAABB(fronteraAtras.BoundingBox);

            //Asigno todos los obb de los Box a colisionar con el auto, pertenecientess al escenario
            oBBsEscenario.Add(oBBObstaculoPrueba);
            oBBsEscenario.Add(oBBfronteraDerecha);
            oBBsEscenario.Add(oBBfronteraIzquierda);
            oBBsEscenario.Add(oBBfronteraAdelante);
            oBBsEscenario.Add(oBBfronteraAtras);



            /////////////TEXTOS///////////////////////

            textPuntosDeControlAlcanzados = new TgcText2d();
            textPuntosDeControlAlcanzados.Position = new Point(0, 50);
            textPuntosDeControlAlcanzados.Text = "Puntos De Control Alcanzados = ";
            textPuntosDeControlAlcanzados.Color = Color.White;

            textGanaste = new TgcText2d();
            textGanaste.Position = new Point(0, 200);
            textGanaste.Color = Color.LightGreen;

            textPerdiste = new TgcText2d();
            textPerdiste.Position = new Point(0, 200);
            textPerdiste.Text = "Perdiste y lograste ";
            textPerdiste.Color = Color.Red;
            textPerdiste.Size = new Size(900, 700);

            textPosicionDelAutoActual = new TgcText2d();
            textPosicionDelAutoActual.Text = "Posicion del auto actual = ";
            textPosicionDelAutoActual.Color = Color.White;
            textPosicionDelAutoActual.Position = new Point(100, 450);

            this.horaInicio = DateTime.Now;
            textTiempo = new TgcText2d();
            textTiempo.Position = new Point(50, 20);
            textTiempo.Text = "1000";
            textTiempo.Color = Color.White;

            textIngreseTecla = new TgcText2d();
            textIngreseTecla.Text = "  Ingrese barra espaciadora para comenzar ";
            textIngreseTecla.Position = new Point(150, 310);
            textIngreseTecla.Align = TgcText2d.TextAlign.LEFT;
            textIngreseTecla.changeFont(new System.Drawing.Font("TimesNewRoman", 23, FontStyle.Bold | FontStyle.Italic));
            textIngreseTecla.Color = Color.GreenYellow;

            textoVelocidad.inicializarTextoVelocidad(auto.velocidad);
            ///////////////MODIFIERS//////////////////
            GuiController.Instance.Modifiers.addFloat("velocidadMaxima", 1000, 7000, 1800f);

            //////////////Reflejo de luz en auto////////////////
            reflejo = new ReflejoLuzEnAuto(autoMesh);


            //contador de puntos de control
            contadorDeActivacionesDePuntosDeControl = 0;
            //flag de victoria
            gano = false;
        }




        public override void render(float elapsedTime)
        {
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Pista\\pistaCarreras.png");

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            //pantalla De Inicio
            if (flagInicio == 0)
            {

                //Actualizar valores cargados en modifiers
                /*sprite.Position = (Vector2)GuiController.Instance.Modifiers["position"];
                sprite.Scaling = (Vector2)GuiController.Instance.Modifiers["scaling"];
                sprite.Rotation = FastMath.ToRad((float)GuiController.Instance.Modifiers["rotation"]);
                    */
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();
                sprite.render();
                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();
                flagInicio = jugador.verSiAprietaSpace();
                textIngreseTecla.render();
            }
            else
            {

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
                //Detección de colisiones
                //Hubo colisión con un objeto. Guardar resultado y abortar loop.



                //Si hubo alguna colisión, hacer esto:
                if (huboColision())
                {

                    autoMesh.moveOrientedY(20 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    auto.velocidad = -(auto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                }
                //Cosas sobre derrape
                int direcGiroDerrape = 0;

                if (auto.velocidad > 1500 && (jugador.estaGirandoDerecha() || jugador.estaGirandoIzquierda()))
                {
                    if (jugador.estaGirandoIzquierda())
                        direcGiroDerrape = -1;
                    else if (jugador.estaGirandoDerecha())
                        direcGiroDerrape = 1;

                    autoMesh.Rotation = new Vector3(0f, auto.rotacion + (direcGiroDerrape * anguloDerrape), 0f);
                    oBBAuto.setRotation(new Vector3(autoMesh.Rotation.X, autoMesh.Rotation.Y + (direcGiroDerrape * anguloDerrape / 2), autoMesh.Rotation.Z));


                    if (anguloDerrape <= anguloMaximoDeDerrape)
                        anguloDerrape += velocidadDeDerrape * elapsedTime;
                }
                else
                {
                    direcGiroDerrape = 0;
                    anguloDerrape = 0;
                }

                //Posiciono las ruedas
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
                    posicion_x = FastMath.Sin(alfa_rueda + auto.rotacion + (anguloDerrape * direcGiroDerrape)) * ro;
                    posicion_y = FastMath.Cos(alfa_rueda + auto.rotacion + (anguloDerrape * direcGiroDerrape)) * ro;

                    ruedas[i].Position = (new Vector3(posicion_x, 15.5f, posicion_y) + autoMesh.Position);
                    //Si no aprieta para los costados, dejo la rueda derecha (por ahora, esto se puede modificar)
                    if (input.keyDown(Key.Left) || input.keyDown(Key.A) || input.keyDown(Key.Right) || input.keyDown(Key.D))
                    {

                        ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion + auto.rotarRueda(i) + (anguloDerrape * direcGiroDerrape), 0f);
                    }
                    else
                        ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion + (anguloDerrape * direcGiroDerrape), 0f);


                }
                //comienzo humo
                float rohumo, alfa_humo;
                float posicion_xhumo;
                float posicion_yhumo;
                rohumo = FastMath.Sqrt(-19f * -19f + 126f * 126f);

                alfa_humo = FastMath.Asin(-19f / rohumo);
                posicion_xhumo = FastMath.Sin(alfa_humo + auto.rotacion + (anguloDerrape * direcGiroDerrape)) * rohumo;
                posicion_yhumo = FastMath.Cos(alfa_humo + auto.rotacion + (anguloDerrape * direcGiroDerrape)) * rohumo;

                humo.Position = (new Vector3(posicion_xhumo, 15.5f, posicion_yhumo) + autoMesh.Position);
                //Si no aprieta para los costados, dejo la rueda derecha (por ahora, esto se puede modificar)
                if (input.keyDown(Key.Left) || input.keyDown(Key.A) || input.keyDown(Key.Right) || input.keyDown(Key.D))
                {

                    humo.Rotation = new Vector3(0f, auto.rotacion + (anguloDerrape * direcGiroDerrape), 0f);
                }
                else
                    humo.Rotation = new Vector3(0f, auto.rotacion + (anguloDerrape * direcGiroDerrape), 0f);
                //fin de humo
                fuego.Position = humo.Position;
                fuego.Rotation = humo.Rotation;
                //fin fuego
                if (auto.nitro)
                {
                    humo.Enabled = false;
                    fuego.Enabled = true;
                }
                else
                {
                    humo.Enabled = true;
                    fuego.Enabled = false;
                }
                tiempoHumo += elapsedTime;
                humo.UVOffset = new Vector2(0.9f, tiempoHumo);
                humo.updateValues();

                if (tiempoHumo > 50f)
                {
                    tiempoHumo = 0f;
                }
                autoMeshPrevX = autoMesh.Position.X;
                autoMeshPrevZ = autoMesh.Position.Z;

                //Lineas de Frenado
                if (jugador.estaFrenandoDeMano())
                {
                    lineaDeFrenado[0].addTrack(new Vector3(ruedaDerechaDelanteraMesh.Position.X, 0, ruedaDerechaDelanteraMesh.Position.Z));
                    lineaDeFrenado[1].addTrack(new Vector3(ruedaDerechaTraseraMesh.Position.X, 0, ruedaDerechaTraseraMesh.Position.Z));
                    lineaDeFrenado[2].addTrack(new Vector3(ruedaIzquierdaDelanteraMesh.Position.X, 0, ruedaIzquierdaDelanteraMesh.Position.Z));
                    lineaDeFrenado[3].addTrack(new Vector3(ruedaIzquierdaTraseraMesh.Position.X, 0, ruedaIzquierdaTraseraMesh.Position.Z));
                }
                if (jugador.dejoDeFrenarDeMano())
                {
                    for (int i = 0; i < lineaDeFrenado.Length; i++)
                    {
                        lineaDeFrenado[i].endTrack();
                    }
                }
                for (int i = 0; i < lineaDeFrenado.Length; i++)
                {
                    lineaDeFrenado[i].render();
                    lineaDeFrenado[i].pasoDelTiempo(elapsedTime);
                }

                //Dibujo el reflejo de la luz en el auto
                reflejo.Render();

                //////Camara///////
                GuiController.Instance.ThirdPersonCamera.Target = autoMesh.Position;

                //La camara no rota exactamente a la par del auto, hay un pequeño retraso
                GuiController.Instance.ThirdPersonCamera.RotationY += 5 * (auto.rotacion - prevCameraRotation) * elapsedTime;
                //Ajusto la camara a menos de 360 porque voy a necesitar hacer calculos entre angulos
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
                boxPista.render();
                humo.render();
                fuego.render();

                fronteraDerecha.render();
                fronteraIzquierda.render();
                fronteraAdelante.render();
                fronteraAtras.render();
                obstaculoDePrueba.render();
                //Hago visibles los obb
                oBBAuto.render();
                oBBObstaculoPrueba.render();



                //Muestro el punto siguiente
                trayecto[0].render();


                //Colision con puntos de control
                for (int i = 0; i < trayecto.Count; i++)
                {
                    //Pregunto si colisiona con un punto de control activado
                    if ((i == 0) && TgcCollisionUtils.testPointCylinder(oBBAuto.Position, trayecto[i].BoundingCylinder))
                    {
                        TgcCylinder cilindroModificado = new TgcCylinder(trayecto[i].Center, 200, 30);


                        if (contadorDeActivacionesDePuntosDeControl != 48)
                        {
                            trayecto[1].UseTexture = true;
                            trayecto.RemoveAt(i);
                            trayecto.Add(cilindroModificado);
                            contadorDeActivacionesDePuntosDeControl++;
                            textPuntosDeControlAlcanzados.Text = "Puntos De Control Alcanzados = " + contadorDeActivacionesDePuntosDeControl.ToString();
                            textTiempo.Text = (Convert.ToDouble(textTiempo.Text) + 3).ToString();

                        }
                        else
                        {
                            gano = true;
                            textGanaste.Text = "Ganaste y obtuviste un puntaje de  " + textTiempo.Text + " puntos";
                            textGanaste.render();
                            auto.estatico();

                        }

                    }
                }

                textPosicionDelAutoActual.Text = autoMesh.Position.ToString();

                //Renderizar los tres textos

                textoVelocidad.mostrarVelocidad(auto.velocidad / 10).render(); //renderiza la velocidad 

                textPuntosDeControlAlcanzados.render();
                textPosicionDelAutoActual.render();

                //Cosas del tiempo
                tiempo.incrementarTiempo(this, elapsedTime, habilitarDecremento);

                //Actualizo y dibujo el relops
                if ((DateTime.Now.Subtract(this.horaInicio).TotalSeconds) > segundosAuxiliares)
                {
                    if (Convert.ToDouble(textTiempo.Text) == 0)
                    {
                        textPerdiste.Text = "Perdiste y lograste " + contadorDeActivacionesDePuntosDeControl.ToString() + " puntos de control";
                        textPerdiste.render();
                        auto.estatico();
                    }
                    else if (gano == true)
                    {

                    }
                    else
                    {
                        this.textTiempo.Text = (Convert.ToDouble(textTiempo.Text) - 1).ToString();
                        segundosAuxiliares++;
                    }
                }

                textTiempo.render();
                contadorDeFrames++;
            }//cierra el if de que no esta en pantalla inicio
            
        }

        public override void close()
        {
            humo.dispose();
            fuego.dispose();
            boxPista.dispose();
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
            sprite.dispose();

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
            textIngreseTecla.dispose();

        }

        //        public bool huboColision(){
        //if (Colisiones.testObbObb2(oBBAuto, oBBObstaculoPrueba)
        //        || Colisiones.testObbObb2(oBBAuto, oBBfronteraAdelante)
        //        || Colisiones.testObbObb2(oBBAuto, oBBfronteraAtras)
        //        || Colisiones.testObbObb2(oBBAuto, oBBfronteraIzquierda)
        //        || Colisiones.testObbObb2(oBBAuto, oBBfronteraDerecha))
        //    {
        //        return true;
        //    } return false;
        //        }

        public bool huboColision()
        {
            for(int i=0;i<oBBsEscenario.Count;i++)
            {
                if (Colisiones.testObbObb2(oBBAuto, oBBsEscenario[i]))
                    return true;
            }
            return false;
        }

      
    }
}

