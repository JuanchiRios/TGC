using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.MiGrupo
{
    class variablesEnPantalla
    {
        TgcText2d text1;


        public TgcText2d mostrarVelocidad(float velocidad)
        {
            text1 = new TgcText2d();
            text1.Text = "velocidad: "+ velocidad.ToString() ;
            text1.Color = Color.White;
            text1.Position = new Point(600, 600);
            text1.Size = new Size(500, 300);
            return text1;
        }
    }
}
