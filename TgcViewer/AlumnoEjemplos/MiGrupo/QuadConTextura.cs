﻿using System;
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

namespace AlumnoEjemplos.MiGrupo
{
    public class QuadConTextura
    {
      
        private CustomVertex.PositionTextured[] _Vertices = new CustomVertex.PositionTextured[6];
        public QuadConTextura()
        {
            Enabled = true;
            IsAlphaBlendEnabled = true;
            Tile = new Vector2(1f, 1f);
            Efecto = GuiController.Instance.Shaders.VariosShader;
            Technique = TgcShaders.T_POSITION_TEXTURED;
        }

        public Vector3 Normal { get; set; }
        public Effect Efecto { get; set; }
        public String Technique { get; set; }
        public Vector2 Tile { get; set; }
        public Vector2 UVOffset { get; set; }
        public Boolean Enabled { get; set; }
        public Boolean IsAlphaBlendEnabled { get; set; }
        private void setAlphaBlend(Boolean isAlphaEnabled)
        {
            Device device = GuiController.Instance.D3dDevice;
            device.RenderState.AlphaTestEnable = isAlphaEnabled;
            device.RenderState.AlphaBlendEnable = isAlphaEnabled;
        }
        public Vector3 Position {get ;set;}


 
        public Vector2 _Size {get;set;}

        public Matrix RotationMatrix{get;set;}
        Matrix _RotationMatrix = Matrix.Identity;
        private TgcTexture _Texture;
        public TgcTexture Texture
        {
            get { return _Texture; }
            set
            {
                if (_Texture != null)
                    _Texture.dispose();
                _Texture = value;
            }
        }
        public void Dispose()
        {
            Texture = null;
        }


        #region TextureMethods
        public void Render()
        {
            if (!Enabled) return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;
            setAlphaBlend(IsAlphaBlendEnabled);
            texturesManager.shaderSet(Efecto, "texDiffuseMap", _Texture);
            texturesManager.clear(1);
            GuiController.Instance.Shaders.setShaderMatrixIdentity(this.Efecto);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            Efecto.Technique = Technique;
            //Render con shader
            Efecto.Begin(0);
            Efecto.BeginPass(0);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2, _Vertices);
            Efecto.EndPass();
            Efecto.End();
            setAlphaBlend(false);
        }
        public void _UpdateValues()
        {
            //TODO:Calcular los 4 corners de la pared, segun el tipo de orientacion
            Vector3 bLeft = new Vector3(-_Size.X / 2, -_Size.Y / 2, 0);
            Vector3 bRight = new Vector3(_Size.X / 2, -_Size.Y / 2, 0);
            Vector3 tLeft = new Vector3(-_Size.X / 2, _Size.Y / 2, 0);
            Vector3 tRight = new Vector3(_Size.X / 2, _Size.Y / 2, 0);
            //Primer triangulo
            _Vertices[0] = new CustomVertex.PositionTextured(bLeft, UVOffset.X, UVOffset.Y + Tile.Y);
            _Vertices[1] = new CustomVertex.PositionTextured(tLeft, UVOffset.X, UVOffset.Y);
            _Vertices[2] = new CustomVertex.PositionTextured(tRight, UVOffset.X + Tile.X, UVOffset.Y);
            //Segundo triangulo
            _Vertices[3] = new CustomVertex.PositionTextured(bLeft, UVOffset.X, UVOffset.Y + Tile.Y);
            _Vertices[4] = new CustomVertex.PositionTextured(tRight, UVOffset.X + Tile.X, UVOffset.Y);
            _Vertices[5] = new CustomVertex.PositionTextured(bRight, UVOffset.X + Tile.X, UVOffset.Y + Tile.Y);


            Vector3 normal;
            normal = (Position- GuiController.Instance.CurrentCamera.getLookAt());
            normal.Normalize();
            float angle = FastMath.Acos(Vector3.Dot(new Vector3(0,0,1), normal));
            Vector3 axisRotation = Vector3.Cross((new Vector3(0,0,1)), normal);
            axisRotation.Normalize();
            Matrix t = Matrix.RotationAxis(axisRotation, angle) * Matrix.Translation(Position);

            //Transformar todos los puntos
            for (int i = 0; i < _Vertices.Length; i++)
            {
                _Vertices[i].Position = Vector3.TransformCoordinate(_Vertices[i].Position, t);
            }


        }
        #endregion TextureMethods
    }
}