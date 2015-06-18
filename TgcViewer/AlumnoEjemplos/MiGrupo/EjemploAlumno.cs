using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
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
using TgcViewer.Utils;


namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class ProbandoMovAuto : TgcExample
    {
        HUD hud;
        EmisorHumo emisorHumo;
        bool motionBlurFlag;
        MotionBlur motionBlur;
        TgcBox humo;
        TgcBox fuego;
        TgcMesh autoMesh;
        TgcMesh meshAutoIA;
        TgcMesh ruedaDerechaDelanteraMesh;
        TgcMesh ruedaDerechaTraseraMesh;
        TgcMesh ruedaIzquierdaDelanteraMesh;
        TgcMesh ruedaIzquierdaTraseraMesh;
        float autoMeshPrevZ;
        float autoMeshPrevX;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        List<float> dx;
        List<float> dy;
        List<float> dxAColision;
        List<float> dyAColision;
        List<TgcObb> objetosColisionables;

        TgcObb posteDeSemaforoOBB;

        float rotacionVertical;
        float prevCameraRotation = 300;
        Auto auto;
        Jugador jugador;
        IA jugadorIA;
        Auto autoIA;
        TgcObb oBBAuto;
        TgcObb oBBAutoIa;
        variablesEnPantalla textoVelocidad = new variablesEnPantalla();
        List<Vector3> posicionesPuntosDeControl;
        List<Vector3> posicionesPuntosDeControlDeIA;
        Boolean gano;
        float tiempoHumo = 0f;
        TgcTexture texturaHumo;
        TgcTexture texturaFuego;
        int flagInicio = 0;

        TgcScene scenePista;
        int coheficienteCamara;


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
        Musica musica = new Musica();

        //colisiones entre los autos
        Colisiones colision = new Colisiones();
        List<TgcBox> obbsAuto = new List<TgcBox>();
        List<TgcBox> obbsOtroAuto = new List<TgcBox>();

        //Tiempo 
        public float tiempoTrans = 100f; //tiempo transcurrido desde el defasaje de rotacion de camara y rotacion del mesh
        Tiempo tiempo = new Tiempo();
        private int segundosAuxiliares = 1;
        bool primerRenderDelJuegoAndando = true;

        //Sobre el derrape y las ruedas
        float anguloDerrape = 0.1f;
        float anguloMaximoDeDerrape = 0.35f;
        float velocidadDeDerrape = 0.4f;

        //Creo un listado de puntos de control
        List<TgcCylinder> trayecto = new List<TgcCylinder>();
        List<TgcCylinder> trayectoDeIA = new List<TgcCylinder>();

        int contadorDeActivacionesDePuntosDeControl;
        int contadorDeActivacionesDePuntosDeControlDeIA;

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
            hud = new HUD();
            hud.init();
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
            //sprite.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            sprite.Position = new Vector2(0, 0);
            sprite.Scaling = new Vector2((float)screenSize.Width / textureSize.Width, (float)screenSize.Height / textureSize.Height + 0.01f);
            //sprite.Scaling = new Vector2(1.3f,1.5f);


            texturaHumo = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\humo.png");
            texturaFuego = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Particulas\\Textures\\fuego.png");

            Vector3 centerHumo = new Vector3(0, 0, 0);
            Vector3 sizeHumo = new Vector3(7, 3, 10);
            humo = TgcBox.fromSize(centerHumo, sizeHumo, texturaHumo);
            humo.AlphaBlendEnable = true;
            fuego = TgcBox.fromSize(centerHumo, sizeHumo, texturaFuego);
            fuego.AlphaBlendEnable = true;


            // cosas del tiempo
            tiempoTrans = 100f; //tiempo transcurrido desde el defasaje de rotacion de camara y rotacion del mesh
            segundosAuxiliares = 1;
            //En este ejemplo primero cargamos una escena 3D entera.
            TgcSceneLoader loader = new TgcSceneLoader();

            //Luego cargamos otro modelo aparte que va a hacer el objeto que controlamos con el teclado
            TgcScene scene1 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\autoRojo-TgcScene.xml");
            TgcScene scene2 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene3 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene scene4 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");
            TgcScene scene5 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\Auto_Rueda_Izquierda-TgcScene.xml");
            scenePista = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\pistaentera-TgcScene.xml");
            TgcScene sceneAutoIA = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\autoVerde-TgcScene.xml");
            //cargo obbs
            objetosColisionables = new List<TgcObb>();
          /*  foreach (TgcMesh mesh in scenePista.Meshes)
            {
                if (mesh.Name.Contains("borde") || mesh.Name.Contains("col") || mesh.Name.Contains("Patrullero"))
                {
                    Vector3[] vertices = mesh.getVertexPositions();
                    objetosColisionables.Add(TgcObb.computeFromPoints(vertices));
                }
            }
           NO BORREN ESTO!!!!!!!!!!!!!!!!!
             guardarObbs(objetosColisionables);NO ME BORREN*/
            objetosColisionables = cargarObbObjetos();
            posteDeSemaforoOBB = cargarobbParticular(new Vector3(-1193.647f, -649.2448f, 948.8185f), new Vector3(-1147f, 596.6053f, 1051.182f));
            objetosColisionables.Add(posteDeSemaforoOBB);
            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)

            autoMesh = scene1.Meshes[0];
            ruedaDerechaDelanteraMesh = scene2.Meshes[0];
            ruedaDerechaTraseraMesh = scene3.Meshes[0];
            ruedaIzquierdaDelanteraMesh = scene4.Meshes[0];
            ruedaIzquierdaTraseraMesh = scene5.Meshes[0];
            //humo = scene6.Meshes[0];

            //creo el mesh del auto de IA
            meshAutoIA = sceneAutoIA.Meshes[0];
            //creo la lista de ruedas
            ruedas = new List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };

            //creo la lineas de frenado
            for (int i = 0; i < 4; i++)
            {
                lineaDeFrenado[i] = new LineaDeFrenado(12, 25, 3, 250, Color.Black);
            }


            //posicion del auto
            autoMesh.Position = new Vector3(-278f, 0f, 281f);

            //posiciones relativas al auto
            dx = new List<float> { 45, -45, -45, 45 };
            dy = new List<float> { -61, 71, -61, 71 };

            //posiciones relativas al auto para los box de colision entre autos
            dxAColision = new List<float> { 20, -20, -20, 20 };
            dyAColision = new List<float> { -35, 45, -35, 45 };

            //posiciono al autoIA
            meshAutoIA.Position = new Vector3(211f, 0f, 216f);

            //Vamos a utilizar la cámara en 3ra persona para que siga al objeto principal a medida que se mueve
            GuiController.Instance.ThirdPersonCamera.Enable = true;

            GuiController.Instance.ThirdPersonCamera.RotationY = 300;
            GuiController.Instance.ThirdPersonCamera.setCamera(autoMesh.Position, 200, 500);
            GuiController.Instance.BackgroundColor = Color.Black;// Black;


            //Le asigno su oriented bounding box que me permite rotar la caja de colisiones (no así bounding box)
            oBBAuto = TgcObb.computeFromAABB(autoMesh.BoundingBox);
            oBBAutoIa = TgcObb.computeFromAABB(meshAutoIA.BoundingBox);

            //creo al auto y al jugador
            auto = new Auto(110, ruedas);
            jugador = new Jugador(auto);

            //creo al auto del IA y al IA
            autoIA = new Auto(110, ruedas);
            jugadorIA = new IA(autoIA, new Vector3(0, 0, 0));

            //Inicializo el circuito, tanto para la persona como para la IA
            posicionesPuntosDeControl = new List<Vector3> { new Vector3 (360, 20, 2634), 
                new Vector3 (-1040, 20, 5524), new Vector3 (-1749, 20, 11113),
                new Vector3 (-4484, 20, 14460), new Vector3 (-7377, 20, 16890), new Vector3 (-12790, 20, 17407),
                new Vector3 (-15308, 20, 12649), new Vector3 (-15153, 20, 8531), new Vector3 (-16068, 20, 7030) ,
                new Vector3 (-16342, 20, 4149), new Vector3 (-16222, 20, -1083), new Vector3 (-14986, 20, -3020),
                new Vector3 (-15734, 20, -8568), new Vector3 (-15281, 20, -12097), new Vector3 (-12022, 20, -15335),
                new Vector3 (-6142, 20, -14446), new Vector3 (-4411, 20, -11766), new Vector3 (-2016, 20, -8940), 
                new Vector3 (-1287, 20, -4142), new Vector3 (20, 20, -2566)};


            posicionesPuntosDeControlDeIA = new List<Vector3> { new Vector3 (360, 20, 2634), 
                new Vector3 (-1040, 20, 5524), new Vector3 (-1749, 20, 11113),
                new Vector3 (-4484, 20, 14460), new Vector3 (-7377, 20, 16890), new Vector3 (-12790, 20, 17407),
                new Vector3 (-15308, 20, 12649), new Vector3 (-15153, 20, 8531), new Vector3 (-16068, 20, 7030) ,
                new Vector3 (-16342, 20, 4149), new Vector3 (-16222, 20, -1083), new Vector3 (-14986, 20, -3020),
                new Vector3 (-15734, 20, -8568), new Vector3 (-15281, 20, -12097), new Vector3 (-12022, 20, -15335),
                new Vector3 (-6142, 20, -14446), new Vector3 (-4411, 20, -11766), new Vector3 (-2016, 20, -8940), 
                new Vector3 (-1287, 20, -4142), new Vector3 (20, 20, -2566)};

            for (int i = 0; i < posicionesPuntosDeControl.Count; i++)
            {
                TgcCylinder unCilindro = new TgcCylinder(posicionesPuntosDeControl[i], 130, 30);
                trayecto.Add(unCilindro);
                unCilindro = new TgcCylinder(posicionesPuntosDeControlDeIA[i], 130, 30);
                trayectoDeIA.Add(unCilindro);
            }


            //Asigno los obb que me permiten detectar las colisiones entre los autos
            obbsAuto.Add(TgcBox.fromSize(new Vector3(autoMesh.Position.X - 100, 15, autoMesh.Position.Z + 100), new Vector3(65, 65, 65), texturaFuego));
            obbsAuto.Add(TgcBox.fromSize(new Vector3(autoMesh.Position.X - 100, 15, autoMesh.Position.Z - 100), new Vector3(65, 65, 65), texturaHumo));
            obbsAuto.Add(TgcBox.fromSize(new Vector3(autoMesh.Position.X + 100, 15, autoMesh.Position.Z + 100), new Vector3(65, 65, 65), texturaFuego));
            obbsAuto.Add(TgcBox.fromSize(new Vector3(autoMesh.Position.X + 100, 15, autoMesh.Position.Z - 100), new Vector3(65, 65, 65), texturaFuego));

            obbsOtroAuto.Add(TgcBox.fromSize(new Vector3(meshAutoIA.Position.X - 100, 15, meshAutoIA.Position.Z + 100), new Vector3(65, 65, 65), texturaFuego));
            obbsOtroAuto.Add(TgcBox.fromSize(new Vector3(meshAutoIA.Position.X - 100, 15, meshAutoIA.Position.Z - 100), new Vector3(65, 65, 65), texturaHumo));
            obbsOtroAuto.Add(TgcBox.fromSize(new Vector3(meshAutoIA.Position.X + 100, 15, meshAutoIA.Position.Z + 100), new Vector3(65, 65, 65), texturaFuego));
            obbsOtroAuto.Add(TgcBox.fromSize(new Vector3(meshAutoIA.Position.X + 100, 15, meshAutoIA.Position.Z - 100), new Vector3(65, 65, 65), texturaFuego));

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
            textTiempo.Text = "10";
            textTiempo.Color = Color.White;

            textIngreseTecla = new TgcText2d();
            textIngreseTecla.Text = " Utilize las flechas o W,A,S,D para moverse. \n Shift izquierdo para activar el nitro \n M para cambiar la camara \n Ingrese barra espaciadora para comenzar ";
            textIngreseTecla.Position = new Point(150, 290);
            textIngreseTecla.Align = TgcText2d.TextAlign.LEFT;
            textIngreseTecla.changeFont(new System.Drawing.Font("TimesNewRoman", 23, FontStyle.Bold | FontStyle.Bold));
            textIngreseTecla.Color = Color.White;

            textoVelocidad.inicializarTextoVelocidad(auto.velocidad);
            ///////////////MODIFIERS//////////////////
            GuiController.Instance.Modifiers.addFloat("velocidadMaxima", 1000, 7000, 1800f);
            GuiController.Instance.Modifiers.addBoolean("jugarConTiempo", "jugar con tiempo:", true);
            GuiController.Instance.Modifiers.addBoolean("motionBlurFlag", "Motion Blur", false);
            //////////////Reflejo de luz en auto////////////////
            reflejo = new ReflejoLuzEnAuto(autoMesh);


            //contador de puntos de control
            contadorDeActivacionesDePuntosDeControl = 0;
            contadorDeActivacionesDePuntosDeControlDeIA = 0;
            //flag de victoria
            gano = false;

            List<TgcMesh> autoIAList = new List<TgcMesh>();
            autoIAList.Add(meshAutoIA);

            //motionBlur = new MotionBlur(scenePista.Meshes);
            //motionBlur.motionBlurInit(0);
            musica.inicializar();

            motionBlur = new MotionBlur(scenePista.Meshes);
            motionBlur.motionBlurInit(0);

            emisorHumo = new EmisorHumo();
            emisorHumo.crearQuads(20);
        }

        //OBB
        private List<TgcObb> cargarObbObjetos()
        {
            string filePath = GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\\\pistaObbs-TgcScene.xml";
            string mediaPath = filePath.Substring(0, filePath.LastIndexOf('\\') + 1);
            string xmlString = File.ReadAllText(filePath);
            List<TgcObb> objetos = new List<TgcObb>();

            XmlDocument dom = new XmlDocument();
            dom.LoadXml(xmlString);
            XmlElement root = dom.DocumentElement;

            //Parsear obbs
            XmlNodeList obbNodes = root.GetElementsByTagName("obbs")[0].ChildNodes;
            foreach (XmlElement obbNode in obbNodes)
            {
                Vector3 center = TgcParserUtils.float3ArrayToVector3(TgcParserUtils.parseFloat3Array(obbNode.Attributes["center"].InnerText));
                Vector3 extents = TgcParserUtils.float3ArrayToVector3(TgcParserUtils.parseFloat3Array(obbNode.Attributes["extents"].InnerText));
                Vector3 orientation0 = TgcParserUtils.float3ArrayToVector3(TgcParserUtils.parseFloat3Array(obbNode.Attributes["orientation0"].InnerText));
                Vector3 orientation1 = TgcParserUtils.float3ArrayToVector3(TgcParserUtils.parseFloat3Array(obbNode.Attributes["orientation1"].InnerText));
                Vector3 orientation2 = TgcParserUtils.float3ArrayToVector3(TgcParserUtils.parseFloat3Array(obbNode.Attributes["orientation2"].InnerText));

                Vector3[] orientation = new Vector3[] { orientation0, orientation1, orientation2 };
                TgcObb minObb = new TgcObb();

                minObb.Center = center;
                minObb.Extents = extents;
                minObb.Orientation = orientation;

                objetos.Add(minObb);
            }

            return objetos;
        }


        //FIN OBB
        //guardar
        private void guardarObbs( List<TgcObb> obbs)
        {
            try
            {

                //Crear XML
                XmlDocument doc = new XmlDocument();
                XmlNode root = doc.CreateElement("tgcScene");

                //name
                XmlElement nameNode = doc.CreateElement("name");
                nameNode.InnerText = "pistaObbs";
                root.AppendChild(nameNode);

                //obbs
                XmlElement obbsNode = doc.CreateElement("obbs");
                obbsNode.SetAttribute("count", obbs.Count.ToString());
                foreach (TgcObb unObb in obbs)
                {
                    XmlElement obbNode = doc.CreateElement("obb");
                    obbNode.SetAttribute("center", TgcParserUtils.printFloat3Array(TgcParserUtils.vector3ToFloat3Array(unObb.Center)));
                    obbNode.SetAttribute("extents", TgcParserUtils.printFloat3Array(TgcParserUtils.vector3ToFloat3Array(unObb.Extents)));
                    obbNode.SetAttribute("orientation0", TgcParserUtils.printFloat3Array(TgcParserUtils.vector3ToFloat3Array(unObb.Orientation[0])));
                    obbNode.SetAttribute("orientation1", TgcParserUtils.printFloat3Array(TgcParserUtils.vector3ToFloat3Array(unObb.Orientation[1])));
                    obbNode.SetAttribute("orientation2", TgcParserUtils.printFloat3Array(TgcParserUtils.vector3ToFloat3Array(unObb.Orientation[2])));
                    obbsNode.AppendChild(obbNode);
                }
                root.AppendChild(obbsNode);


                //Guardar XML, borrar si ya existe
                doc.AppendChild(root);
                string sceneFileName = "pistaObbs-TgcScene.xml";
                string sceneFilePath = GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Auto\\" + sceneFileName;
                if (File.Exists(sceneFileName))
                {
                    File.Delete(sceneFileName);
                }
                doc.Save(sceneFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear XML de escena: pistaObbs", ex); ;
            }
        }
        //fin guardar
        //carga alguno especial porque su AABB esta mal
        public TgcObb cargarobbParticular(Vector3 min, Vector3 max)
        {
            TgcBoundingBox objetoABB = new TgcBoundingBox(min, max);
            return TgcObb.computeFromAABB(objetoABB);
        }

        public override void render(float elapsedTime)
        {

            motionBlurFlag = (bool)GuiController.Instance.Modifiers["motionBlurFlag"];
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
                musica.verSiCambioMP3();
            }
            else
            {
                //Para contar el tiempo desde que preciona la barra espaciadora y comienza el juego
                if (primerRenderDelJuegoAndando == true)
                {
                    this.horaInicio = DateTime.Now;
                    primerRenderDelJuegoAndando = false;
                }
                //Todo lo referente a lo que debe hacer el IA
                autoIA.elapsedTime = elapsedTime;
                autoIA.establecerVelocidadMáximaEn((float)GuiController.Instance.Modifiers["velocidadMaxima"] * 1.02f);

                if (colision.getTiempoQueChoco() == 0)
                    jugadorIA.jugar(trayectoDeIA[0].Center, meshAutoIA.Position);

                meshAutoIA.Rotation = new Vector3(0f, autoIA.rotacion, 0f);
                jugadorIA.setRotacion(meshAutoIA.Rotation);

                meshAutoIA.moveOrientedY(-autoIA.velocidad * elapsedTime);
                //Fin movimiento de auto IA

                //Le paso el elapsed time al auto porque sus metodos no deben depender de los FPS
                auto.elapsedTime = elapsedTime;

                //Varío la velocidad Máxima del vehículo con el modifier "velocidadMáxima" 
                auto.establecerVelocidadMáximaEn((float)GuiController.Instance.Modifiers["velocidadMaxima"]);

                //El jugador envia mensajes al auto dependiendo de que tecla presiono
                //Se pone un tiempo para que luego de chocar 2 autos, estos no puedan ingresar movimiento (sólo se mueve por inercia)
                if (colision.getTiempoQueChoco() == 0)
                    jugador.jugar();
                else
                {
                    colision.setTiempoQueChoco(colision.getTiempoQueChoco() - (8 * elapsedTime));
                    if (colision.getTiempoQueChoco() < 0)
                        colision.setTiempoQueChoco(0);
                }

                //Transfiero la rotacion del auto abstracto al mesh, y su obb
                autoMesh.Rotation = new Vector3(0f, auto.rotacion, 0f);
                oBBAuto.Center = autoMesh.Position;
                oBBAuto.setRotation(autoMesh.Rotation);
                meshAutoIA.Rotation = new Vector3(0f, autoIA.rotacion, 0f);
                oBBAutoIa.Center = meshAutoIA.Position;
                oBBAutoIa.setRotation(meshAutoIA.Rotation);


                //Calculo de giro de la rueda
                rotacionVertical -= auto.velocidad * elapsedTime / 60;

                //Calculo el movimiento del mesh dependiendo de la velocidad del auto
                autoMesh.moveOrientedY(-auto.velocidad * elapsedTime);
                //Detección de colisiones
                //Hubo colisión con un objeto. Guardar resultado y abortar loop.

                


                //Si hubo alguna colisión, hacer esto:
                if (huboColision(oBBAuto))
                {
                    autoMesh.moveOrientedY(20 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    auto.velocidad = -(auto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                }
                if (huboColision(oBBAutoIa))
                {
                    meshAutoIA.moveOrientedY(20 * autoIA.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    autoIA.velocidad = -(autoIA.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                }

                //Colisión entre los autos
                for (int i = 0; i < 4; i++)
                {
                    float ro, alfa_rueda;
                    float posicion_xA1;
                    float posicion_yA1;
                    float posicion_xA2;
                    float posicion_yA2;

                    ro = FastMath.Sqrt(dx[i] * dxAColision[i] + dyAColision[i] * dyAColision[i]);

                    alfa_rueda = FastMath.Asin(dxAColision[i] / ro);
                    if (i == 0 || i == 2)
                    {
                        alfa_rueda += FastMath.PI;
                    }
                    posicion_xA1 = FastMath.Sin(alfa_rueda + auto.rotacion) * ro;
                    posicion_yA1 = FastMath.Cos(alfa_rueda + auto.rotacion) * ro;

                    posicion_xA2 = FastMath.Sin(alfa_rueda + autoIA.rotacion) * ro;
                    posicion_yA2 = FastMath.Cos(alfa_rueda + autoIA.rotacion) * ro;

                    obbsAuto[i].Position = (new Vector3(posicion_xA1, 15.5f, posicion_yA1) + autoMesh.Position);

                    obbsOtroAuto[i].Position = (new Vector3(posicion_xA2, 15.5f, posicion_yA2) + meshAutoIA.Position);
                }

                colision.colisionEntreAutos(obbsAuto, obbsOtroAuto, jugador, auto, autoIA, autoMesh, meshAutoIA, elapsedTime);
                /*//Aca muestro las cajas de colision entre los autos
                for (int i = 0; i < 4; i++)
                {
                    obbsAuto[i].render();
                    obbsOtroAuto[i].render();
                }*/

                //Fin colisión entre  los autos

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
                //Fin derrape

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
                    fuego.Enabled = false;
                }
                else
                {
                    humo.Enabled = false;
                    fuego.Enabled = false;
                }
                tiempoHumo += elapsedTime;
                humo.UVOffset = new Vector2(0.9f, tiempoHumo);
                humo.updateValues();
                fuego.UVOffset = new Vector2(0.9f, tiempoHumo);
                fuego.updateValues();

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
                
                if (jugador.estaMirandoHaciaAtras())
                {
                    GuiController.Instance.ThirdPersonCamera.setCamera(autoMesh.Position, 200, -500);
                    GuiController.Instance.ThirdPersonCamera.Target = autoMesh.Position;
                    GuiController.Instance.ThirdPersonCamera.RotationY = auto.rotacion;
                }
                else
                {
                    coheficienteCamara = jugador.verSiCambiaCamara();
                    GuiController.Instance.ThirdPersonCamera.setCamera(autoMesh.Position, 100 + (coheficienteCamara), 900 - (coheficienteCamara) * 4);
                    GuiController.Instance.ThirdPersonCamera.Target = autoMesh.Position;
                    GuiController.Instance.ThirdPersonCamera.RotationY = auto.rotacion;
                }

                //La camara no rota exactamente a la par del auto, hay un pequeño retraso
                //GuiController.Instance.ThirdPersonCamera.RotationY += 5 * (auto.rotacion - prevCameraRotation) * elapsedTime;
                //Ajusto la camara a menos de 360 porque voy a necesitar hacer calculos entre angulos
                while (prevCameraRotation > 360)
                {
                    prevCameraRotation -= 360;
                }
                prevCameraRotation = GuiController.Instance.ThirdPersonCamera.RotationY;
                
                ///////Musica/////////
                jugador.verSiModificaMusica(musica);

                //Dibujar objeto principal
                //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)

                if (motionBlurFlag)
                {             
                motionBlur.update(elapsedTime);
                motionBlur.motionBlurRender(elapsedTime, HighResolutionTimer.Instance.FramesPerSecond, auto.velocidad, 0);
                }
                else
                {
                  foreach (TgcMesh mesh in scenePista.Meshes)
                    {
                        mesh.Technique = "DefaultTechnique";
                        mesh.render();
                    }
                }
                //scenePista.renderAll();

                //Hago visibles los obb
                oBBAuto.render();

                foreach (TgcObb obbColisionable in objetosColisionables)
                {
                    obbColisionable.render();
                }

                //Mostrar al auto IA
                meshAutoIA.render();

                //Muestro el punto siguiente
                trayecto[0].render();
                trayectoDeIA[0].render();










                autoMesh.render();

                for (int i = 0; i < 4; i++)
                {
                    ruedas[i].render();
                }

                humo.render();
                fuego.render();



                //Colision con puntos de control, tanto de persona como IA
                for (int i = 0; i < trayecto.Count; i++)
                {
                    //Pregunto si colisiona con un punto de control activado. Lo sé, feo.
                    if ((i == 0) && TgcCollisionUtils.testPointCylinder(oBBAuto.Position, trayecto[i].BoundingCylinder))
                    {
                        TgcCylinder cilindroModificado = new TgcCylinder(trayecto[i].Center, 130, 30);

                        if (contadorDeActivacionesDePuntosDeControl != (posicionesPuntosDeControl.Count * 3))
                        {
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
                            //Para el IA
                            autoIA.estatico();
                        }

                    }

                }
                for (int i = 0; i < trayectoDeIA.Count; i++)
                {
                    //Pregunto si colisiona con un punto de control activado
                    if ((i == 0) && TgcCollisionUtils.testPointCylinder(meshAutoIA.Position, trayectoDeIA[i].BoundingCylinder))
                    {
                        TgcCylinder cilindroModificado = new TgcCylinder(trayectoDeIA[i].Center, 130, 30);

                        if (contadorDeActivacionesDePuntosDeControlDeIA != (posicionesPuntosDeControlDeIA.Count * 3))
                        {
                            trayectoDeIA.RemoveAt(i);
                            trayectoDeIA.Add(cilindroModificado);
                            contadorDeActivacionesDePuntosDeControlDeIA++;
                        }
                        else
                        {
                            gano = true;
                            textGanaste.Text = "Ganó la máquina :P  ";
                            textGanaste.render();
                            //Para el IA
                            autoIA.estatico();
                            auto.estatico();
                        }
                    }
                }


                // textPosicionDelAutoActual.Text = (jugadorIA.angulo(trayecto[0].Center, meshAutoIA.Position)).ToString();
                //textPosicionDelAutoActual.Text = jugadorIA.getRotacion().ToString();

                textPosicionDelAutoActual.Text = autoMesh.Position.ToString();

                //Renderizar los tres textos

                textoVelocidad.mostrarVelocidad(auto.velocidad / 10).render(); //renderiza la velocidad 

                textPuntosDeControlAlcanzados.render();
                textPosicionDelAutoActual.render();

                //Cosas del tiempo
                tiempo.incrementarTiempo(this, elapsedTime, (bool)GuiController.Instance.Modifiers["jugarConTiempo"]);

                //Actualizo y dibujo el relops
                if ((bool)GuiController.Instance.Modifiers["jugarConTiempo"])
                {
                    if ((DateTime.Now.Subtract(this.horaInicio).TotalSeconds) > segundosAuxiliares)
                    {
                        if (Convert.ToDouble(textTiempo.Text) == 0)
                        {
                            textPerdiste.Text = "Perdiste y lograste " + contadorDeActivacionesDePuntosDeControl.ToString() + " puntos de control";
                            textPerdiste.render();
                            auto.estatico();
                            //Para el IA
                            autoIA.estatico();
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
                }
                /*foreach (TgcMesh mesh in scenePista.Meshes)
                {
                    mesh.render();
                }*/
                emisorHumo.update(elapsedTime, GuiController.Instance.CurrentCamera.getLookAt(), auto.rotacion, autoMesh.Position, anguloDerrape, direcGiroDerrape, auto.nitro, auto.velocidad);
                emisorHumo.render(GuiController.Instance.CurrentCamera.getPosition());
                textTiempo.render();
                contadorDeFrames++;
                hud.render(auto.velocidad,1f);
            }//cierra el if de que no esta en pantalla inicio

        }

        public override void close()
        {
            scenePista.disposeAll();
            humo.dispose();
            fuego.dispose();
            for (int i = 0; i < 4; i++)
            {
                ruedas[i].dispose();
            }
            autoMesh.dispose();
            meshAutoIA.dispose();
            oBBAuto.dispose();
            oBBAutoIa.dispose();
            sprite.dispose();

            //borro los puntos de control del trayecto
            for (int i = 0; i < trayecto.Count; i++)
            {
                trayecto[i].dispose();
                trayecto[i].BoundingCylinder.dispose();
                trayectoDeIA[i].dispose();
                trayectoDeIA[i].BoundingCylinder.dispose();
            }
            trayecto.Clear();
            trayectoDeIA.Clear();

            foreach (TgcObb obbColisionable in objetosColisionables)
            {
                obbColisionable.dispose();
            }

            //Liberar textos

            textPuntosDeControlAlcanzados.dispose();
            textPosicionDelAutoActual.dispose();
            textIngreseTecla.dispose();

            //para que una vez cerrado el juego y vuelto a abrir, comience nuevamente en la pantalla de inicio
            flagInicio = 0;
            primerRenderDelJuegoAndando = true;

        }
        //no borren el metodo HuboCollision
        public bool huboColision(TgcObb obbDeUnAuto)
        {
            for (int i = 0; i < objetosColisionables.Count; i++)
            {
                if (Colisiones.testObbObb2(obbDeUnAuto, objetosColisionables[i]))
                    return true;
            }
            return false;
        }


    }
}

