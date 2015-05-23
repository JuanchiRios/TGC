using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Modifiers;


namespace AlumnoEjemplos.MiGrupo
{
    class AutoMejorado
    {
        TgcD3dInput input = GuiController.Instance.D3dInput;

        //MESHES
        TgcMesh auto;
        List<TgcMesh> ruedas;

        //CONSTANTES DEL AUTO
        float fuerzaMotor=2000000f;
        float constFrenado=50000f;
        const float resistenciaAerodinamica=0.4527f;
        const float resistenciaDeRodadura=12.8f;
        float masa=1000f;
        float elapsedTime;
        float radioRueda=0.2f;
        List<float> posRelativasX;
        List<float> posRelativasY;
        
        //VARIABLES DEL AUTO
        float rotacion=0;
        Vector2 posicion=new Vector2(0f,0f);
        Vector2 velocidad=new Vector2(0f,0f);
        Boolean avanzar=false;
        Boolean retroceder=false;

        //CONSTRUCTOR
        public AutoMejorado(TgcMesh unAuto, List<TgcMesh> unasRuedas, List<float> dx, List<float> dy, Vector2 unaPos, float unaRot)
        {
            auto = unAuto;
            ruedas = unasRuedas;
            posRelativasX = dx;
            posRelativasY = dy;
            posicion = unaPos;
            rotacion = unaRot;
            auto.Rotation = new Vector3(0,unaRot,0);
        }

        //VALORES CALCULADOS
        private Vector2 aceleracion() { return new Vector2(fuerzaLongitudinal().X / masa, fuerzaLongitudinal().Y / masa); }

        private void calcVelocidad() {
            velocidad = new Vector2(velocidad.X + aceleracion().X * elapsedTime, velocidad.Y + aceleracion().Y * elapsedTime);
        }

        private void calcPosicion() {
            posicion = new Vector2(posicion.X + velocidad.X * elapsedTime, posicion.Y + velocidad.Y * elapsedTime);
        }

        private Vector2 fuerzaLongitudinal()
        {
            return new Vector2(fuerzaAplicada().X + fuerzaResistenciaRodadura().X + fuerzaResistenciaAerodinamica().X,
                               fuerzaAplicada().Y + fuerzaResistenciaRodadura().Y + fuerzaResistenciaAerodinamica().Y);

        }

        private Vector2 fuerzaResistenciaAerodinamica()
        {
            return new Vector2(-resistenciaAerodinamica * velocidad.X * Matemagicas.modulo(velocidad),
                               -resistenciaAerodinamica * velocidad.Y * Matemagicas.modulo(velocidad));
        }
            
        private Vector2 fuerzaResistenciaRodadura() { return new Vector2(-resistenciaDeRodadura * velocidad.X, -resistenciaDeRodadura * velocidad.Y); }

        private Vector2 fuerzaAplicada()
        {
            if (avanzar) return -fuerzaTraccion();
            if (retroceder) return -fuerzaFrenado();
            return new Vector2(0f,0f);
        }

        private Vector2 fuerzaTraccion() { return new Vector2(fuerzaMotor * FastMath.Sin(rotacion), fuerzaMotor * FastMath.Cos(rotacion)); }

        private Vector2 fuerzaFrenado() { return new Vector2(-constFrenado * FastMath.Sin(rotacion), -constFrenado * FastMath.Cos(rotacion)); }

        private Vector2 ratioDeslizamiento() {
            if (Matemagicas.modulo(velocidad) == 0f) return new Vector2(0f, 0f);
            return new Vector2((velocidadAngular() * radioRueda - velocidad.X) / Matemagicas.modulo(velocidad),
                                    (velocidadAngular() * radioRueda - velocidad.Y) / Matemagicas.modulo(velocidad));
        }

        private float velocidadAngular()
        {
            return Matemagicas.modulo(velocidad) / (2 * FastMath.PI * radioRueda);
        }

        public float getRotacion()
        {
            return rotacion;
        }

        //INPUT
       public void jugar()
        {
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                rotacion -= 3 * elapsedTime;
            }
            if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                rotacion += 3 * elapsedTime;
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                avanzar = true;
                retroceder = false;
            }
            if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                avanzar = false;
                retroceder = true;
               
            }
            if (!input.keyDown(Key.Down) && !input.keyDown(Key.S) && !input.keyDown(Key.Up) && !input.keyDown(Key.W))
            {
                retroceder = false;
                avanzar = false;
            }
            if (!input.keyDown(Key.Right) && !input.keyDown(Key.D))
            {
                ;
            }
            if (!input.keyDown(Key.Left) && !input.keyDown(Key.A))
            {
                ;
            }

        }
        
        //MAIN
       public void mover(float unElapsedTime)
       {
           elapsedTime = unElapsedTime;
           jugar();
           calcVelocidad();
           calcPosicion();
           auto.Position = new Vector3(posicion.X, 0, posicion.Y);
           auto.Rotation = new Vector3(auto.Rotation.X, rotacion, auto.Rotation.Z);
           Matemagicas.posicionarRuedas(posRelativasX, posRelativasY, rotacion, ruedas, auto);
           render();
       }

        //RENDER
       private void render()
       {
           auto.render();
           for (int i = 0; i < 4; i++)
           {
               ruedas[i].render();
           }
       }

    }
}
