using System;

namespace SIO.Infrastructure
{
    public readonly struct Subject : IEquatable<Subject>
    {
        internal string Value { get; }

        internal Subject(string value)
        {
            Value = value;
        }

        public static Subject New()
        {
            var value = Base64UrlIdGenerator.New();

            return new Subject(value);
        }

        public static Subject From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Subject(value);
        }
        
        public bool Equals(Subject other) => Value == other.Value;
        public override bool Equals(object obj) => obj is Subject other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(Subject left, Subject right) => left.Equals(right);
        public static bool operator !=(Subject left, Subject right) => !left.Equals(right);
        public static implicit operator string(Subject id) => id.Value;
    }
}
