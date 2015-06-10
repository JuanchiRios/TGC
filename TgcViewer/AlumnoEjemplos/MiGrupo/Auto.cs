using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    public class Auto
    {
        public float velocidad;
        float rozamientoCoef=200f;
        public float rotacion;
        public float elapsedTime;
        float aceleracionAvanzar=600f;
        float aceleracionFrenar=800f;
        float aceleracionMarchaAtras=400f;
        float velocidadMinima=-2000f;
        float velocidadMaxima=3000f;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        float direccion;

        public Boolean nitro = false;
                
        //Interfaz de usuario

        public Auto(float rot, List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> unasRuedas)
        {
            rotacion = rot;
            ruedas = unasRuedas;
        }

        public void avanzar()
        {
            acelerar(aceleracionAvanzar);
        }

        public void retroceder()
        {
            if (velocidad > 0) frenar();
            if (velocidad < 0) marchaAtras();
        }

        public void noMover()
        {
            acelerar(0);
        } 
    
        public void rotar(float unaDireccion)
        {
            direccion = unaDireccion;
            rotacion += (elapsedTime * direccion * (velocidad / 1000)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
            ajustarRotacion();
        }
        //       ruedas = new List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh }
        public float rotarRueda(int i){
                if (i == 0 || i == 2)
                {
                    return 0.5f * direccion;
                }
            return 0;
        }
        public void ponerNitro(){
            nitro = true;
        }
        //Metodos Internos
            //DeVelocidad

        private void acelerar(float aumento)
        {
            if (nitro == false)
            {
                velocidad += (aumento - rozamiento()) * elapsedTime;
                ajustarVelocidad();
            }
            else
            {
                velocidadMaxima=3000f;
                velocidad += aumento * 4  * elapsedTime;
                ajustarVelocidad();
                velocidadMaxima=1800f;
                nitro = false;
                

            }

        }

        public void ajustarVelocidad()
        {
            if (velocidad > velocidadMaxima) velocidad = velocidadMaxima;
            if (velocidad < velocidadMinima) velocidad = velocidadMinima;
        }

        public void establecerVelocidadMáximaEn(float velMaxima)
        {
            velocidadMaxima = velMaxima;
        }

        public float rozamiento()
        {
            return rozamientoCoef * Math.Sign(velocidad);
        }

        private void frenar()
        {
            acelerar(-aceleracionFrenar);
        }

        public void frenoDeMano()
        {
            acelerar(-aceleracionFrenar * 2.5f);
        }

        public void marchaAtras()
        {
            acelerar(-aceleracionMarchaAtras);
        }

        
            //DeRotacion
        private void ajustarRotacion()
        {
            while (rotacion > 360)
            {
                rotacion -= 360;
            }
        }

        //para que el auto se quede quieto cuando perdio el jugador
        public void estatico()
        {
            velocidadMaxima = 0f;
            velocidadMinima = 0f;
            ajustarVelocidad();
        }

        public float getRotacion()
        {
            return this.rotacion;
        }
    }
}
