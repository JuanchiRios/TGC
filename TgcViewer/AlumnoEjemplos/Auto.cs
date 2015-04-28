using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        float aceleracionMarchaAtras=300f;

        float velocidadMinima = -500f;
        float velocidadMaxima = 2000f;
                
        //Interfaz de usuario

        public Auto(float rot)
        {
            rotacion = rot;
        }
        public void establecerVelocidadMáximaEn(float velMaxima)
        {
            this.velocidadMaxima = velMaxima;
        }

        public void avanzar()
        {
            acelerar(aceleracionAvanzar);
        }

        public void retroceder()
        {
            if (velocidad > 0) frenar();
            if (velocidad <= 0) marchaAtras();
        }

        public void noMover()
        {
            acelerar(0);
        } 

        public void rotar(int direccion)
        {
            //rotacion += (elapsedTime * direccion * (velocidad / 300)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
            rotacion += (elapsedTime * direccion * (velocidad / 1000));
            ajustarRotacion();
        }

        //Metodos Internos
            //DeVelocidad

        private void acelerar(float aumento)
        {
                velocidad += (aumento-rozamiento())*elapsedTime;
                ajustarVelocidad();
        }

        public void ajustarVelocidad()
        {
            if (velocidad > velocidadMaxima) velocidad = velocidadMaxima;
            if (velocidad < velocidadMinima) velocidad = velocidadMinima;
        }

        public float rozamiento()
        {
            return rozamientoCoef * Math.Sign(velocidad);
        }

        private void frenar()
        {
            acelerar(-aceleracionFrenar);
        }

        private void marchaAtras()
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
        
    }
}
