using System.Text;
using System;

namespace MassiveNumbers
{
    /*
     * By Joseph Sak
     * cyrogemgames.com
     * Updated 11/29/25
     */

    [System.Serializable]
    public struct Number
    {
        /// <summary>
        /// Number of significant figures saved in the struct
        /// </summary>
        public const int SignificantFigures = 12;

        #region Exposed Values

        /// <summary>
        /// Multiplier * 10^Exponent gives you the actual value
        /// </summary>
        public long Multiplier
        {
            get
            {
                return trueValue;
            }
            private set
            {
                if (value == 0)
                {
                    trueValue = 0;
                    exponent = -SignificantFigures;
                    return;
                }
                if (value < 0)
                {
                    value *= -1;
                    if (value == long.MinValue) value = long.MaxValue;
                    negative = true;
                }
                long tttpo = LongExtensions.TenToThePowerOf(SignificantFigures + 1);
                while (value >= tttpo)
                {
                    exponent++;
                    value /= 10;
                }
                tttpo /= 10;
                while (value < tttpo)
                {
                    exponent--;
                    value *= 10;
                }
                trueValue = value;
            }
        }
        private long trueValue;
        /// <summary>
        /// Is the number negative
        /// </summary>
        public bool Negative => negative;
        private bool negative;
        /// <summary>
        /// Multiplier * 10^Exponent gives you the actual value
        /// </summary>
        public long Exponent => exponent;
        private long exponent;

        #endregion

        #region Constructors

        public Number(int Quantity, int Exponent)
        {
            trueValue = 0;
            if (Quantity < 0)
            {
                Quantity *= -1;
                negative = true;
            }
            else
            {
                negative = false;
            }
            this.exponent = Exponent;
            Multiplier = Quantity;
        }

        public Number(long Quantity, int Exponent)
        {
            trueValue = 0;
            if (Quantity < 0)
            {
                Quantity *= -1;
                negative = true;
            }
            else
            {
                negative = false;
            }
            this.exponent = Exponent;
            Multiplier = Quantity;
        }

        public Number(int Quantity, long Exponent)
        {
            trueValue = 0;
            if (Quantity < 0)
            {
                Quantity *= -1;
                negative = true;
            }
            else
            {
                negative = false;
            }
            this.exponent = Exponent;
            Multiplier = Quantity;
        }

        public Number(long Quantity, long Exponent)
        {
            trueValue = 0;
            if (Quantity < 0)
            {
                Quantity *= -1;
                negative = true;
            }
            else
            {
                negative = false;
            }
            this.exponent = Exponent;
            Multiplier = Quantity;
        }

        #endregion

        #region Operator Overrides

        public static Number operator + (Number n1, Number n2)
        {
            // First do we have a single negative? If so send it to subtraction
            if (n1.negative && !n2.negative)
            {
                n1.negative = false;
                return (n2 - n1);
            }
            else if (!n1.negative && n2.negative)
            {
                n2.negative = false;
                return (n1 - n2);
            }
            // For Addition, first check exponents
            if (n1.Exponent == n2.Exponent)
            {
                n1.Multiplier += n2.Multiplier;
                return n1;
            }
            else if (n1.Exponent + SignificantFigures < n2.Exponent)
            {
                // The number being added is too small to matter
                return n2;
            }
            else if (n2.Exponent + SignificantFigures < n1.Exponent)
            {
                // The number coming in is so large our current number doesn't matter
                return n1;
            }
            else
            {   // Things are difficult and we actually have to math
                Number n3, n4;
                if (n1.Exponent > n2.Exponent)
                {
                    n3 = n2;
                    n4 = n1;
                }
                else
                {
                    n3 = n1;
                    n4 = n2;
                } // n4 is now larger exponent than n3
                n4.Multiplier += LongExtensions.ReduceTenToThePowerOf(n3.Multiplier, n4.Exponent - n3.Exponent);
                // If we were adding two negative numbers this whole time make sure the final result is negative as well
                n4.negative = n1.negative && n2.negative;
                return n4;
            }
        }

