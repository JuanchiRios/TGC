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


namespace AlumnoEjemplos.MiGrupo
{
    class PuntoDeControl
    {
        Vector3 posicion;
        TgcCylinder cilindro;
        TgcFixedYBoundingCylinder fixedBoundingDelCilindro;
        float radio;
        float espesorAltura;

        public PuntoDeControl(float unRadio, float espesorAltura, Vector3 unaPosicion) 
        {
            radio = unRadio;
            posicion = unaPosicion;
            this.espesorAltura = espesorAltura;

            cilindro = new TgcCylinder(unaPosicion, radio, espesorAltura);
            fixedBoundingDelCilindro = new TgcFixedYBoundingCylinder(unaPosicion, radio, espesorAltura);
        }

        public TgcCylinder tgcCilindro()
        {
            return cilindro;
        }
        public void setCilindro(TgcCylinder cilindroNuevo)
        {
            this.cilindro = cilindroNuevo;
        }
        public TgcFixedYBoundingCylinder fixedBounding()
        {
            return fixedBoundingDelCilindro;
        }
    }
}
