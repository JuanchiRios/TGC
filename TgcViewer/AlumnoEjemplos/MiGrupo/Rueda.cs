using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.MeantToLiveGeo.Terrain;

/*
 * Esta clase es la encargada de todo el moviemiento y actualizacion de una rueda.
 * 
 */
namespace AlumnoEjemplos.MiGrupo
{
    class Rueda
    {
        private float ROTATION_SPEED = Geometry.DegreeToRadian(30);

        private float MAX_ANGLE = Geometry.DegreeToRadian(15);
        private float MIN_ANGLE = Geometry.DegreeToRadian(-15);

        public const float RADIO_WHEEL = 0.45f;

        private TgcMesh mesh;

        private float angle;

        private int direction;

        private float gradesRotated = 0f;

        private Vector3 offset;

        private HeightMap heightMap;

        public Rueda(TgcMesh mesh, int direction, Vector3 offset, HeightMap heightMap)
        {
            this.mesh = mesh;
            this.offset = offset;
            this.direction = direction;
            this.heightMap = heightMap;
            if (this.direction == -1)
            {
                float angRotar = Geometry.DegreeToRadian(180);
                this.mesh.rotateY(angRotar);
            }
            this.move(this.offset);
        }

        public void position(Vector3 position)
        {
            this.mesh.Position = position;
        }

        public void move(Vector3 movement)
        {
            this.mesh.move(movement);
        }

        public void moveOrientedY(float distance)
        {
            float realDistance = this.direction * distance;
            this.mesh.moveOrientedY(realDistance);
            this.rotateX(realDistance / RADIO_WHEEL);
        }

        public void rotateWheel(float distance)
        {
            float realDistance = this.direction * distance;
            this.rotateX(realDistance / RADIO_WHEEL);
        }

        public void rotateY(float rotAngle)
        {
            this.move(-this.offset);
            this.mesh.rotateY(rotAngle);
            this.move(this.offset);
        }

        public void rotateX(float rotAngle)
        {
            this.mesh.rotateX(rotAngle);
        }

        public void rotateZ(float rotAngle)
        {
            this.mesh.rotateZ(rotAngle);
        }

        public void turnLeft()
        {
            this.angle = 0;
            this.angle = this.gradesRotated > MIN_ANGLE ? -ROTATION_SPEED : 0;
        }

        public void turnRight()
        {
            this.angle = 0;
            this.angle = this.gradesRotated < MAX_ANGLE ? ROTATION_SPEED : 0;
        }

        public void straighten()
        {
            if (this.isRotated())
            {
                this.angle = this.gradesRotated > 0f ? -ROTATION_SPEED : ROTATION_SPEED;
            }
        }

        public void turn(float elapsedTime)
        {
            float twirl = this.angle * elapsedTime;
            this.rotateY(twirl);
            this.gradesRotated += twirl;
        }

        public void render()
        {
            this.mesh.render();
        }

        public Vector3 getPosition()
        {
            return this.mesh.Position;
        }

        public Vector3 getPositionInVehicle()
        {
            return Vector3.Add(this.mesh.Position, this.offset);
        }

        /*
         * Ubica la rueda en la posicion que le corresponde dentro del vehiculo.
         * 
         */
        public void setPositionInVehicle(TgcMesh vehicle, Vector3 center, float angleY)
        {
            this.mesh.Position = vehicle.Position;
            float x = center.X + (float)Math.Sin((float)vehicle.Rotation.Y) * this.offset.Z;
            float y = 0f;
            float z = center.X + (float)Math.Cos((float)vehicle.Rotation.Y) * this.offset.Z;
            this.mesh.move(x, y, z);
            this.mesh.rotateY(angleY);
            this.mesh.move((float)Math.Cos(-this.mesh.Rotation.Y) * (this.offset.X * this.direction), 0f, (float)Math.Sin(-this.mesh.Rotation.Y) * (this.offset.X * this.direction));
            //this.mesh.Position = new Vector3(this.mesh.Position.X, this.getHeightInYourPosition(), this.mesh.Position.Z);
        }

        public void move(float x, float y, float z)
        {
            this.mesh.move(x, y, z);
        }

        public float getHeightInYourPosition()
        {
            return this.heightMap.get(this.mesh.Position.X / LandscapeRenderer.kGridSpacing, this.mesh.Position.Z / LandscapeRenderer.kGridSpacing) + RADIO_WHEEL;
        }

        public float getGradesRotated()
        {
            return this.gradesRotated;
        }

        public bool isRotated()
        {
            return this.gradesRotated != 0f;
        }

        public void dispose()
        {
            this.mesh.dispose();
        }
    }
}
