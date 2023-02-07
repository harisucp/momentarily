using System;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Extensions
{
    public static class AuditExtensions
    {
        public static bool WasModified(this IAuditEntity original, IAuditEntity modified)
        {
            return DateTime.Compare(original.ModDate, modified.ModDate) != 0 ||
                        original.ModBy != modified.ModBy;
        } 
    }
}