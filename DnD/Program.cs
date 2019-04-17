using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace DnD {
    class Program {
        static RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
        static int numberOfDice;
        static int diceSides;
        static int modifierAmount;
        static char modifier;
        static int finalResult;

        public static int NumberOfDice {
            get { return numberOfDice; }
            set { numberOfDice = value; }
        }
        public static int DiceSides {
            get { return diceSides; }
            set { diceSides = value; }
        }
        public static int ModifierAmount {
            get { return modifierAmount; }
            set { modifierAmount = value; }
        }
        public static char Modifier {
            get { return modifier; }
            set { modifier = value; }
        }
        public static int FinalResult {
            get { return finalResult; }
            set { finalResult = value; }
        }

        static void Main(string[] args) {
            Console.WriteLine("Modifiers other than + or - result in a +0");
            Roll();
        }

        static byte RollDice(int _intSides) {
            byte numSides = Convert.ToByte(_intSides);

            if (numSides <= 0) { throw new ArgumentOutOfRangeException("RollDice parameter out of range: _numSides"); }

            byte[] randomNum = new byte[1];
            do { rngCSP.GetBytes(randomNum); }
            while (!IsFairRoll(randomNum[0], numSides));

            return (byte)((randomNum[0] % numSides) + 1);
        }

        static bool IsFairRoll(byte roll, byte _numSides) {
            int fullSetsOfValues = byte.MaxValue / _numSides;
            return roll < _numSides * fullSetsOfValues;
        }

        static bool IsDnDNotation(string _input) {
            Regex parseInput = new Regex(@"\d*d\d+");
            Match matchNotation = parseInput.Match(_input);

            if (matchNotation.ToString() == string.Empty) { return false; }
            else { return true; }
        }

        static void GetNumberOfDice(string _input) {
            Regex parseNumberOfDice = new Regex(@"\d*d");
            Match matchNumberOfDice = parseNumberOfDice.Match(_input);

            if (int.TryParse(matchNumberOfDice.ToString().Remove(matchNumberOfDice.Length - 1), out int _n)) {
                NumberOfDice = _n;
            }
            else { NumberOfDice = 1; }
        }

        static void GetNumberOfDiceSides(string _input) {
            Regex parseDiceSides = new Regex(@"[d]\d+");
            Match matchDiceSides = parseDiceSides.Match(_input);

            if (int.TryParse(matchDiceSides.ToString().Substring(1), out int _n)) {
                DiceSides = _n;
            }
            else { DiceSides = int.MaxValue; }
        }

        static void GetModifier(string _input) {
            Regex parseModifier = new Regex(@"(\-|\+)\d+");
            Match matchModifier = parseModifier.Match(_input);
            if (matchModifier.ToString() == string.Empty) {
                Modifier = '+';
                ModifierAmount = 0;
            }
            else {
                Modifier = matchModifier.ToString()[0];
                if (int.TryParse(matchModifier.ToString().Substring(1), out int n)) { ModifierAmount = n; }
                else {
                    Modifier = '`';
                    ModifierAmount = int.MinValue;
                }
            }
        }

        static void DetectErrors() {
            if(DiceSides == int.MaxValue) { Console.WriteLine("Something failed on the way to GetNumberOfDiceSides"); }
            if(Modifier == '`') { Console.WriteLine("Something failed on the way to GetModifier"); }
        }

        static void Parse(string _notation) {
            GetNumberOfDice(_notation);
            GetNumberOfDiceSides(_notation);
            GetModifier(_notation);
            DetectErrors();
        }

        static void RollSingleDice() {
            switch (Modifier) {
                case '+':
                    FinalResult = RollDice(DiceSides) + ModifierAmount;
                    break;
                case '-':
                    FinalResult = RollDice(DiceSides) - ModifierAmount;
                    break;
                case '`':
                    Console.WriteLine("Something failed on the way to GetModifier and DetectErrors didn't catch it");
                    break;
            }
        }

        static void RollMultipleDice() {
            int diceRollsTotal = 0;
            for(int i = 0; i < NumberOfDice; i++) {
                diceRollsTotal += RollDice(DiceSides);
            }
            switch (Modifier) {
                case '+':
                    FinalResult = diceRollsTotal + ModifierAmount;
                    break;
                case '-':
                    FinalResult = diceRollsTotal - ModifierAmount;
                    break;
                case '`':
                    Console.WriteLine("Something failed on the way to GetModifier and DetectErrors didn't catch it");
                    break;
            }
        }

        static void RollAgain() {
            Console.Write("\nEnter Y to continue or press Enter to quit: ");
            string _ans = Console.ReadLine();
            if (_ans == "y" || _ans == "Y") {
                Console.WriteLine("");
                Roll();
            }
            else {
                Environment.Exit(0);
            }
        }

        static void Roll() {
            Console.Write("Enter Your Roll: ");
            string notation = Console.ReadLine();
            notation = notation.ToLower();
            notation = notation.Replace(" ", string.Empty);

            if (IsDnDNotation(notation)) {
                Parse(notation);
                DetectErrors();

                if (NumberOfDice == 1) {
                    RollSingleDice();
                }
                else {
                    RollMultipleDice();
                }
                Console.WriteLine("Result: {0}", FinalResult);

                RollAgain();
            }
            else {
                Console.WriteLine("\nPlease Use Proper DnD Notation");
                Console.WriteLine("Examples: '2d20' or 'd100' or '3d8-3'");
                RollAgain();
            }
        }
    }
}