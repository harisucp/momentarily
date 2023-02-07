using Apeek.Common.Extensions;
using Apeek.Entities.Interfaces;
using Apeek.Entities.Validators;
namespace Apeek.Common.Converters
{
    public class DescriptionConverter
    {
        //from user edit to db
        public static void RnToBr(IDescription obj)
        {
            if (obj.Description != null)
            {
                obj.Description = StringHelper.FormatPunctuation(obj.Description).RnToBr();
            }
        }
        public static void BrToRn(IDescription obj)
        {
            if (obj.Description != null)
            {
                obj.Description = obj.Description.BrToRn();
            }
        }
    }
}