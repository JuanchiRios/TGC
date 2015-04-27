using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;

namespace AlumnoEjemplos.MiGrupo
{
    public class Jugador
    {
        TgcD3dInput input = GuiController.Instance.D3dInput;
        Auto auto;

        public Jugador(Auto unAuto)
        {
            auto = unAuto;
        }

        public void jugar()
        {
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                auto.rotar(-1);
            }
            if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                auto.rotar(1);
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                auto.avanzar();
            }
            if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                auto.retroceder();
            }
            if (!input.keyDown(Key.Down) && !input.keyDown(Key.S) && !input.keyDown(Key.Up) && !input.keyDown(Key.W))
            {
                auto.noMover();
            }
        }
    }
}
