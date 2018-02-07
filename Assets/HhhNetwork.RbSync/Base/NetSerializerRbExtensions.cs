namespace HhhNetwork.RbSync
{
    using HhhNetwork;
    using HhhPrefabManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class NetSerializerRbExtensions
    {
        public static void Write(this NetSerializer s, PrefabType prefabType)
        {
            var typeMode = PrefabManager.instance.prefabTypeMode;
            switch (typeMode)
            {
            case PrefabTypeMode.Byte:
                {
                    s.Write((byte)prefabType);
                    break;
                }

            case PrefabTypeMode.Short:
                {
                    s.Write((short)prefabType);
                    break;
                }

            case PrefabTypeMode.Int:
                {
                    s.Write((int)prefabType);
                    break;
                }

            default:
                {
                    throw new System.NotImplementedException();
                }
            }
        }

        public static PrefabType ReadPrefabType(this NetDeserializer s)
        {
            var typeMode = PrefabManager.instance.prefabTypeMode;
            switch (typeMode)
            {
            case PrefabTypeMode.Byte:
                {
                    return (PrefabType)s.ReadByte();
                }

            case PrefabTypeMode.Short:
                {
                    return (PrefabType)s.ReadShort();
                }

            case PrefabTypeMode.Int:
                {
                    return (PrefabType)s.ReadInt();
                }

            default:
                {
                    throw new System.NotImplementedException();
                }
            }
        }

        public static int GetByteSize(this PrefabType prefabType)
        {
            var typeMode = PrefabManager.instance.prefabTypeMode;
            switch (typeMode)
            {
            case PrefabTypeMode.Byte:
                {
                    return 1;
                }

            case PrefabTypeMode.Short:
                {
                    return 2;
                }

            case PrefabTypeMode.Int:
                {
                    return 4;
                }

            default:
                {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}
