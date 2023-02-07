namespace Apeek.Common
{
    public class ArgValidation
    {
        public static bool String(string target)
        {
            return string.IsNullOrWhiteSpace(target);
        }
        public static bool Object(object target)
        {
            return target == null;
        }
    }
}