        public static Number operator - (Number n1, Number n2)
        {
            // First do we have one negative one positive, if so send it to addition
            if (n1.negative ^ n2.negative)
            {
                n2.negative = n1.negative;
                return (n1 + n2);
            }
            if (n1.negative && n2.negative) // If it's -X - -Y that's -X + Y which is Y - X
            {
                n1.negative = false;
                n2.negative = false;
                return (n2 - n1);
            }
            // First check exponents
            if (n1.Exponent == n2.Exponent)
            {
                if (n2.Multiplier > n1.Multiplier)
                {
                    n2.Multiplier -= n1.Multiplier;
                    n2.negative = true;
                    return n2;
                }
                else
                {
                    n1.Multiplier -= n2.Multiplier;
                    return n1;
                }
            }
            else if (n1.Exponent + SignificantFigures < n2.Exponent)
            {
                // The number being subtracted is so large n1 doesn't matter
                n2.negative = true;
                return n2;
            }
            else if (n2.Exponent + SignificantFigures < n1.Exponent)
            {
                // The n1 is so large that the subtraction doesn't matter
                return n1;
            }
            else
            {   // Things are difficult and we actually have to math
                if (n2.Exponent > n1.Exponent)
                {
                    // This should end up negative
                    n2.Multiplier -= LongExtensions.ReduceTenToThePowerOf(n1.Multiplier, n1.Exponent - n2.Exponent);
                    n2.negative = true;
                    return n2;
                }
                else
                {
                    // This should end up positive
                    n1.Multiplier -= LongExtensions.ReduceTenToThePowerOf(n2.Multiplier, n2.Exponent - n1.Exponent);
                    n1.negative = false;
                    return n1;
                }
            }
        }

        public static Number operator + (Number n1)
        {
            return new Number(n1.Multiplier, n1.Exponent);
        }

        public static Number operator - (Number n1)
        {
            return new Number(-n1.Multiplier, n1.Exponent);
        }

        public static Number operator ++ (Number n1)
        {
            return n1 + One;
        }

        public static Number operator -- (Number n1)
        {
            return n1 - One;
        }

        public static Number operator * (Number n1, Number n2)
        {
            // If either value is zero, then the result is zero
            if (n1 == Zero || n2 == Zero) return Zero;
            // Get our lesser value as n3, store the other value as n4
            Number n3 = (n1 < n2) ? n1 : n2;
            Number n4 = (n3 == n1) ? n2 : n1;
            // Convert n3 into a multipliable format
            Tuple<long, long> multiplier = MultiplierPrep(n3);
            // Apply the multipliable format to n4
            n4.Multiplier *= multiplier.Item1;
            n4.exponent += multiplier.Item2;
            // One negative means a negative number, all other combinations are positive
            n4.negative = (n1.negative ^ n2.negative);
            return n4;
        }

        public static Number operator / (Number n1, Number n2)
        {
            // If numerator zero then answer is zero
            if (n1 == Zero || n2 == Zero)
            {   // If  denominator is zero yell at the user to not break math
                return Zero;
            }
            Number n3 = n1;
            // Put the denominator into a multipliable format since division is just inverse multiplication
            Tuple<long, long> multiplier = MultiplierPrep(n2);
            // Apply inversed multipliable format by dividing and subtracting
            n3.Multiplier /= multiplier.Item1;
            n3.exponent -= multiplier.Item2;
            // One negative means a negative number, all other combinations are positive
            n3.negative = (n1.negative ^ n2.negative);
            return n3;
        }

        public static Number operator % (Number n1, Number n2)
        {
            Number n3 = n1 / n2;
            n3 = MathHuge.Floor(n3);
            n3 *= n2;
            return n1 - n3;
        }

