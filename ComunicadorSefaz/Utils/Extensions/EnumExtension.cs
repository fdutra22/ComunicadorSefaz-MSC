namespace ComunicadorSefaz.Utils.Extensions
{
    public static class EnumExtention
    {
        public static T ParseEnumItem<T>(this object pValue, string pTag)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), $"Item{pValue}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Value [{pValue}] not valid for a tag [{pTag}].", ex);
            }
        }
    }
}