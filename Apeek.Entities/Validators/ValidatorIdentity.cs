namespace Apeek.Entities.Validators
{
    public class ValidatorIdentity : ValidatorBase
    {
        public override bool IsValid(object value)
        {
            var identity = value as int?;
            return identity.HasValue && identity > 0;
        }
    }
}