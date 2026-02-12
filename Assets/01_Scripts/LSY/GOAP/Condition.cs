using System;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public interface ICondition
    {
        public static readonly Type[] allowedTypes = new Type[]
        {
            typeof(bool),
            typeof(int), typeof (uint),
            typeof(float), typeof(double), typeof(decimal),
            typeof(string),
            typeof(DateTime),
        };

        public enum ECategory
        {
            Equal,
            NotEqual,
            LessThan,
            GreaterThan,
            LessThanOrEqual,
            GreaterThanOrEqual,
            // range would be represented with the above condition categories
        }

        string Name { get; }
        ECategory Category { get; }
        bool IsConditionMet(Blackboard blackboard);
    }

    [Serializable]
    public struct Condition<T> : ICondition
    {
        [field: SerializeField] public string Name { readonly get; private set; }
        [field: SerializeField] public ICondition.ECategory Category { readonly get; private set; }
        [field: SerializeField] public T Comparand { readonly get; private set; }

        public Condition(string name, ICondition.ECategory category, T comparand)
        {
            if (ICondition.allowedTypes.All(t => t != typeof(T))) throw new ArgumentException($"Comparand type T is not in allowed types.");

            Name = name;
            Category = category;
            Comparand = comparand;

            string typeName = typeof(T).AssemblyQualifiedName;

            switch (category)
            {
                case ICondition.ECategory.Equal:
                case ICondition.ECategory.NotEqual:
                    if (!typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
                        throw new ArgumentException($"Category argument is \"{category}\" for non-equatable type \"{typeName}\".");
                    break;
                case ICondition.ECategory.LessThan:
                case ICondition.ECategory.GreaterThan:
                case ICondition.ECategory.LessThanOrEqual:
                case ICondition.ECategory.GreaterThanOrEqual:
                    if (!typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                        throw new ArgumentException($"Category argument is \"{category}\" for non-comparable type \"{typeName}\".");
                    break;
                default:
                    throw new ArgumentException($"Category argument is unknown.");
            }
        }

        public readonly bool IsConditionMet(Blackboard blackboard)
        {
            if (!blackboard.TryGetValue(Name, out T value)) return false;

            IEquatable<T> equatable = value as IEquatable<T>;
            IComparable<T> comparable = value as IComparable<T>;

            return Category switch
            {
                ICondition.ECategory.Equal => equatable.Equals(value),
                ICondition.ECategory.NotEqual => !equatable.Equals(value),
                ICondition.ECategory.LessThan => 0 > comparable.CompareTo(value),
                ICondition.ECategory.GreaterThan => 0 < comparable.CompareTo(value),
                ICondition.ECategory.LessThanOrEqual => 0 >= comparable.CompareTo(value),
                ICondition.ECategory.GreaterThanOrEqual => 0 <= comparable.CompareTo(value),
                _ => throw new NotImplementedException("Not implemented category.")
            };
        }
    }
}
