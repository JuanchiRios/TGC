using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils._2D;
using System.IO;
using System.Linq;

namespace AlumnoEjemplos.MiGrupo
{
    public class Musica
    {
        string currentFile;
        private string archivo;
        private bool empezado;
        TgcMp3Player player = GuiController.Instance.Mp3Player;


        public void inicializar(){
            archivo = (string)GuiController.Instance.AlumnoEjemplosMediaDir + "TheC#\\Musica\\The Beatles - Back in the USSR.mp3";
            currentFile = archivo;
            empezado = false;
        }
        
        // Cargar un nuevo MP3 si hubo una variacion
       public void loadMp3(string filePath)
        {

            //Cargar archivo
            GuiController.Instance.Mp3Player.closeFile();

             currentFile = archivo;

             GuiController.Instance.Mp3Player.FileName = currentFile;

                //currentMusicText.Text = "Playing: " + new FileInfo(currentFile).Name;
            
        }
        //Ver si cambio el MP3
       public void verSiCambioMP3()
       {
           if(empezado==false){
          // string filePath = (string)GuiController.Instance.Modifiers["MP3-File"];
           loadMp3(archivo);
           reproducir();
           empezado = true;
           return;
           }
           TgcMp3Player.States currentState = player.getStatus();
           if (currentState == TgcMp3Player.States.Playing)
           {
               //Pausar el MP3
               player.pause();
           }
           if (currentState == TgcMp3Player.States.Paused)
                {
                    //Resumir la ejecución del MP3
                    player.resume();
                }
                   
       }
       public void reproducir()
       {
           //Reproducir MP3
           TgcMp3Player.States currentState = player.getStatus();
           if (currentState == TgcMp3Player.States.Open)
           {
               //Reproducir MP3
               player.play(true);
           }
          // player.play(true);
       } 

    }
}
