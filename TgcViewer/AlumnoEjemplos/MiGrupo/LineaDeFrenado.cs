using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;

namespace AlumnoEjemplos.MiGrupo
{
    class LineaDeFrenado
    {
        private List<TgcBoxLine> rectas;
        private Vector3 puntoAnterior;
        private bool existePuntoAnterior;
        private float paso;
        private int longitud;
        private float grosor;
        private bool enabled;
        private System.Drawing.Color color;

        private float tiempo;
        private float tiempoMaximoVisualizacionDeLineas = 300;

        public LineaDeFrenado(float _paso, int _longitud, float _grosor, float tiempoDeVisualizacion, System.Drawing.Color _color)
        {
            rectas = new List<TgcBoxLine>();
            existePuntoAnterior = false;
            paso = _paso;
            longitud = _longitud;
            grosor = _grosor;
            tiempoMaximoVisualizacionDeLineas = tiempoDeVisualizacion;
            color = _color;
            enabled = true;
        }

        public void setEnabled(bool _enabled)
        {
            enabled = _enabled;
        }

        private float distancia(Vector3 a, Vector3 b)
        {
            return FastMath.Sqrt(FastMath.Pow2(b.X - a.X) + FastMath.Pow2(b.Y - a.Y));
        }

        private bool cumplePaso(Vector3 punto)
        {
            return distancia(puntoAnterior, punto) >= paso;
        }

        public void addTrack(Vector3 punto)
        {
            bool puntoAgregado = false;

            if (existePuntoAnterior && cumplePaso(punto))
            {
                if (rectas.Count == longitud)
                {
                    rectas.RemoveAt(0);
                }

                rectas.Add(TgcBoxLine.fromExtremes(puntoAnterior, punto, color, grosor));

                puntoAgregado = true;
            }

            if (!existePuntoAnterior || puntoAgregado)
            {
                puntoAnterior = punto;

                existePuntoAnterior = true;
            }
        }

        public void render()
        {
            if (enabled)
            {
                foreach (TgcBoxLine recta in rectas)
                {
                    recta.render();
                }
            }
        }

        public void endTrack()
        {
            existePuntoAnterior = false;
        }

        public void pasoDelTiempo(float elapsedTime)
        {
            tiempo += elapsedTime * 10;

            if (tiempo > tiempoMaximoVisualizacionDeLineas)
            {
                tiempo = 0;
                rectas.Clear();
            }
        }
    }
}
