using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Validations
{
    public class ValidationErrors
    {
        public const string EmailAlreadyExist = "Entered email address already exist";
        public const string WrongLogin = "Entered email or password does not exist";
    }
}
