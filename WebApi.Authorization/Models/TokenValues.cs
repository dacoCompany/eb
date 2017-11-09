using System;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace WebApi.Authorization.Models
{
    public class TokenValues
    {
        private const string EmptyValueMessage = "Value cannot be empty.";

        private string issuer = "Issuer";
        private string secret = "This is my secret.";
        private DateTime expiredDate = DateTime.Today.AddDays(1);
        private string audience = "Audience";
        private string claimType = "Claim type";
        private string claimValue = "Claim value";

        public TokenValues()
        {
            Claims = new Collection<Claim>();
        }

        /// <summary>
        /// The audience
        /// </summary>
        public string Audience
        {
            get => audience;
            set
            {
                if (audience == value)
                    return;

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(EmptyValueMessage);

                audience = value;
            }
        }

        /// <summary>
        /// The issuer
        /// </summary>
        public string Issuer
        {
            get => issuer;
            set
            {
                if (issuer == value)
                    return;

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(EmptyValueMessage);

                issuer = value;
            }
        }

        public string Secret
        {
            get => secret;
            set
            {
                if (secret == value)
                    return;

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(EmptyValueMessage);

                secret = value;
            }
        }

        public DateTime IssuedDate => DateTime.Now;

        public DateTime ExpiredDate
        {
            get => expiredDate;
            set
            {
                if (expiredDate == value)
                    return;

                if (value == null)
                    throw new ArgumentException(EmptyValueMessage);

                if (value <= DateTime.Today)
                    throw new ArgumentException("Date must be from the future.");

                expiredDate = value;
            }
        }

        public string ClaimType
        {
            get => claimType;
            set
            {
                if (claimType == value)
                    return;

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(EmptyValueMessage);

                claimType = value;
            }
        }

        public string ClaimValue
        {
            get => claimValue;
            set
            {
                if (claimValue == value)
                    return;

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(EmptyValueMessage);

                claimValue = value;
            }
        }

        public Collection<Claim> Claims { get; private set; }
    }
}