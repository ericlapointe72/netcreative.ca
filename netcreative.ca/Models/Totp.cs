using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace netcreative.ca
{
    // Two-factor sign-in for the admin: time-based one-time codes (TOTP, RFC 6238: HMAC-SHA1, 6 digits, 30 seconds), the same
    // codes as Google Authenticator, Microsoft Authenticator, Authy or a password manager produce. Nothing leaves the server:
    // no outside service is involved. The secret of a user is kept encrypted with the site's machine key (App_Data\machineKey.config),
    // the recovery codes only as hashes.
    public static class Totp
    {
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const string RecoveryAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // no 0/O, 1/I
        private const int StepSeconds = 30;
        private const int Digits = 6;
        private const string Issuer = "Net.Créative";
        private const string Purpose = "nc-admin-totp";

        // ---- secret ----------------------------------------------------------------------------------

        // 20 random bytes (160 bits) as Base32 text, what the authenticator app stores
        public static string NewSecret()
        {
            var bytes = new byte[20];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Base32Encode(bytes);
        }

        // "ABCD EFGH ..." in groups of 4, easier to type into an app
        public static string Grouped(string secret)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < secret.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                {
                    sb.Append(' ');
                }

                sb.Append(secret[i]);
            }

            return sb.ToString();
        }

        // the address an authenticator app understands (a QR code carries the same text)
        public static string OtpAuthUri(string userName, string secret)
        {
            return "otpauth://totp/" + Uri.EscapeDataString(Issuer) + ":" + Uri.EscapeDataString(userName)
                + "?secret=" + secret + "&issuer=" + Uri.EscapeDataString(Issuer) + "&algorithm=SHA1&digits=6&period=30";
        }

        // ---- codes -----------------------------------------------------------------------------------

        public static string CodeAt(string secret, long step)
        {
            byte[] key = Base32Decode(secret);

            byte[] counter = BitConverter.GetBytes(step);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            byte[] hash;
            using (var hmac = new HMACSHA1(key))
            {
                hash = hmac.ComputeHash(counter);
            }

            int offset = hash[hash.Length - 1] & 0x0F;
            int binary = ((hash[offset] & 0x7F) << 24) | (hash[offset + 1] << 16) | (hash[offset + 2] << 8) | hash[offset + 3];

            int mod = 1;
            for (int i = 0; i < Digits; i++)
            {
                mod *= 10;
            }

            return (binary % mod).ToString(CultureInfo.InvariantCulture).PadLeft(Digits, '0');
        }

        public static long StepOf(DateTime utc)
        {
            return (long)(utc.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / StepSeconds;
        }

        // The step whose code was typed, or -1. The code of the step before and after the current one is accepted (a phone clock a
        // few seconds off), and a step already used for a sign-in (lastUsedStep) is refused: a code seen over a shoulder cannot be replayed.
        public static long Verify(string secret, string typed, DateTime nowUtc, long lastUsedStep)
        {
            string digits = OnlyDigits(typed);
            if (digits.Length != Digits || string.IsNullOrEmpty(secret))
            {
                return -1;
            }

            long now = StepOf(nowUtc);
            long found = -1;

            // all three are always computed: the time taken does not tell which one matched
            for (long step = now - 1; step <= now + 1; step++)
            {
                if (SameText(CodeAt(secret, step), digits) && step > lastUsedStep)
                {
                    found = step;
                }
            }

            return found;
        }

        // ---- storage ---------------------------------------------------------------------------------

        public static string Protect(string secret)
        {
            return Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(secret), Purpose));
        }

        // null when the stored value cannot be read (for example after the machine key was changed)
        public static string Unprotect(string stored)
        {
            if (string.IsNullOrEmpty(stored))
            {
                return null;
            }

            try
            {
                byte[] bytes = MachineKey.Unprotect(Convert.FromBase64String(stored), Purpose);
                return bytes == null ? null : Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ---- recovery codes --------------------------------------------------------------------------

        // 8 codes like "K7QM-X2PD", to print and keep: each works once, in place of a code from the app
        public static List<string> NewRecoveryCodes()
        {
            var codes = new List<string>();
            var bytes = new byte[8];

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int n = 0; n < 8; n++)
                {
                    rng.GetBytes(bytes);

                    var sb = new StringBuilder();
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 4)
                        {
                            sb.Append('-');
                        }

                        sb.Append(RecoveryAlphabet[bytes[i] % RecoveryAlphabet.Length]);
                    }

                    codes.Add(sb.ToString());
                }
            }

            return codes;
        }

        public static string NormalizeRecovery(string typed)
        {
            var sb = new StringBuilder();
            foreach (char c in (typed ?? string.Empty).ToUpperInvariant())
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        // a code of the app is six digits; anything else with eight letters or digits is taken for a recovery code
        public static bool LooksLikeRecovery(string typed)
        {
            string n = NormalizeRecovery(typed);
            return n.Length == 8 && OnlyDigits(typed).Length != 6;
        }

        public static string HashRecovery(string userName, string code)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes("nc-recovery|" + (userName ?? string.Empty).ToLowerInvariant() + "|" + NormalizeRecovery(code)));
                var sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                }

                return sb.ToString();
            }
        }

        // the hashes (comma separated, as stored) without the one that matches; null when no code matches
        public static string ConsumeRecovery(string userName, string stored, string typed)
        {
            if (string.IsNullOrEmpty(stored) || NormalizeRecovery(typed).Length != 8)
            {
                return null;
            }

            string wanted = HashRecovery(userName, typed);
            var kept = new List<string>();
            bool found = false;

            foreach (string hash in stored.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (SameText(hash, wanted))
                {
                    found = true;
                }
                else
                {
                    kept.Add(hash);
                }
            }

            return found ? string.Join(",", kept) : null;
        }

        // ---- Base32 and small helpers -----------------------------------------------------------------

        public static string Base32Encode(byte[] data)
        {
            var sb = new StringBuilder();
            int buffer = 0;
            int bits = 0;

            foreach (byte b in data)
            {
                buffer = (buffer << 8) | b;
                bits += 8;

                while (bits >= 5)
                {
                    sb.Append(Base32Alphabet[(buffer >> (bits - 5)) & 31]);
                    bits -= 5;
                }
            }

            if (bits > 0)
            {
                sb.Append(Base32Alphabet[(buffer << (5 - bits)) & 31]);
            }

            return sb.ToString();
        }

        public static byte[] Base32Decode(string text)
        {
            var bytes = new List<byte>();
            int buffer = 0;
            int bits = 0;

            foreach (char raw in (text ?? string.Empty).ToUpperInvariant())
            {
                if (raw == ' ' || raw == '-' || raw == '=')
                {
                    continue;
                }

                int value = Base32Alphabet.IndexOf(raw);
                if (value < 0)
                {
                    throw new FormatException("not Base32");
                }

                buffer = (buffer << 5) | value;
                bits += 5;

                if (bits >= 8)
                {
                    bytes.Add((byte)((buffer >> (bits - 8)) & 0xFF));
                    bits -= 8;
                }
            }

            return bytes.ToArray();
        }

        private static string OnlyDigits(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text ?? string.Empty)
            {
                if (c >= '0' && c <= '9')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static bool SameText(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}
