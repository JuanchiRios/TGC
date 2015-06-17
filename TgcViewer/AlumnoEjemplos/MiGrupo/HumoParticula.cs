using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{
    class HumoParticula
    {
        public float tiempoDeVida;
        Vector3 posicion;
        public bool activa;
        Vector3 normal;
        float alpha;
        TgcQuad quad;
        Vector2 tamanio;

        public HumoParticula(float tiempo)
        {
            tiempoDeVida = tiempo;
            quad = new TgcQuad();
            tamanio = new Vector2(30, 30);
        }

        public void setPosicion(Vector3 unaPos){
            posicion=unaPos;
        }
        
    
        internal void disminuirVida(float tiempo)
        {
 	        tiempoDeVida-=tiempo;
        }


        internal void resetear(float nuevoTiempo)
        {
            tiempoDeVida = nuevoTiempo;
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
            quad.Center = posicion;
            quad.Size = tamanio;
            quad.Normal = normal;
            quad.AlphaBlendEnable = true;
            //Poner la textura y el alpha al quad
            quad.updateValues();
            quad.render();
        }
    }
}
