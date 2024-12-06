using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IOperation
{
    string Operator { get; }
    double Calculate(double left, double right);
}

public abstract class BaseOperation : IOperation
{
    public abstract string Operator { get; }
    public abstract double Calculate(double left, double right);
}

public class AddOperation : BaseOperation
{
    public override string Operator => "+";
    public override double Calculate(double left, double right) => left + right;
}

public class SubtractOperation : BaseOperation
{
    public override string Operator => "-";
    public override double Calculate(double left, double right) => left - right;
}

public class MultiplyOperation : BaseOperation
{
    public override string Operator => "*";
    public override double Calculate(double left, double right) => left * right;
}

public class DivideOperation : BaseOperation
{
    public override string Operator => "/";
    public override double Calculate(double left, double right)
    {
        if (right == 0) throw new ArgumentException("Деление на ноль невозможно хихихихих)");
        return left / right;
    }
}

public class ExpOperation : BaseOperation
{
    public override string Operator => "exp";
    public override double Calculate(double left, double right) => Math.Exp(right);
}

public class ModuloOperation : BaseOperation
{
    public override string Operator => "mod";
    public override double Calculate(double left, double right) => left % right;
}

public class PowerOperation : BaseOperation
{
    public override string Operator => "^";
    public override double Calculate(double left, double right) => Math.Pow(left, right);
}

public class LogOperation : BaseOperation
{
    public override string Operator => "log";
    public override double Calculate(double left, double right) => Math.Log(right, left);
}

public class OperationFactory
{
    private readonly Dictionary<string, Func<IOperation>> _operations = new Dictionary<string, Func<IOperation>>()
    {
        { "+", () => new AddOperation() },
        { "-", () => new SubtractOperation() },
        { "*", () => new MultiplyOperation() },
        { "/", () => new DivideOperation() },
        { "log", () => new LogOperation() },
        { "exp", () => new ExpOperation() },
        { "mod", () => new ModuloOperation() },
        { "^", () => new PowerOperation() }
    };

    public IOperation GetOperation(string op) => _operations.TryGetValue(op, out var operation) ? operation() : null;
}

public class Calculator
{
    private readonly OperationFactory _operationFactory;

    public Calculator(OperationFactory operationFactory)
    {
        _operationFactory = operationFactory;
    }

    public double Calculate(string expression)
    {
        DateTime startTime = DateTime.Now;
        Console.WriteLine($"Начало вычисления: {startTime}, Поток: {Thread.CurrentThread.ManagedThreadId}");

        string[] parts = expression.Split(' ');
        if (parts.Length != 3) throw new ArgumentException("Неверный формат");

        if (!double.TryParse(parts[0], out double left) || !double.TryParse(parts[2], out double right))
            throw new ArgumentException("Неверные символы");

        IOperation operation = _operationFactory.GetOperation(parts[1]);
        if (operation == null) throw new ArgumentException($"Неизвестная операция {parts[1]}");

        double result = operation.Calculate(left, right);

        DateTime endTime = DateTime.Now;
        Console.WriteLine($"Завершение вычисления: {endTime}, Поток: {Thread.CurrentThread.ManagedThreadId}");
        Console.WriteLine($"Время выполнения: {endTime - startTime}");

        return result;
    }

    public async Task<double> CalculateAsync(string expression)
    {
        DateTime startTime = DateTime.Now;
        Console.WriteLine($"Начало вычисления: {startTime}, Поток: {Thread.CurrentThread.ManagedThreadId}");

        string[] parts = expression.Split(' ');
        if (parts.Length != 3) throw new ArgumentException("Неверный формат");

        if (!double.TryParse(parts[0], out double left) || !double.TryParse(parts[2], out double right))
            throw new ArgumentException("Неверные символы");

        IOperation operation = _operationFactory.GetOperation(parts[1]);
        if (operation == null) throw new ArgumentException($"Неизвестная операция {parts[1]}");

        double result1 = await Task.Run(() => operation.Calculate(left, right));
        double result2 = await Task.Run(() => operation.Calculate(left, right));
        double result3 = await Task.Run(() => operation.Calculate(left, right));

        double finalResult = result1 + result2 + result3;

        DateTime endTime = DateTime.Now;
        Console.WriteLine($"Завершение вычисления: {endTime}, Поток: {Thread.CurrentThread.ManagedThreadId}");
        Console.WriteLine($"Время выполнения: {endTime - startTime}");

        return finalResult;
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var factory = new OperationFactory();
        var calculator = new Calculator(factory);

        while (true)
        {
            Console.Write("введите уравнение: ");
            string input = Console.ReadLine();
            if (input.ToLower() == "quit") break;

            try
            {
                double result = await calculator.CalculateAsync(input);
                Console.WriteLine($"Результат: {result:F3}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
