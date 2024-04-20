namespace DynaMod.Serialization.Attributes
{
    public enum DataTypeDescriptor
    {
        // Scalar Types
        S,          // String
        N,          // Number
        B,          // BinaryBinary
        BOOL,       // Boolean
        NULL,       // Null

        // Set Types
        SS,         // String Set
        NS,         // Number Set
        BS,         // Binary Set

        M,          // Map
        L,          // List
    }

    public abstract class AttributeValue
    {
        public readonly DataTypeDescriptor attributeType;

        protected AttributeValue(DataTypeDescriptor attributeType)
        {
            this.attributeType = attributeType;
        }

        public abstract object AsObject();
    }
}
