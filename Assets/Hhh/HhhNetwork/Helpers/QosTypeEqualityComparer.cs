namespace HhhNetwork
{
    using System.Collections.Generic;
    using UnityEngine.Networking;

    public sealed class QosTypeEqualityComparer : IEqualityComparer<QosType>
    {
        public bool Equals(QosType x, QosType y)
        {
            return x == y;
        }

        public int GetHashCode(QosType obj)
        {
            return (int)obj;
        }
    }
}