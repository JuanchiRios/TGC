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
        bool giraIzquierda, giraDerecha, frenaDeMano, recienSoltoFrenoDeMano, estaRetrocediendo;
        private bool camaraCamibada;
        private int valorDeCamara = 60;
        private bool mirandoHaciaAtras = false;

        public Jugador(Auto unAuto)
        {
            auto = unAuto;
            giraIzquierda = false;
            giraDerecha = false;
            camaraCamibada = true;
        }

        public void jugar()
        {
            if (input.keyDown(Key.LeftControl) || input.keyDown(Key.LeftShift))
            {
                auto.ponerNitro();
            }
            else
            {
                auto.sacarNitro();
            }
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                auto.rotar(-1); //-1 representa la izquierda
                giraIzquierda = true;
            }
            if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                auto.rotar(1); //1 representa la derecha
                giraDerecha = true;
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                auto.avanzar();
                estaRetrocediendo = false;
            }
            if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                auto.retroceder();
                estaRetrocediendo = true;
            }
            if (!input.keyDown(Key.Down) && !input.keyDown(Key.S) && !input.keyDown(Key.Up) && !input.keyDown(Key.W))
            {
                auto.noMover();
                estaRetrocediendo = false;
            }
            if (!input.keyDown(Key.Right) && !input.keyDown(Key.D))
            {
                giraDerecha = false;
            }
            if (!input.keyDown(Key.Left) && !input.keyDown(Key.A))
            {
                giraIzquierda = false;
            }
                                
            //Para el freno de mano que deja marcas en el suelo
            if (input.keyDown(Key.Space))
            {
                recienSoltoFrenoDeMano = false;
                if (auto.velocidad > 0)
                {
                    auto.frenoDeMano();
                    frenaDeMano = true;
                }
                else
                    frenaDeMano = false;
            }
            else
            {
                frenaDeMano = false;
            }
            if (input.keyUp(Key.Space))
            {
                recienSoltoFrenoDeMano = true;
            }
            
            //Para mirar hacia atrás
            if (input.keyDown(Key.Tab))
                mirandoHaciaAtras = true;
            else
                mirandoHaciaAtras = false;
        }

        public bool estaGirandoDerecha()
        {
            return giraDerecha;
        }
        public bool estaGirandoIzquierda()
        {
            return giraIzquierda;
        }

        public bool estaFrenandoDeMano()
        {
            return frenaDeMano;
        }
        public bool dejoDeFrenarDeMano()
        {
            return recienSoltoFrenoDeMano;
        }

        public int verSiAprietaSpace()
        {
            if (input.keyDown(Key.Space))
                return 1;
            return 0;
        }
        public int verSiCambiaCamara()
        {
            if (input.keyPressed(Key.M))
            {
                if (camaraCamibada)
                {
                    camaraCamibada = (camaraCamibada == false);
                    valorDeCamara = 100;
                    return 100;
                }
                camaraCamibada = (camaraCamibada == false);
                valorDeCamara = 60;
                return 60;
            }
            return valorDeCamara;
        }

        public bool estaMarchaAtras()
        {
            return estaRetrocediendo;
        }

        public Boolean estaMirandoHaciaAtras()
        {
            return mirandoHaciaAtras;
        }



        internal void verSiModificaMusica(Musica musica)
        {
            if (input.keyPressed(Key.S)) 
                musica.verSiCambioMP3();
        }
    }
}
