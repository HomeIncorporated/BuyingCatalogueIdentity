﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public sealed class OrganisationFunction : IEquatable<OrganisationFunction>
    {
        public static readonly OrganisationFunction Authority = new OrganisationFunction(1, nameof(Authority));
        public static readonly OrganisationFunction Buyer = new OrganisationFunction(2, nameof(Buyer));

        private static readonly IEnumerable<OrganisationFunction> _values = new[] { Authority, Buyer };

        public int Value { get; }

        public string DisplayName { get; }

        private OrganisationFunction(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public static OrganisationFunction FromDisplayName(string displayName)
        {
            if (displayName is null)
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            return _values.SingleOrDefault(x => string.Equals(x.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as OrganisationFunction);
        }

        public bool Equals(OrganisationFunction other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public override int GetHashCode() => HashCode.Combine(Value);
    }
}
