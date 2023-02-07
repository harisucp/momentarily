namespace Apeek.Entities.Validators
{
    public class ValidatorString : ValidatorBase
    {
        public override bool IsValid(object value)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            return true;
        }
    }
}