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
        float velocidadMinima=-1000f;
        float velocidadMaxima=5000f;
                

        public Auto(float rot)
        {
            rotacion = rot;
        }

        private void acelerar(float aumento)
        {
            if((velocidad>=velocidadMinima) && (velocidad<=velocidadMaxima))
                {
                velocidad += (aumento-rozamiento())*elapsedTime;
                }
        }
        
        public float rozamiento()
        {
            return rozamientoCoef * Math.Sign(velocidad);
        }

        public void rotar(int direccion)
        {
            rotacion += (elapsedTime * direccion * (velocidad / 300));
            ajustarRotacion();
        }

        public void ajustarRotacion()
        {
            while (rotacion > 360)
            {
                rotacion -= 360;
            }
        }

        public void avanzar()
        {
            acelerar(aceleracionAvanzar);
        }

        public void retroceder()
        {
            if(velocidad>0) frenar();
            if(velocidad<=0) marchaAtras();
        }

        private void frenar()
        {
            acelerar(-aceleracionFrenar);
        }        

        private void marchaAtras()
        {
            acelerar(-aceleracionMarchaAtras);
        }

        public void noMover()
        {
            acelerar(0);
        }
    }
}
