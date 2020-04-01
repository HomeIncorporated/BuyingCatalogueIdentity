﻿using System;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class ErrorMessage
    {
        public string Id { get; }

        public string Field { get; }

        public ErrorMessage(string id, string field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        private bool Equals(ErrorMessage other)
        {
            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ErrorMessage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Field);
        }
    }
}