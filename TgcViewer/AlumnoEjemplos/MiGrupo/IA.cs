using System;
using System.Collections.Generic;
using System.Text;
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

namespace AlumnoEjemplos.MiGrupo
{
    class IA
    {
        Auto auto;
        Vector3 rotacion;

        public IA(Auto unAuto, Vector3 rotacionAuto)
        {
            auto = unAuto;
            rotacion = rotacionAuto;
        }

        public void jugar(Vector3 puntoADondeIR, Vector3 posicionActual)
        {
            if (estaALaIzquierda(puntoADondeIR, posicionActual))
            {
                auto.rotar(-1.3f); //Le damos un giro extra, mayor al de la persona ;)
            }
            if (estaALaDerecha(puntoADondeIR, posicionActual))
            {
                auto.rotar(1.3f);
            }
            auto.avanzar(); //me muevo hacia adelante
        }

        public void setRotacion(Vector3 rotacionAuto)
        {
            rotacion = rotacionAuto;
        }
        public Vector3 getRotacion()
        {
            return rotacion;
        }

        private bool estaALaDerecha(Vector3 puntoObjetivo, Vector3 miPosicion)
        {
            float componenteX = FastMath.Sin(rotacion.Y);
            float componenteZ = FastMath.Cos(rotacion.Y);

            Vector3 v3 = new Vector3(puntoObjetivo.X - miPosicion.X, 0, puntoObjetivo.Z - miPosicion.Z);
            Vector3 v4 = new Vector3(componenteX, 0, componenteZ);
            Vector3 vectorPerpendicular = Vector3.Cross(v3, v4);

            return vectorPerpendicular.Y > 0.2f;

        }
        private bool estaALaIzquierda(Vector3 puntoObjetivo, Vector3 miPosicion)
        {
            float componenteX = FastMath.Sin(rotacion.Y);
            float componenteZ = FastMath.Cos(rotacion.Y);

            Vector3 v3 = new Vector3(puntoObjetivo.X - miPosicion.X, 0, puntoObjetivo.Z - miPosicion.Z);
            Vector3 v4 = new Vector3(componenteX, 0, componenteZ);
            Vector3 vectorPerpendicular = Vector3.Cross(v3, v4);

            return vectorPerpendicular.Y < -0.2f;
        }

        public float angulo(Vector3 puntoObjetivo, Vector3 miPosicion)
        {
            float componenteX = FastMath.Sin(rotacion.Y);
            float componenteZ = FastMath.Cos(rotacion.Y);

            Vector3 v3 = new Vector3(puntoObjetivo.X - miPosicion.X, 0, puntoObjetivo.Z - miPosicion.Z);
            Vector3 v4 = new Vector3(componenteX, 0, componenteZ);
            Vector3 vectorPerpendicular = Vector3.Cross(v3, v4);

            return vectorPerpendicular.Y;
        }
    }
}
