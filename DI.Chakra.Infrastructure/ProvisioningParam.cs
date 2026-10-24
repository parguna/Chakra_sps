namespace DI.Chakra.Infrastructure
{
    public class ProvisioningParam
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            if (Name != null && !string.IsNullOrEmpty(Value))
                return string.Format("{0}-{1}", Name, Value);

            return Value;
        }
    }
}
