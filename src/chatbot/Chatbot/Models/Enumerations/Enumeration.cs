namespace Chatbot.Models.Enumerations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// <see cref="https://github.com/HeadspringLabs/Tarantino/blob/master/src/Tarantino.Core/Commons/Model/Enumerations/Enumeration.cs"/>
    /// </summary>
    [Serializable]
    public class Enumeration : IComparable
    {
        private readonly string _displayName;
        private readonly byte _value;

        protected Enumeration()
        {
        }

        protected Enumeration(byte value, string displayName)
        {
            _value = value;
            _displayName = displayName;
        }

        public string DisplayName => _displayName;

        public byte Value => _value;

        public static byte AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
            => (byte)Math.Abs(firstValue.Value - secondValue.Value);

        public static T FromDisplayName<T>(string displayName) where T : Enumeration, new()
            => Parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);

        public static T FromValue<T>(byte value) where T : Enumeration, new()
            => Parse<T, byte>(value, "value", item => item.Value == value);

        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            foreach (var info in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }

        public static IEnumerable GetAll(Type type)
        {
            foreach (var info in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                object instance = Activator.CreateInstance(type);
                yield return info.GetValue(instance);
            }
        }

        public static bool operator ==(Enumeration left, Enumeration right)
        {
            if (left is null)
            {
                return right is null;
            }

            if (right is null)
            {
                return false;
            }

            return left.Value == right.Value;
        }

        public static bool operator !=(Enumeration left, Enumeration right)
        {
            return !(left == right);
        }

        public static bool operator >(Enumeration left, Enumeration right)
        {
            ValidateInputArguments(left, right);
            return left.Value > right.Value;
        }

        public static bool operator >=(Enumeration left, Enumeration right)
        {
            ValidateInputArguments(left, right);
            return left.Value >= right.Value;
        }

        public static bool operator <(Enumeration left, Enumeration right)
        {
            ValidateInputArguments(left, right);
            return left.Value < right.Value;
        }

        public static bool operator <=(Enumeration left, Enumeration right)
        {
            ValidateInputArguments(left, right);
            return left.Value <= right.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var otherValue = obj as Enumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = _value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public virtual int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return Value.CompareTo(((Enumeration)obj).Value);
        }

        private static void ValidateInputArguments(Enumeration left, Enumeration right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T));
                throw new ArgumentOutOfRangeException(message);
            }

            return matchingItem;
        }
    }
}