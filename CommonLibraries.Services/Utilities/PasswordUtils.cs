using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Utilities
{
	public class PasswordUtils
	{
		public static bool ValidatePassword(string pw, out string error)
		{
			error = null;

			if (string.IsNullOrWhiteSpace(pw))
			{ error = "Password cannot be empty."; return false; }
			if (pw.Length < 8)
			{ error = "Must be at least 8 characters."; return false; }

			if (!pw.Any(char.IsLower))
			{ error = "Must contain a lowercase letter."; return false; }
			if (!pw.Any(char.IsUpper))
			{ error = "Must contain an uppercase letter."; return false; }
			if (!pw.Any(char.IsDigit))
			{ error = "Must contain a digit."; return false; }
			if (!pw.Any(c => "#?!@$%^&*-".Contains(c)))
			{ error = "Must contain a special character (#?!@$%^&*-)."; return false; }

			return true;
		}
	}
}
