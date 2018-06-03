﻿using System;

namespace Rfc2253
{
    /// <summary>
    /// An RFC 2253 Relative Distinguished Name, with an Attribute Type (<see cref="Type"/>) and Attribute Value
    /// (<see cref="Value"/>).
    /// </summary>
    /// <remarks>
    /// Note that methods within this class can throw exceptions, and those exceptions may include the RDN Type and
    /// Value (for troubleshooting purposes), which may cause inadvertent leakage of information if those
    /// exceptions are not caught.
    /// </remarks>
    public class RelativeDistinguishedName : Rfc2253Base, IRelativeDistinguishedName, INormalizable
    {
        public IAttributeComponent Type { get; set; }
        public IAttributeComponent Value { get; set; }

        private static readonly Lazy<IRelativeDistinguishedName> lazy =
            new Lazy<IRelativeDistinguishedName>(() => Create());

        public static IRelativeDistinguishedName Default => lazy.Value;

        private RelativeDistinguishedName() { /* Disable default constructor for public use. */ }


        private static IRelativeDistinguishedName Create()
        {
            return new RelativeDistinguishedName()
            {
                Type = RdnType.Create(rdnType: String.Empty),
                Value = RdnValue.Create(rdnValue: String.Empty),
            };
        }

        public static IRelativeDistinguishedName Create(IAttributeComponent type, IAttributeComponent value)
        {
            return new RelativeDistinguishedName()
            {
                Type = type ?? throw new ArgumentNullException(nameof(type)),
                Value = value ?? throw new ArgumentNullException(nameof(value)),
            };
        }


        /// <summary>
        /// Gets the RDN as a normalized string and optionally normalizes the data structure.
        /// </summary>
        public override string GetAsNormalized(bool convertToNormalized)
        {
            try
            {
                var normalizedRdnString = String.Empty;

                if ((Value as RdnValue).IsMultiValued)
                {
                    normalizedRdnString = Value.GetAsNormalized(convertToNormalized);
                }
                else
                {
                    normalizedRdnString = Type.GetAsNormalized(convertToNormalized) + "=" + 
                        Value.GetAsNormalized(convertToNormalized);
                }

                IsNormalized = true;                                        // because all its Type and value are.
                if (normalizedRdnString == "=")
                    return String.Empty;
                else
                    return normalizedRdnString;
            }
            catch (Exception ex)
            {
                var message = $"An error occurred while normalizing a Relative Distinguished Name," +
                    $" '{Type}={Value}'.";
                throw new Exception(message, ex);
            }
        }


        /// <summary>
        /// Returns the Relative Distinguished Name as a string representation of its current state, which may or
        /// may not be normalized.
        /// </summary>
        public override string ToString() => Type + "=" + Value;
    }
}