using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.Model
{
    public abstract class ModelBase<TId>
    {
        public abstract TId Id { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not ModelBase<TId> other) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id!);
        }

        public static bool operator ==(ModelBase<TId>? left, ModelBase<TId>? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelBase<TId>? left, ModelBase<TId>? right)
        {
            return !Equals(left, right);
        }
    }
}
