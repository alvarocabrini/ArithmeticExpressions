using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArithmeticExpressions.Controllers
{
    [Route("calculator")]
    [ApiController]
    public class CalculatorController : Controller
    {
        private static string ValidOperators => "*+-";

        [HttpPost]
        [Route("divisible101")]
        public IActionResult GetDivisibleByOneHundredOne([FromBody] string input)
        {
            var lineBreakCount = System.Text.RegularExpressions.Regex.Matches(input, "\n|\r|\n\r").Count;

            if (lineBreakCount != 2)
            {
                return BadRequest("you must enter two lines, " +
                    "first line contains a single integer denoting " +
                    "the number of elements in the list. The second " +
                    "line contains n space-separated integers a(1), a(2),..., a(n) denoting " +
                    "the elements of the list.");
            }

            var firstLine = input.Split(new[] { '\r', '\n' }).FirstOrDefault();
            var lastLine = input.Split(new[] { '\r', '\n' }).LastOrDefault();

            if (!FirstLineCorrectValidation(firstLine)
                || !SecondLineCorrectValidation(lastLine))
            {
                return BadRequest("Constraints: 2 <= n <= 10^4, 1 <= a(i) <= 100, The length of the output expression should not exceed 10(n).");
            }

            int[] numbers = Array.ConvertAll(lastLine.Split(' '), int.Parse);

            if (int.Parse(firstLine) != numbers.Length)
            {
                return BadRequest("You miss number value at first line");
            }

            var combinations = new List<string>();
            int numbersLength = numbers.Length - 1;
            GenerateOperationCombination(string.Empty, 0, numbersLength, ValidOperators, ref combinations);

            var operationPosible = new List<string>();
            var dibisibleBy = new List<string>();

            foreach (var item in combinations)
            {
                var stringBuilder = new System.Text.StringBuilder();

                for (int j = 0; j < numbers.Length || j < item.Length; j++)
                {
                    if (j < numbers.Length)
                    {
                        stringBuilder.Append(numbers[j] + " ");
                    }

                    if (j < item.Length)
                    {
                        stringBuilder.Append(item[j] + " ");
                    }
                }

                stringBuilder.AppendLine();

                operationPosible.Add(stringBuilder.ToString().Trim());
            }

            foreach (var item in operationPosible)
            {
                var valueCalculed = CalculateOperation(item);

                if (valueCalculed % 101 == 0)
                {
                    dibisibleBy.Add(item);
                }
            }

            return Ok(dibisibleBy);
        }

        private int CalculateOperation(string operation)
        {
            operation = operation.Replace(" ", "");

            int result = 0;
            bool firstIteration = false;

            while (operation.IndexOfAny(ValidOperators.ToCharArray()) != -1)
            {
                var operatorSymbol = "";
                var numberOne = 0;
                var numberTwo = 0;

                if (firstIteration)
                {
                    var firstOperator = operation.IndexOfAny(ValidOperators.ToCharArray());
                    numberOne = int.Parse(operation.Substring(0, firstOperator));

                    operation = operation.Substring(numberOne.ToString().Length, operation.Length - numberOne.ToString().Length);
                    operatorSymbol = operation.Substring(0, 1);
                    operation = operation.Remove(0, 1);

                    var secondOperator = operation.IndexOfAny(ValidOperators.ToCharArray());

                    numberTwo = int.Parse(operation.Substring(0, secondOperator));
                    operation = operation.Substring(numberTwo.ToString().Length, operation.Length - numberTwo.ToString().Length);

                    firstIteration = true;
                }
                else
                {
                    numberOne = result;
                    operatorSymbol = operation.Substring(0, 1);
                    operation = operation.Remove(0, 1);

                    var secondOperator = operation.IndexOfAny(ValidOperators.ToCharArray());

                    if (secondOperator != -1)
                    {
                        numberTwo = int.Parse(operation.Substring(0, secondOperator));
                        operation = operation.Substring(numberTwo.ToString().Length, operation.Length - numberTwo.ToString().Length);
                    }
                    else
                    {
                        numberTwo = int.Parse(operation);
                    }
                }

                switch (operatorSymbol)
                {
                    case "+":
                        result = numberOne + numberTwo;
                        break;

                    case "-":
                        result = numberOne - numberTwo;
                        break;

                    case "*":
                        result = numberOne * numberTwo;
                        break;

                    default:
                        throw new ArgumentException("Unexpected operator string: " + operatorSymbol);
                }
            }

            return result;
        }

        private bool FirstLineCorrectValidation(string firstLine)
        {
            if (!firstLine.All(char.IsDigit))
            {
                return false;
            }

            var number = int.Parse(firstLine);

            if (number < 2 || number > Math.Pow(10, 4))
            {
                return false;
            }

            return true;
        }

        private bool SecondLineCorrectValidation(string secondLine)
        {
            var stringNumbers = secondLine.Split(' ');
            var lastLineAllAreNumbers = string.Concat(stringNumbers).All(char.IsDigit);

            if (!lastLineAllAreNumbers)
            {
                return false;
            }

            int[] numbers = Array.ConvertAll(stringNumbers, int.Parse);

            if (numbers.Any(x => x < 1 || x > 100))
            {
                return false;
            }

            return true;
        }

        public static void GenerateOperationCombination(
            string text,
            int initialLength,
            int numbersLength,
            string validCharacters,
            ref List<string> combinations)
        {
            initialLength += 1;

            foreach (char character in validCharacters)
            {
                if ((text + character).Length == numbersLength)
                {
                    combinations.Add(text + character);
                }

                if (initialLength < numbersLength)
                {
                    GenerateOperationCombination(text + character, initialLength, numbersLength, validCharacters, ref combinations);
                }
            }
        }
    }
}