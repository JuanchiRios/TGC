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
        bool activado;

        public PuntoDeControl(bool unActivado) 
        {
            activado = unActivado;
        }

        public void activarPunto()
        {
            this.activado = true;
        }
        public void desactivarPunto()
        {
            this.activado = true;
        }
        public bool estaActivado()
        {
            return activado;
        }
    }
}
