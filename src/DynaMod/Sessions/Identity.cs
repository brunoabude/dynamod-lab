using DynaMod.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DynaMod.Sessions
{
    public class Identity
    {
        public AttributeValue Pk;

        public Identity(AttributeValue pk)
        {
            Pk = pk;
        }

        public override bool Equals(object obj)
        {
            return obj is Identity identity &&
                   EqualityComparer<AttributeValue>.Default.Equals(Pk, identity.Pk);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pk);
        }

        public static bool operator ==(Identity left, Identity right)
        {
            return EqualityComparer<Identity>.Default.Equals(left, right);
        }

        public static bool operator !=(Identity left, Identity right)
        {
            return !(left == right);
        }
    }
}
