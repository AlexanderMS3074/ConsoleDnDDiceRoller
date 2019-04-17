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
            Roll();
        }

        static byte RollDice(int _intSides) {
            byte _numSides = Convert.ToByte(_intSides);

            if (_numSides <= 0) { throw new ArgumentOutOfRangeException("RollDice parameter out of range: _numSides"); }

            byte[] _randomNum = new byte[1];
            do { rngCSP.GetBytes(_randomNum); }
            while (!IsFairRoll(_randomNum[0], _numSides));

            return (byte)((_randomNum[0] % _numSides) + 1);
        }

        static bool IsFairRoll(byte _roll, byte _numSides) {
            int _fullSetsOfValues = byte.MaxValue / _numSides;
            return _roll < _numSides * _fullSetsOfValues;
        }

        static void GetNumberOfDice(string _input) {
            if (int.TryParse(_input[0].ToString(), out int _n)) { numberOfDice = _n; }
            else { numberOfDice = 1; }
        }

        static void GetNumberOfDiceSides(string _input) {
            Regex _parseDiceSides = new Regex(@"[d]\d+");
            Match _matchDiceSides = _parseDiceSides.Match(_input);
            if (int.TryParse(_matchDiceSides.ToString().Substring(1), out int _n)) { DiceSides = _n; }
            else { DiceSides = int.MaxValue; }
        }

        static void GetModifier(string _input) {
            Regex _parseModifier = new Regex(@"(\-|\+)\d+");
            Match _matchModifier = _parseModifier.Match(_input);
            if (_matchModifier.ToString() == string.Empty) {
                Modifier = '+';
                ModifierAmount = 0;
            }
            else {
                Modifier = _matchModifier.ToString()[0];
                if (int.TryParse(_matchModifier.ToString().Substring(1), out int n)) { ModifierAmount = n; }
                else {
                    Modifier = '`';
                    ModifierAmount = int.MinValue;
                }
            }
        }

        static void DetectErrors() {
            if(DiceSides == int.MaxValue) { Console.WriteLine("Something fucked up on the way to GetNumberOfDiceSides"); }
            if(Modifier == '`') { Console.WriteLine("Something fucked up on the way to GetModifier"); }
        }

        static void Parse(string _rawNotation) {
            string _notation = _rawNotation.ToLower();
            _notation = _rawNotation.Replace(" ", string.Empty);

            GetNumberOfDice(_notation);
            GetNumberOfDiceSides(_notation);
            GetModifier(_notation);

            //Console.WriteLine("_notation.......: {0}", _notation);
            //Console.WriteLine("_numberOfDice...: {0}", DiceNumber);
            //Console.WriteLine("_numberOfSides..: {0}", DiceSides);
            //Console.WriteLine("_modifier.......: {0}", Modifier);
            //Console.WriteLine("_modifierAmount.: {0}", ModifierAmount);
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
                    Console.WriteLine("Something fucked up on the way to GetModifier and DetectErrors didn't catch it");
                    break;
            }
        }

        static void RollMultipleDice() {
            List<int> diceRolls = new List<int>();
            for(int i = 0; i < NumberOfDice; i++) {
                diceRolls.Add(RollDice(DiceSides));
            }
        }

        static void Roll() {
            Console.WriteLine("Enter Your Roll:");
            string notation = Console.ReadLine();

            Parse(notation);
            DetectErrors();

            int roll = RollDice(DiceSides);

            if (NumberOfDice == 1) {
                //TODO Finish RollSingleDice
            }
            else {
                //TODO Finish RollMultipleDice
            }

            Console.WriteLine(roll);
            Console.WriteLine(Modifier);
            Console.WriteLine(FinalResult);

            RollAgain();
        }

        static void RollAgain() {
            Console.WriteLine("Enter Y to continue or press Enter to quit");
            string _ans = Console.ReadLine();
            if (_ans == "y" || _ans == "Y") {
                Roll();
            } else {
                Environment.Exit(0);
            }
        }
    }
}