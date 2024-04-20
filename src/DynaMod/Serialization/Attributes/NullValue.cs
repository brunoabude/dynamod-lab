namespace DynaMod.Serialization.Attributes
{
    public class NullValue : AttributeValue
    {
        public NullValue() : base(DataTypeDescriptor.NULL)
        {
        }

        public override object AsObject()
        {
            return null;
        }
    }
}
