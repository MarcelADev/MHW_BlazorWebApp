using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MHWApp.Client.Utils
{
    public class ValidationResultComparer : IEqualityComparer<ValidationResult>
    {
        public bool Equals(ValidationResult? x, ValidationResult? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.ErrorMessage == y.ErrorMessage &&
                   x.MemberNames.SequenceEqual(y.MemberNames);
        }

        public int GetHashCode([DisallowNull] ValidationResult obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (obj.ErrorMessage?.GetHashCode() ?? 0);
                foreach (var member in obj.MemberNames)
                    hash = hash * 23 + member.GetHashCode();
                return hash;
            }
        }
    }
}