        public static bool operator > (Number n1, Number n2)
        {
            if (n1.negative ^ n2.negative) // If n2 is negative and n1 isn't then true, if n1 is negative and n2 isn't then false
            {
                return n2.negative;
            }
            // If both numbers are negative then our expectations are reversed, remember this
            bool inversed = n1.negative && n2.negative;
            // Check exponents first
            if (n1.Exponent == n2.Exponent)
            {
                if (inversed) return n1.Multiplier < n2.Multiplier;
                return n1.Multiplier > n2.Multiplier;
            }
            else
            {
                if (inversed) return n1.Exponent < n2.Exponent;
                return n1.Exponent > n2.Exponent;
            }
        }

        public static bool operator < (Number n1, Number n2)
        {
            if (n1.negative ^ n2.negative) // If n2 is negative and n1 isn't then false, if n1 is negative and n2 isn't then true
            {
                return n1.negative;
            }
            // If both numbers are negative then our expectations are reversed, remember this
            bool inversed = n1.negative && n2.negative;
            // Check exponents first
            if (n1.Exponent == n2.Exponent)
            {
                if (inversed) return n1.Multiplier > n2.Multiplier;
                return n1.Multiplier < n2.Multiplier;
            }
            else
            {
                if (inversed) return n1.Exponent > n2.Exponent;
                return n1.Exponent < n2.Exponent;
            }
        }

        public static bool operator == (Number n1, Number n2)
        {
            // Make sure that all variables are the same
            return n1.Multiplier == n2.Multiplier && n1.Exponent == n2.Exponent && n1.negative == n2.negative;
        }

        public static bool operator != (Number n1, Number n2)
        {
            // If any one variable is different then we return true
            return n1.Multiplier != n2.Multiplier || n1.Exponent != n2.Exponent || n1.negative != n2.negative;
        }

        public static bool operator >= (Number n1, Number n2)
        {
            return n1 > n2 || n1 == n2;
        }

        public static bool operator <= (Number n1, Number n2)
        {
            return n1 < n2 || n1 == n2;
        }

        public static bool operator & (Number n1, Number n2)
        {
            if (n1)
            {
                if (n2) return true;
            }
            else if (n2)
            {
                return false;
            }
            return false;
        }

        public static bool operator | (Number n1, Number n2)
        {
            if (n1)
            {
                if (n2) return true;
                return true;
            }
            if (n2) return true;
            return false;
        }

        public static bool operator ^ (Number n1, Number n2)
        {
            if (n1)
            {
                if (n2) return false;
                return true;
            }
            if (n2) return true;
            return false;
        }

        public static bool operator true(Number n1)
        {
            return n1 != Zero;
        }

        public static bool operator false(Number n1)
        {
            return n1 == Zero;
        }

        public static bool operator ! (Number n1)
        {
            if (n1) return false;
            return true;
        }

