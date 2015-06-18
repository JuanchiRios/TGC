﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class HumoParticula
    {
        public float tiempoDeVida;
        Vector3 posicion;
        public bool activa;
        Vector3 normal;
        float alpha;
        TexturedQuad quad;
        Vector2 tamanio;
        public float posX;
        public float posZ;

        public HumoParticula(float tiempo, List<float> posicionesRelativas)
        {
            tiempoDeVida = tiempo;
            quad = new TexturedQuad();
            tamanio = new Vector2(0.3f, 0.3f);
            posX = posicionesRelativas[0];
            posZ = posicionesRelativas[1];
        }

        public void setTextura(TgcTexture textura)
        {
            quad.Texture = textura;
        }

        public void setPosicion(Vector3 unaPos){
            posicion=unaPos;
        }
        
    
        internal void disminuirVida(float tiempo)
        {
 	        tiempoDeVida-=tiempo;
        }


        internal void resetear(float nuevoTiempo, List<float> posRelativas)
        {
            tiempoDeVida = nuevoTiempo;
            tamanio = new Vector2(0.3f, 0.3f);
            posX = posRelativas[0];
            posZ = posRelativas[1];
        }

        internal void setNormal(Vector3 unaNormal)
        {
            normal = unaNormal;
        }


        internal void calcularAlpha()
        {
            alpha = tiempoDeVida*5;
        }



        internal void render()
        {
            quad.Position = posicion;
            quad.Size = tamanio;
            quad.RotationMatrix.RotateY(GuiController.Instance.CurrentCamera.getLookAt().Y);
            //quad.Normal = normal;
            quad.IsAlphaBlendEnabled = true;
            quad._UpdateValues();
            quad.Render();
        }

        internal void calcularTamanio()
        {
            tamanio = (new Vector2(1,1) * (3.0f-tiempoDeVida))*5;
        }
    }
}
