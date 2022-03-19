using AltV.Net;
using AltV.Net.EntitySync;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Altv_Roleplay.EntityStreamer
{
    public class Ped : Entity, IEntity
    {
        private static List<Ped> pedList = new List<Ped>();

        public static List<Ped> PedList
        {
            get
            {
                lock (pedList)
                {
                    return pedList;
                }
            }
            set
            {
                pedList = value;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                if (!TryGetData("Rotation", out Dictionary<string, object> data))
                    return default;

                return new Vector3()
                {
                    X = Convert.ToSingle(data["x"]),
                    Y = Convert.ToSingle(data["y"]),
                    Z = Convert.ToSingle(data["z"]),
                };
            }
            set
            {
                if (Rotation != null && Rotation.X == value.X && Rotation.Y == value.Y && Rotation.Z == value.Z && value != new Vector3(0, 0, 0))
                    return;

                Dictionary<string, object> dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("Rotation", dict);
            }
        }

        /// <summary>
        /// Set or get the current ped model.
        /// </summary>
        public string Model
        {
            get
            {
                if (!TryGetData("Model", out string model))
                    return null;
                return model;
            }
            set
            {
                if (Model == value)
                    return;
                SetData("Model", value);
            }
        }

        public Vector3 PositionInitial { get; internal set; }

        public Ped(Vector3 position, int dimension, uint range, ulong entityType) : base(entityType, position, dimension, range)
        {
        }

        public void SetRotation(Vector3 rot)
        {
            Rotation = rot;
        }

        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        public void Delete()
        {
            Ped.PedList.Remove(this);
            AltEntitySync.RemoveEntity(this);
        }

        public void Destroy()
        {
            Ped.PedList.Remove(this);
            AltEntitySync.RemoveEntity(this);
        }
    }

    public static class PedStreamer
    {
        /// <summary>
        /// Create Ped
        /// </summary>
        public static Ped Create(string model, Vector3 position, Vector3 rotation, int dimension = 0, uint streamRange = 100)
        {
            Ped obj = new Ped(position, dimension, streamRange, 6)
            {
                Rotation = rotation,
                Model = model,
            };
            Ped.PedList.Add(obj);
            AltEntitySync.AddEntity(obj);
            return obj;
        }

        public static bool Delete(ulong dynamicObjectId)
        {
            Ped obj = GetPed(dynamicObjectId);

            if (obj == null)
                return false;
            Ped.PedList.Remove(obj);
            AltEntitySync.RemoveEntity(obj);
            return true;
        }

        public static void Delete(Ped obj)
        {
            Ped.PedList.Remove(obj);
            AltEntitySync.RemoveEntity(obj);
        }

        public static Ped GetPed(ulong dynamicObjectId)
        {
            if (!AltEntitySync.TryGetEntity(dynamicObjectId, 6, out IEntity entity))
            {
                Console.WriteLine($"ERROR: Entity with id {dynamicObjectId} could not found.");
                return default;
            }

            if (!(entity is Ped))
                return default;

            return (Ped)entity;
        }

        public static void DestroyAllPeds()
        {
            foreach (Ped obj in GetAllPeds())
                AltEntitySync.RemoveEntity(obj);

            Ped.PedList.Clear();
        }

        public static List<Ped> GetAllPeds()
        {
            List<Ped> objects = new List<Ped>();

            foreach (IEntity entity in Ped.PedList)
            {
                Ped obj = GetPed(entity.Id);
                if (obj != null)
                    objects.Add(obj);
            }
            return objects;
        }

        public static (Ped obj, float distance) GetClosestPed(Vector3 pos)
        {
            if (GetAllPeds().Count == 0)
                return (null, 5000);
            Ped obj = null;
            float distance = 5000;
            foreach (Ped o in GetAllPeds())
            {
                float dist = Vector3.Distance(o.Position, pos);
                if (dist < distance)
                {
                    obj = o;
                    distance = dist;
                }
            }
            return (obj, distance);
        }
    }
}