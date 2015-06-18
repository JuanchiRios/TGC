using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.MiGrupo
{
    public class QuadConTextura
    {
        private CustomVertex.PositionTextured[] vertices;

        readonly Vector3 ORIGINAL_DIR = new Vector3(0, 1, 0);

        Vector3 normal;
        /// <summary>
        /// Normal del plano
        /// </summary>
        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        Vector3 origin;
        /// <summary>
        /// Origen de coordenadas de la pared.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        Vector2 size;
        /// <summary>
        /// Tamaño del plano, en ancho y longitud
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        TgcTexture texture;
        /// <summary>
        /// Textura de la pared
        /// </summary>
        public TgcTexture Texture
        {
            get { return texture; }
        }

        protected Effect effect;
        /// <summary>
        /// Shader del mesh
        /// </summary>
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        protected string technique;
        /// <summary>
        /// Technique que se va a utilizar en el effect.
        /// Cada vez que se llama a render() se carga este Technique (pisando lo que el shader ya tenia seteado)
        /// </summary>
        public string Technique
        {
            get { return technique; }
            set { technique = value; }
        }

        float uTile;
        /// <summary>
        /// Cantidad de tile de la textura en coordenada U.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public float UTile
        {
            get { return uTile; }
            set { uTile = value; }
        }

        float vTile;
        /// <summary>
        /// Cantidad de tile de la textura en coordenada V.
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public float VTile
        {
            get { return vTile; }
            set { vTile = value; }
        }

        bool autoAdjustUv;
        /// <summary>
        /// Auto ajustar coordenadas UV en base a la relación de tamaño de la pared y la textura
        /// Llamar a updateValues() para aplicar cambios.
        /// </summary>
        public bool AutoAdjustUv
        {
            get { return autoAdjustUv; }
            set { autoAdjustUv = value; }
        }

        Vector2 uvOffset;
        /// <summary>
        /// Offset UV de textura
        /// </summary>
        public Vector2 UVOffset
        {
            get { return uvOffset; }
            set { uvOffset = value; }
        }

        private bool enabled;
        /// <summary>
        /// Indica si la pared esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private TgcBoundingBox boundingBox;
        /// <summary>
        /// BoundingBox de la pared
        /// </summary>
        public TgcBoundingBox BoundingBox
        {
            get { return boundingBox; }
        }

        public Vector3 Position
        {
            get { return origin; }
            set { origin = value; }
        }

        private bool alphaBlendEnable;
        /// <summary>
        /// Habilita el renderizado con AlphaBlending para los modelos
        /// con textura o colores por vértice de canal Alpha.
        /// Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable
        {
            get { return alphaBlendEnable; }
            set { alphaBlendEnable = value; }
        }

        /// <summary>
        /// Crea una Quad con Textura vacio.
        /// </summary>
        public QuadConTextura()
        {
            this.vertices = new CustomVertex.PositionTextured[6];
            this.autoAdjustUv = false;
            this.enabled = true;
            this.boundingBox = new TgcBoundingBox();
            this.uTile = 1;
            this.vTile = 1;
            this.alphaBlendEnable = false;
            this.uvOffset = new Vector2(0, 0);

            //Shader
            this.effect = GuiController.Instance.Shaders.VariosShader;
            this.technique = TgcShaders.T_POSITION_TEXTURED;

        }


        /// <summary>
        /// Actualizar parámetros de la pared en base a los valores configurados
        /// </summary>
        public void updateValues()
        {


            //Calcular los 4 corners de la pared, segun el tipo de orientacion
            Vector3 bLeft = new Vector3(-size.X / 2, 0, -size.Y / 2);
            Vector3 tLeft = new Vector3(size.X / 2, 0, -size.Y / 2);
            Vector3 bRight = new Vector3(-size.X / 2, 0, size.Y / 2);
            Vector3 tRight = new Vector3(size.X / 2, 0, size.Y / 2);
            //Auto ajustar UV
            float offsetU = this.uvOffset.X;
            float offsetV = this.uvOffset.Y;

            //Primer triangulo
            vertices[0] = new CustomVertex.PositionTextured(bLeft, offsetU + uTile, offsetV + vTile);
            vertices[1] = new CustomVertex.PositionTextured(tLeft, offsetU, offsetV + vTile);
            vertices[2] = new CustomVertex.PositionTextured(tRight, offsetU, offsetV);

            //Segundo triangulo
            vertices[3] = new CustomVertex.PositionTextured(bLeft, offsetU + uTile, offsetV + vTile);
            vertices[4] = new CustomVertex.PositionTextured(tRight, offsetU, offsetV);
            vertices[5] = new CustomVertex.PositionTextured(bRight, offsetU + uTile, offsetV);

            /*Versión con triángulos para el otro sentido
            //Primer triangulo
            vertices[0] = new CustomVertex.PositionTextured(tLeft, 0 * this.uTile, 1 * this.vTile);
            vertices[1] = new CustomVertex.PositionTextured(bLeft, 1 * this.uTile, 1 * this.vTile);
            vertices[2] = new CustomVertex.PositionTextured(bRight, 1 * this.uTile, 0 * this.vTile);

            //Segundo triangulo
            vertices[3] = new CustomVertex.PositionTextured(bRight, 1 * this.uTile, 0 * this.vTile);
            vertices[4] = new CustomVertex.PositionTextured(tRight, 0 * this.uTile, 0 * this.vTile);
            vertices[5] = new CustomVertex.PositionTextured(tLeft, 0 * this.uTile, 1 * this.vTile);
            */

            //BoundingBox
            normal = (origin - GuiController.Instance.CurrentCamera.getLookAt());
            normal.Normalize();
            float angle = FastMath.Acos(Vector3.Dot(ORIGINAL_DIR, normal));
            Vector3 axisRotation = Vector3.Cross(ORIGINAL_DIR, normal);
            axisRotation.Normalize();
            Matrix t = Matrix.RotationAxis(axisRotation, angle) * Matrix.Translation(origin);

            //Transformar todos los puntos
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position = Vector3.TransformCoordinate(vertices[i].Position, t);
            }

        }

        /// <summary>
        /// Configurar textura de la pared
        /// </summary>
        public void setTexture(TgcTexture texture)
        {
            if (this.texture != null)
            {
                this.texture.dispose();
            }
            this.texture = texture;
        }

        /// <summary>
        /// Renderizar la pared
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            activateAlphaBlend();

            texturesManager.shaderSet(effect, "texDiffuseMap", texture);
            texturesManager.clear(1);
            GuiController.Instance.Shaders.setShaderMatrixIdentity(this.effect);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            effect.Technique = this.technique;

            //Render con shader
            effect.Begin(0);
            effect.BeginPass(0);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2, vertices);
            effect.EndPass();
            effect.End();

            resetAlphaBlend();
        }

        /// <summary>
        /// Activar AlphaBlending, si corresponde
        /// </summary>
        protected void activateAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            if (alphaBlendEnable)
            {
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.AlphaBlendEnable = true;
            }
        }

        /// <summary>
        /// Desactivar AlphaBlending
        /// </summary>
        protected void resetAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaBlendEnable = false;
        }



        /// <summary>
        /// Liberar recursos de la pared
        /// </summary>
        public void dispose()
        {
            texture.dispose();
        }

     
    }
}
