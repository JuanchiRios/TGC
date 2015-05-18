using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.MiGrupo
{
    class Tiempo
    {
        public void incrementarTiempo(ProbandoMovAuto pantalla, float elapsedTime, bool habilitarDecremento)
        {
            if (habilitarDecremento) pantalla.tiempoTrans -= elapsedTime * 1.1f;
            else pantalla.tiempoTrans += elapsedTime;
            if (pantalla.tiempoTrans < 0) pantalla.tiempoTrans = 0;
            if (pantalla.tiempoTrans > 1.5f) pantalla.tiempoTrans = 1.5f;
        }
    }
}