        public override int GetHashCode()
        {
            long t = trueValue;
            while (t > int.MaxValue)
            {
                t /= 10;
            }
            return Convert.ToInt32(t) + Convert.ToInt32(Exponent) * (negative ? -1 : 1);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Number)) return false;
            return this == (Number)obj;
        }

        #endregion

        #region Implicit Casts

        public static implicit operator Number(int i) => new Number(i, 0); // We have a system for this already
        public static implicit operator Number(long i) => new Number(i, 0); // We have a system for this already
        public static implicit operator Number(float i) => (Number)(double)i; // Just do a double conversion        
        public static implicit operator Number(double i) // We need to remove decimals before anything else
        {
            // Fix Double being rude
            if (double.IsNaN(i)) i = 0;
            else if (double.IsPositiveInfinity(i)) i = double.MaxValue;
            else if (double.IsNegativeInfinity(i)) i = double.MinValue;

            // Is this between a normal long
            if (long.MaxValue >= i && long.MinValue <= i)
            {
                int exponent = 0;
                long multiplier = Convert.ToInt64(Math.Floor(i));
                string value = i.ToString();
                bool negative = value[0] == '-';
                int decimalloc = value.IndexOf('.');
                long sigFigCap = LongExtensions.TenToThePowerOf(SignificantFigures);

                // Is there a decimal point
                if (decimalloc >= 0)
                {
                    // Are we a large enough number we're all good
                    if (multiplier >= sigFigCap)
                    {
                        return multiplier; // Yup
                    }
                    // Else we go to the painful part
                    decimalloc++;
                    while (multiplier < sigFigCap && decimalloc < value.Length)
                    {
                        multiplier *= 10;
                        multiplier += Convert.ToInt64(char.GetNumericValue(value[decimalloc++]));
                        exponent--;
                    }
                    if (negative) multiplier *= -1;
                    return new Number(multiplier, exponent);
                }
                else
                {
                    // There is no decimal point, we're done here
                    return multiplier;
                }


                //int exponent = 0;
                //long errorless = (long)Math.Floor(i);
                //// If we are out of decimals, or we are past sig figs then continue
                //double x = 1;
                ////UnityEngine.Debug.Log(((i % x) * 10 / x));
                //while (i % x != 0 && ((i % x) * 10 / x) < 9.9999d && errorless < LongExtensions.TenToThePowerOf(SignificantFigures) && exponent > (-SignificantFigures) + 1)
                //{
                //    errorless *= 10;
                //    errorless += (long)(Math.Round((i % x) * 10 / x, 1));
                //    exponent--;
                //    x /= 10;
                //    //UnityEngine.Debug.Log(((i % x) * 10 / x));
                //}
                ////UnityEngine.Debug.Log(((i % x) * 10 / x));
                //return new Number(errorless, exponent);
            }
            else
            {
                long exponent = 0;
                while (i > long.MaxValue || i < long.MinValue)
                {
                    exponent++;
                    i /= 10;
                }
                return new Number(Convert.ToInt64(i), exponent);
            }
        }

        public static implicit operator string(Number i) => i.ToString();

        #endregion

        #region Explicit Casts

        public static explicit operator int(Number i)
        {
            if ((Number)int.MaxValue <= i) return int.MaxValue;
            //Debug.Log($"Multiplier = {i.Multiplier}, Exponent = {i.exponent}, Combined = {Math.Pow(10, i.exponent) * i.Multiplier}");
            double output = Math.Pow(10, i.Exponent) * i.Multiplier * (i.Negative ? -1 : 1);
            //Debug.Log($"Output = {output}, and when cast becomes {(int)output}");
            //Debug.Log($"Output = {output}, and when cast becomes {Convert.ToInt32(output)}");
            return Convert.ToInt32(output);
        }
        public static explicit operator long(Number i)
        {
            if ((Number)long.MaxValue <= i) return long.MaxValue;
            double output = Math.Pow(10, i.Exponent) * i.Multiplier * (i.Negative ? -1 : 1);
            return Convert.ToInt64(output);
        }
        public static explicit operator float(Number i)
        {
            if ((Number)float.MaxValue <= i) return i.Negative ? float.MinValue : float.MaxValue;
            double output = Math.Pow(10, i.Exponent) * i.Multiplier * (i.Negative ? -1 : 1);
            return Convert.ToSingle(output);
        }
        public static explicit operator double(Number i)
        {
            if ((Number)double.MaxValue <= i) return i.Negative ? double.MinValue : double.MaxValue;
            return Math.Pow(10, i.Exponent) * i.Multiplier * (i.Negative ? -1 : 1);
        }
        public static Number FromBigInt(System.Numerics.BigInteger bigI)
        {
            if (bigI < (System.Numerics.BigInteger)double.MaxValue) return (Number)(double)bigI;
            long exponent = 0;
            System.Numerics.BigInteger maxLong = (System.Numerics.BigInteger)long.MaxValue / 100;
            while (bigI > maxLong)
            {
                exponent += 8;
                bigI /= 100000000;
            }
            return new Number((long)bigI, exponent);
        }

        #endregion

        #region Convenient Shorthands

        public static Number One
        {
            get { return new Number(1, 0); }
        }

        public static Number Zero
        {
            get { return new Number(0, 0); }
        }

        public static Number Maximum
        {
            get { return new Number(long.MaxValue, long.MaxValue - 807); }
        }

        public static Number Minimum
        {
            get { return new Number(long.MinValue, long.MaxValue - 807); }
        }

        #endregion

        #region ToString Overrides and Extensions

        private static string ImprovedSuffixEfficient(long correctExponent, StringBuilder finalSuffix, int startingIndex, int remainder)
        {
            // We have already subtracted 3 and divided by 3
            if (correctExponent == 0) return finalSuffix.Append("Thousand").ToString();
            // Fractions do not work on the Conway long scale
            if (correctExponent < 0)
            {
                if (correctExponent == -1)
                {
                    finalSuffix.Remove(finalSuffix.Length - 1, 1);
                    return finalSuffix.ToString();
                }
                switch (remainder)
                {
                    case -3:
                        return finalSuffix.Append("x 10^").Append((correctExponent * 3 + 3) - remainder - 2).ToString();
                    case -1:
                        return finalSuffix.Append("x 10^").Append((correctExponent * 3 + 3) - remainder - 1).ToString();
                    default:
                        return finalSuffix.Append("x 10^").Append((correctExponent * 3 + 3) - remainder).ToString();
                }
            }
            if (correctExponent < 10) return finalSuffix.Append(StandardNamesCapital[correctExponent]).Append("on").ToString();
            if (correctExponent < 0) correctExponent *= -1;

            // Get our new base
            for (; correctExponent >= 1; correctExponent /= 1000)
            {
                int position1 = Convert.ToInt32(correctExponent % 10);
                int position10 = Convert.ToInt32((correctExponent / 10) % 10);
                int position100 = Convert.ToInt32((correctExponent / 100) % 10);
                // First is this a '00X'
                if (position10 == 0 && position100 == 0)
                {
                    finalSuffix.Insert(startingIndex, StandardNames[position1]);
                }
                else
                {
                    finalSuffix.Insert(startingIndex, ConwayLongScale[position100, 2]);
                    finalSuffix.Insert(startingIndex, ConwayLongScale[position10, 1]);
                    finalSuffix.Insert(startingIndex, ConwayLongScale[position1, 0]);
                }
            }

            // Now to parse the finalSuffix correctly, first I will append the end of it, this will remove any ending i's and any ending illi's
            for (int i = finalSuffix.Length - 1; i >= 0; i--)
            {
                if (finalSuffix[i] == 'i' || finalSuffix[i] == 'l' || finalSuffix[i] == 'a' || finalSuffix[i] == ' ') finalSuffix.Remove(i, 1);
                else break;
            }
            //finalSuffix = finalSuffix.TrimEnd(new char[] { 'i', 'l', 'a' });
            //if (negative) finalSuffix.Append("illionth"); else 
            finalSuffix.Append("illion");

            for (int i = finalSuffix.Length - 1; i >= startingIndex; i--)
            {
                char[] left = new char[] { '0', '0' }, right = new char[] { '0', '0' };
                // Find our first () set
                if (finalSuffix[i] == ')')
                {
                    // Store what's inside
                    int index = 0; i--;
                    while (i >= 0 && finalSuffix[i] != '(')
                    {
                        right[index] = finalSuffix[i];
                        finalSuffix.Remove(i, 1);
                        index++; i--;
                    }
                    i--;
                    // Check if we immediately have a matching set
                    if (i >= 0 && finalSuffix[i] == ')')
                    {
                        index = 0; i--;
                        while (i >= 0 && finalSuffix[i] != '(')
                        {
                            left[index] = finalSuffix[i];
                            finalSuffix.Remove(i, 1);
                            index++; i--;
                        }
                        // Now check for a match
                        for (int j = 0; j < 2 && i >= 0; j++)
                        {
                            if (left[j] == '0') continue;
                            for (int k = 0; k < 2; k++)
                            {
                                if (right[k] == '0') continue;
                                if (left[j] == right[k]) finalSuffix.Insert(i, left[j].ToString());
                                if (left[j] == '*' && (right[k] == 's' || right[k] == 'x')) finalSuffix.Insert(i, 's');
                                //if (left[j] == '*' && right[k] == 'x') finalSuffix.Insert(i, 'x');
                            }
                        }
                    }

                }
            }
            // Lastly remove the ()'s
            finalSuffix.Replace("(", "");
            finalSuffix.Replace(")", "");
            finalSuffix.Replace("*", "");
            finalSuffix[startingIndex] = char.ToUpper(finalSuffix[startingIndex]);
            return finalSuffix.ToString();
        }

        // Combine them all together, look for an open parenthesis, store the next letters before the closing parenthesis and delete all those characters
        //      then check if there is immediately a new open parenthesis, if not throw your memory out, if there is look for similar internal characters
        //      if there aren't any, throw them out, if there are, keep that one character and throw the rest out.
        private static string[,] ConwayLongScale =
        {
            // 1's          10's                100's
             { "",          "",                 "" },                 // 0
             { "un",        "(n)deci",          "(nx)centi" },        // 1
             { "duo",       "(ms)viginti",      "(n)ducenti" },       // 2
             { "tre(*)",    "(ns)triginta",     "(ns)trecenti" },     // 3
             { "quattuor",  "(ns)quadraginta",  "(ns)quadringenti" }, // 4
             { "quin",      "(ns)quinquaginta", "(ns)quingenti" },    // 5
             { "se(sx)",    "(n)sexaginta",     "(n)sescenti" },      // 6
             { "septe(mn)", "(n)septuaginta",   "(n)septingenti" },   // 7
             { "octo",      "(mx)octoginta",    "(mx)octingenti" },   // 8
             { "nove(mn)",  "nonaginta",        "nongenti" },         // 9
        };

        private static string[] StandardNames =
        {
            "nilli", "milli", "billi", "trilli", "quadrilli", "quintilli", "sextilli", "septilli", "octilli", "nonilli"
        };
        private static string[] StandardNamesCapital =
        {
            "Nilli", "Milli", "Billi", "Trilli", "Quadrilli", "Quintilli", "Sextilli", "Septilli", "Octilli", "Nonilli"
        };

        private static void TwoDecimals(StringBuilder s, bool negative)
        {
            while (s.Length < 3) s.Append('0');
            s.Insert(s.Length - 2, '.');
            if (negative) s.Insert(0, '-');
            s.Append(' ');
        }

        public override string ToString()
        {
            if (Multiplier == 0) return "0";
            if (exponent == -13) return $"0.{LongExtensions.ReduceTenToThePowerOf(Multiplier, -11)}";
            if (exponent == -14) return $"0.0{LongExtensions.ReduceTenToThePowerOf(Multiplier, -12)}";

            int remainder = Convert.ToInt32((Exponent + SignificantFigures) % 3) - 1;
            long suffix = Convert.ToInt64(Math.Floor((Decimal)(Exponent + SignificantFigures - 3) / 3)); // -3 then /3 for Conway

            if (remainder == 0)
            {
                StringBuilder sb = new StringBuilder(LongExtensions.ReduceTenToThePowerOf(Multiplier, -9).ToString());
                TwoDecimals(sb, negative);
                return ImprovedSuffixEfficient(suffix, sb, sb.Length, remainder).ToString();
            }
            else if (remainder == 1)
            {
                StringBuilder sb = new StringBuilder(LongExtensions.ReduceTenToThePowerOf(Multiplier, -8).ToString());
                TwoDecimals(sb, negative);
                return ImprovedSuffixEfficient(suffix, sb, sb.Length, remainder).ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder(LongExtensions.ReduceTenToThePowerOf(Multiplier, -10).ToString());
                TwoDecimals(sb, negative);
                return ImprovedSuffixEfficient(suffix, sb, sb.Length, remainder).ToString();
            }
        }

        public string ToShortString(bool engineeringNotation = false)
        {
            if (Convert.ToInt64(Math.Floor((Decimal)(Exponent + SignificantFigures - 3) / 3)) == -1) // -3 then /3 for Conway
            {
                // Write these normally
                return ToString();
            }
            StringBuilder sb = new StringBuilder(LongExtensions.ReduceTenToThePowerOf(Multiplier, -10).ToString());
            TwoDecimals(sb, negative);
            if (!engineeringNotation)
                sb.Append("x 10^");
            else
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append("e");
            }
            sb.Append(Exponent + SignificantFigures);
            return sb.ToString();
        }

        /// <summary>
        /// DO NOT CALL THIS REGULARLY!! This will *actually* print out the value, which is a lot of zeroes likely
        /// </summary>
        /// <returns></returns>
        public string TooManyZeros()
        {
            if (exponent < -SignificantFigures) return "0";
            if (exponent < 0)
            {
                // Shorten the string
                string returnVal = Multiplier.ToString();
                return returnVal.Substring(0, SignificantFigures + 1 + (int)exponent);
            }
            return Multiplier.ToString() + new string('0', (int)exponent);
        }

        #endregion

        #region Private Methods for Math

        /// <summary>
        /// Pass the smaller of the Numbers into this first
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static Tuple<long, long> MultiplierPrep(Number n)
        {
            long multiplier = n.Multiplier;
            long exponent = n.Exponent;
            // longs are 19 digits long, sooo multiplying is limited to 6 sig figs of the smaller number
            while (multiplier >= LongExtensions.TenToThePowerOf(6))
            {
                multiplier /= 10;
                exponent++;
            }
            return new Tuple<long, long>(multiplier, exponent);
        }

        #endregion

    }

    #region Custom Long Functionality

    public static class LongExtensions
    {
        /// <summary>
        /// Returns 10 ^ X
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static long TenToThePowerOf(int power)
        {
            long value = 1;
            for (int i = 0; i < power; i++)
            {
                value *= 10;
            }
            return value;
        }

        /// <summary>
        /// Returns 10 ^ X
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static long TenToThePowerOf(long power)
        {
            long value = 1;
            for (long i = 0; i < power; i++)
            {
                value *= 10;
            }
            return value;
        }

        /// <summary>
        /// Reduces a long by a power and returns the result, precision is lost with this
        /// </summary>
        /// <param name="l"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static long ReduceTenToThePowerOf(long l, int power) => ReduceTenToThePowerOf(l, Convert.ToInt64(power));

        /// <summary>
        /// Reduces a long by a power and returns the result, precision is lost with this
        /// </summary>
        /// <param name="l">Base Long</param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static long ReduceTenToThePowerOf(long l, long power)
        {
            if (power < 0) power *= -1;
            long value = l;
            for (long i = 0; i < power; i++)
            {
                value /= 10;
            }
            return value;
        }
    }

    #endregion

    public static class MathHuge
    {
        /* a^b = x
         * log(a^b) = log(x)
         * b * log(a) = log(x)
        */

        /// <summary>
        /// Returns a ^ b
        /// </summary>
        /// <param name="a">Denominator</param>
        /// <param name="b">Exponent</param>
        /// <returns></returns>
        public static Number Power(Number a, double b)
        {
            if (a == 0) return 0;
            if (b == 0) return 1;
            if (b == 1) return a;
            double c = b * Log(a);
            if (double.IsInfinity(c)) c = double.MaxValue;
            Number d = Math.Pow(10, c - Math.Floor(c));
            Number e = new Number(10, (Convert.ToInt64(Math.Floor(c)) - 1));
            return (d * e);// / 10;
        }
        /// <summary>
        /// Returns a ^ b
        /// </summary>
        /// <param name="a">Denominator</param>
        /// <param name="b">Exponent</param>
        /// <returns></returns>
        public static Number Power(Number a, float b) => Power(a, (double)b);
        /// <summary>
        /// Returns a ^ b
        /// </summary>
        /// <param name="a">Denominator</param>
        /// <param name="b">Exponent</param>
        /// <returns></returns>
        public static Number Power(Number a, long b) => Power(a, (double)b);
        /// <summary>
        /// Returns a ^ b
        /// </summary>
        /// <param name="a">Denominator</param>
        /// <param name="b">Exponent</param>
        /// <returns></returns>
        public static Number Power(Number a, int b) => Power(a, (double)b);
        /// <summary>
        /// Returns a ^ b
        /// </summary>
        /// <param name="a">Denominator</param>
        /// <param name="b">Exponent</param>
        /// <returns></returns>
        public static Number Power(Number a, Number b) => Power(a, (double)b);

        /// <summary>
        /// Returns Log base 10 of the input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Log(Number value)
        {
            return value.Exponent + Math.Log10(value.Multiplier);
        }

        /// <summary>
        /// Returns Log base baseValue of the value
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Log(Number baseValue, Number value)
        {
            return (value.Exponent + Math.Log10(value.Multiplier)) / (baseValue.Exponent + Math.Log10(baseValue.Multiplier));
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Number Floor(Number value)
        {
            if (value.Exponent >= 0) return value;
            //UnityEngine.Debug.Log(value.Exponent);
            if (value.Exponent < -Number.SignificantFigures)
            {
                if (value.Negative) return -Number.One;
                return Number.Zero;
            }

            Number n1 = value;
            //UnityEngine.Debug.Log(value.Exponent);
            //UnityEngine.Debug.Log(LongExtensions.TenToThePowerOf(-value.Exponent));
            //UnityEngine.Debug.Log(n1.Multiplier % LongExtensions.TenToThePowerOf(-value.Exponent));
            n1 -= new Number(n1.Multiplier % LongExtensions.TenToThePowerOf(-value.Exponent), n1.Exponent);
            return n1;
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to the input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Number Ceil(Number value)
        {
            if (value.Exponent >= 0) return value;
            if (value.Exponent < -Number.SignificantFigures)
            {
                if (value.Negative || value.Multiplier == 0) return Number.Zero;
                return Number.One;
            }

            Number n1 = value;
            n1 -= new Number((n1.Multiplier % LongExtensions.TenToThePowerOf(-value.Exponent) == 0) ? 0 : n1.Multiplier % LongExtensions.TenToThePowerOf(-value.Exponent) - LongExtensions.TenToThePowerOf((-value.Exponent)), n1.Exponent);
            return n1;
        }

        /// <summary>
        /// Returns the absolute value of the input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Number Abs(Number value)
        {
            if (!value.Negative) value *= -1;
            return value;
        }

        /// <summary>
        /// Returns the closest value that is greater than the min and less than the max
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Number Clamp(Number value, Number min, Number max)
        {
            if (value > max) value = max;
            else if (value < min) value = min;
            return value;
        }

        /// <summary>
        /// <br>Interpolate between A and B over time</br>
        /// <br>Time is clamped between 0 and 1</br>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Number Lerp01(Number a, Number b, float t)
        {
            if (t < 0) t = 0;
            else if (t > 1) t = 1;
            return (b - a) * t + a;
        }

        /// <summary>
        /// <br>Interpolate between A and B over time</br>
        /// <br>Time is unclamped</br>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Number Lerp(Number a, Number b, float t)
        {
            return (b - a) * t + a;
        }

        public static Number Round(Number value)
        {
            if (value.Exponent >= 0) return value;

            if (value - Floor(value) >= Number.One / 2) return Ceil(value);
            return Floor(value);
        }

        public static Number Max(Number n1, Number n2)
        {
            return n1 > n2 ? n1 : n2;
        }

        public static Number Min(Number n1, Number n2)
        {
            return n1 > n2 ? n2 : n1;
        }
    }
}




