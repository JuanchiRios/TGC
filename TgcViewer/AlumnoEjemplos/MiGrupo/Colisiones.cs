using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class Colisiones
    {
        float tiempoDeChoque;

        public void colisionEntreAutos(List<TgcBox> obbsAuto, List<TgcBox> obbsOtroAuto, Jugador jugadorAuto, Auto auto, Auto otroAuto, TgcMesh meshAuto, TgcMesh meshOtroAuto, float elapsedTime)
        {
            TgcBoundingBox dDAuto = obbsAuto[0].BoundingBox;
            TgcBoundingBox dIAuto = obbsAuto[2].BoundingBox;
            TgcBoundingBox tDAuto = obbsAuto[1].BoundingBox;
            TgcBoundingBox tIAuto = obbsAuto[3].BoundingBox;

            TgcBoundingBox dDOtroAuto = obbsOtroAuto[0].BoundingBox;
            TgcBoundingBox dIOtroAuto = obbsOtroAuto[2].BoundingBox;
            TgcBoundingBox tDOtroAuto = obbsOtroAuto[1].BoundingBox;
            TgcBoundingBox tIOtroAuto = obbsOtroAuto[3].BoundingBox;

            if (!jugadorAuto.estaMarchaAtras())
            {
                if (TgcCollisionUtils.testAABBAABB(tIAuto, dIOtroAuto) || TgcCollisionUtils.testAABBAABB(tIAuto, dDOtroAuto)
                    || TgcCollisionUtils.testAABBAABB(tDAuto, dIOtroAuto) || TgcCollisionUtils.testAABBAABB(tDAuto, dDOtroAuto))
                {
                    //auto gira cierto angulo hacia izquierda y sube velocidad
                    if (Math.Truncate(auto.velocidad) == 0)
                        auto.velocidad = 300;
                    else
                        auto.velocidad = Math.Abs(auto.velocidad * 0.5f);
                    //otroAuto reduce bastante velocidad y se lo traslada un poco hacia atrás para no seguir chocando
                    meshOtroAuto.moveOrientedY(10 * otroAuto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    otroAuto.velocidad = -(otroAuto.velocidad * 0.3f);
                    if (tiempoDeChoque == 0)
                        tiempoDeChoque = 5;
                }
                else if (TgcCollisionUtils.testAABBAABB(tIOtroAuto, dIAuto) || TgcCollisionUtils.testAABBAABB(tIOtroAuto, dDAuto)
                    || TgcCollisionUtils.testAABBAABB(tDOtroAuto, dIAuto) || TgcCollisionUtils.testAABBAABB(tDOtroAuto, dDAuto))
                {
                    //auto gira cierto angulo hacia derecha y sube velocidad
                    if (Math.Truncate(otroAuto.velocidad) == 0)
                        otroAuto.velocidad = 300;
                    else
                        otroAuto.velocidad = Math.Abs(otroAuto.velocidad * 0.5f);
                    //otroAuto reduce bastante velocidad y se lo traslada un poco hacia atrás para no seguir chocando
                    meshAuto.moveOrientedY(10 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    auto.velocidad = -(auto.velocidad * 0.3f);
                    if (tiempoDeChoque == 0)
                        tiempoDeChoque = 5;
                }
                else if (TgcCollisionUtils.testAABBAABB(dIAuto, dIOtroAuto) || TgcCollisionUtils.testAABBAABB(dDAuto, dIOtroAuto) || TgcCollisionUtils.testAABBAABB(dIAuto, dDOtroAuto) || TgcCollisionUtils.testAABBAABB(dDAuto, dDOtroAuto))
                {
                    //auto se mueve hacia atrás y cambia de sentido 180° (rebota)
                    if (Math.Truncate(auto.velocidad) == 0)
                        auto.velocidad = -300;
                    else
                    {
                        meshAuto.moveOrientedY(10 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                        auto.velocidad = -(auto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                    }
                    //otroAuto hace lo mismo que el auto
                    meshOtroAuto.moveOrientedY(10 * otroAuto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    otroAuto.velocidad = -(otroAuto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                    if (tiempoDeChoque == 0)
                        tiempoDeChoque = 5;
                }
            }
            else
            {
                if (TgcCollisionUtils.testAABBAABB(tIAuto, dIOtroAuto) || TgcCollisionUtils.testAABBAABB(tIAuto, dDOtroAuto) || TgcCollisionUtils.testAABBAABB(tDAuto, dIOtroAuto) || TgcCollisionUtils.testAABBAABB(tDAuto, dDOtroAuto))
                {
                    //ambos autos rebotan (cambia sentido 180°)
                    if (Math.Truncate(auto.velocidad) == 0)
                        auto.velocidad = 300;
                    else
                        auto.velocidad = Math.Abs(auto.velocidad * 0.5f);

                    meshOtroAuto.moveOrientedY(10 * otroAuto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    otroAuto.velocidad = -(otroAuto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                    if (tiempoDeChoque == 0)
                        tiempoDeChoque = 5;
                }
                else if (TgcCollisionUtils.testAABBAABB(tIAuto, tIOtroAuto) || TgcCollisionUtils.testAABBAABB(tIAuto, tDOtroAuto) || TgcCollisionUtils.testAABBAABB(tDAuto, tIOtroAuto) || TgcCollisionUtils.testAABBAABB(tDAuto, tDOtroAuto))
                {
                    //rebota auto
                    meshAuto.moveOrientedY(10 * auto.velocidad * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    auto.velocidad = -(auto.velocidad * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                    if (tiempoDeChoque == 0)
                        tiempoDeChoque = 5;
                }

            }
        }

        public void setTiempoQueChoco(float t)
        {
            tiempoDeChoque = t;
        }
        public float getTiempoQueChoco()
        {
            return tiempoDeChoque;
        }

        /// <summary>
        /// Crea un array de floats con X,Y,Z
        /// </summary>
        private static float[] toArray(Vector3 v)
        {
            return new float[] { v.X, v.Y, v.Z };
        }

        /// <summary>
        /// Testear si hay colision entre dos OBB
        /// </summary>
        /// <param name="a">Primer OBB</param>
        /// <param name="b">Segundo OBB</param>
        /// <returns>True si hay colision</returns>
        public static bool testObbObb2(TgcObb a, TgcObb b)
        {
            return Colisiones.testObbObb2(a.toStruct(), b.toStruct());
        }

        /// <summary>
        /// Testear si hay colision entre dos OBB
        /// </summary>
        /// <param name="a">Primer OBB</param>
        /// <param name="b">Segundo OBB</param>
        /// <returns>True si hay colision</returns>
        public static bool testObbObb2(TgcObb.OBBStruct a, TgcObb.OBBStruct b)
        {
            float ra, rb;
            float[,] R = new float[3, 3];
            float[,] AbsR = new float[3, 3];
            float[] ae = toArray(a.extents);
            float[] be = toArray(b.extents);


            // Compute rotation matrix expressing b in a’s coordinate frame
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    R[i, j] = Vector3.Dot(a.orientation[i], b.orientation[j]);

            // Compute translation vector t
            Vector3 tVec = b.center - a.center;
            // Bring translation into a’s coordinate frame
            float[] t = new float[3];
            t[0] = Vector3.Dot(tVec, a.orientation[0]);
            t[1] = Vector3.Dot(tVec, a.orientation[1]);
            t[2] = Vector3.Dot(tVec, a.orientation[2]);

            // Compute common subexpressions. Add in an epsilon term to
            // counteract arithmetic errors when two edges are parallel and
            // their cross product is (near) null (see text for details)
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    AbsR[i, j] = FastMath.Abs(R[i, j]) + float.Epsilon;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    ae[i] = FastMath.Abs(ae[i]);
                    be[i] = FastMath.Abs(be[i]);
                }
            }

            // Test axes L = A0, L = A1, L = A2
            for (int i = 0; i < 3; i++)
            {
                ra = ae[i];
                rb = be[0] * AbsR[i, 0] + be[1] * AbsR[i, 1] + be[2] * AbsR[i, 2];
                if (FastMath.Abs(t[i]) > FastMath.Abs(ra + rb)) return false;
            }

            // Test axes L = B0, L = B1, L = B2
            for (int i = 0; i < 3; i++)
            {
                ra = ae[0] * AbsR[0, i] + ae[1] * AbsR[1, i] + ae[2] * AbsR[2, i];
                rb = be[i];
                if (FastMath.Abs(t[0] * R[0, i] + t[1] * R[1, i] + t[2] * R[2, i]) > FastMath.Abs(ra + rb)) return false;
            }

            // Test axis L = A0 x B0
            ra = ae[1] * AbsR[2, 0] + ae[2] * AbsR[1, 0];
            rb = be[1] * AbsR[0, 2] + be[2] * AbsR[0, 1];
            if (FastMath.Abs(t[2] * R[1, 0] - t[1] * R[2, 0]) > FastMath.Abs(ra + rb)) return false;

            // Test axis L = A0 x B1
            ra = ae[1] * AbsR[2, 1] + ae[2] * AbsR[1, 1];
            rb = be[0] * AbsR[0, 2] + be[2] * AbsR[0, 0];
            if (FastMath.Abs(t[2] * R[1, 1] - t[1] * R[2, 1]) > FastMath.Abs(ra + rb)) return false;

            // Test axis L = A0 x B2
            ra = ae[1] * AbsR[2, 2] + ae[2] * AbsR[1, 2];
            rb = be[0] * AbsR[0, 1] + be[1] * AbsR[0, 0];
            if (FastMath.Abs(t[2] * R[1, 2] - t[1] * R[2, 2]) > FastMath.Abs(ra + rb)) return false;

            // Test axis L = A1 x B0
            ra = ae[0] * AbsR[2, 0] + ae[2] * AbsR[0, 0];
            rb = be[1] * AbsR[1, 2] + be[2] * AbsR[1, 1];
            if (FastMath.Abs(t[0] * R[2, 0] - t[2] * R[0, 0]) > FastMath.Abs(ra + rb)) return false;


            // Test axis L = A1 x B1
            ra = ae[0] * AbsR[2, 1] + ae[2] * AbsR[0, 1];
            rb = be[0] * AbsR[1, 2] + be[2] * AbsR[1, 0];
            if (FastMath.Abs(t[0] * R[2, 1] - t[2] * R[0, 1]) > FastMath.Abs(ra + rb)) return false;

            // Test axis L = A1 x B2
            ra = ae[0] * AbsR[2, 2] + ae[2] * AbsR[0, 2];
            rb = be[0] * AbsR[1, 1] + be[1] * AbsR[1, 0];
            if (FastMath.Abs(t[0] * R[2, 2] - t[2] * R[0, 2]) > FastMath.Abs(ra + rb)) return false;

            // Test axis L = A2 x B0
            ra = ae[0] * AbsR[1, 0] + ae[1] * AbsR[0, 0];
            rb = be[1] * AbsR[2, 2] + be[2] * AbsR[2, 1];
            if (FastMath.Abs(t[1] * R[0, 0] - t[0] * R[1, 0]) > FastMath.Abs(ra + rb)) return false;

            // Test axis L = A2 x B1
            ra = ae[0] * AbsR[1, 1] + ae[1] * AbsR[0, 1];
            rb = be[0] * AbsR[2, 2] + be[2] * AbsR[2, 0];
            if (FastMath.Abs(t[1] * R[0, 1] - t[0] * R[1, 1]) > FastMath.Abs(ra + rb)) return false;

            // Test axis L = A2 x B2
            ra = ae[0] * AbsR[1, 2] + ae[1] * AbsR[0, 2];
            rb = be[0] * AbsR[2, 1] + be[1] * AbsR[2, 0];
            if (FastMath.Abs(t[1] * R[0, 2] - t[0] * R[1, 2]) > FastMath.Abs(ra + rb)) return false;


            // Since no separating axis is found, the OBBs must be intersecting
            return true;
        }


    }
}

