using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    public class Matemagicas
    {
        public static float modulo(Vector2 vector){
            return FastMath.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y));
        }

        public static void posicionarRuedas(List<float> dx, List<float> dy, float rotacion, List<TgcMesh> ruedas, TgcMesh auto)
        {
            for (int i = 0; i < 4; i++)
            {

                float ro, alfa_rueda;
                float posicion_x;
                float posicion_y;
                ro = FastMath.Sqrt(dx[i] * dx[i] + dy[i] * dy[i]);

                alfa_rueda = FastMath.Asin(dx[i] / ro);
                if (i == 0 || i == 2)
                {
                    alfa_rueda += FastMath.PI;
                }
                posicion_x = FastMath.Sin(alfa_rueda + rotacion) * ro;
                posicion_y = FastMath.Cos(alfa_rueda + rotacion) * ro;

                ruedas[i].Position = (new Vector3(posicion_x, 15.5f, posicion_y) + auto.Position);
                //Si no aprieta para los costados, dejo la rueda derecha (por ahora, esto se puede modificar)
                /*if (input.keyDown(Key.Left) || input.keyDown(Key.A) || input.keyDown(Key.Right) || input.keyDown(Key.D))
                {

                    ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion + auto.rotarRueda(i) + (anguloDerrape * direcGiroDerrape), 0f);
                }
                else
                    ruedas[i].Rotation = new Vector3(rotacionVertical, auto.rotacion + (anguloDerrape * direcGiroDerrape), 0f);*/

                //ruedas[i].move(autoMesh.Position.X - autoMeshPrevX, 0, autoMesh.Position.Z-autoMeshPrevZ);
            }
        }
    }
}
