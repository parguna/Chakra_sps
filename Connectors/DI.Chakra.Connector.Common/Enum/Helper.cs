using System.ComponentModel;


namespace DI.Chakra.Connector.Common.Enum
{

    public static class Helper
    {
        public static string GetCustomDescription( object objEnum )
        {
            var fi = objEnum.GetType().GetField(objEnum.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute) , false);
            return (attributes.Length > 0) ? attributes[0].Description : objEnum.ToString();
        }

        public static string Description( this System.Enum value )
        {
            return GetCustomDescription(value);
        }


    }
}
