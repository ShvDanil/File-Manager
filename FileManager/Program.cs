using System;
using System.Linq;
/*
Информация: Это консольное приложение - "Файловый менеджер", которое содержит набор нумерованных текстовых меню и 
может выполнять следующие операции:
1) просмотр списка дисков компьютера и выбор диска;
2) переход в другую директорию (выбор папки);
3) просмотр списка файлов в директории;
4) вывод содержимого текстового файла в консоль в выбранной пользователем кодировке;
5) копирование файла;
6) перемещение файла в выбранную пользователем директорию;
7) удаление файла;
8) создание простого текстового файла в выбранной пользователем кодировке;
9) конкатенация содержимого двух или более текстовых файлов и вывод результата в консоль в кодировке UTF-8.
Данное консольное приложение разработано для C# .NET 5.0. Тип проекта - Console Application .NET 5.0. 
Дисциплина: "Программирование".
Группа: БПИ21"рандомное число от 1 до 11".
Студент: Анонимный аноним.
Дата дедлайна: Понедельник, 8 ноября, 23:59.
*/
namespace FileManager
{
    /*
        Основной класс Program.
        Содержит все методы, необходимые для работы консольного приложения "Файловый менеджер". 
    */
    internal static class Program
    {
        /// <summary>
        /// Данный метод продолжает работу программы после определенной операции.
        /// </summary>
        private static void ContinueProgramExecution()
        {
            Console.WriteLine("Нажмите \"ENTER\", чтобы продолжить");
            
            ConsoleKeyInfo continueKey;
            do
            {
                continueKey = Console.ReadKey();
                Console.WriteLine();
                
                if (continueKey.Key != ConsoleKey.Enter)
                    Console.WriteLine("Вы нажали неверную кнопку, повторите действие!");
                
            } while (continueKey.Key != ConsoleKey.Enter);
        }
        
        /// <summary>
        /// Данный метод приветствует пользователя.
        /// </summary>
        private static void GreetUser()
        {
            Console.WriteLine("Дорогой пользователь, добро пожаловать в консольное приложение " +
                              "\"Файловый менеджер\"!\nПриступая к работе, настоятельно рекомендую ознакомиться " +
                              "с файлом README.docx во избежание различных недопониманий.\nВ нем находятся описания " +
                              "доступных операций, а также примечания к коду.\nЖелаю приятного пользования!\n");
            
            ContinueProgramExecution();
        }

        /// <summary>
        /// Данный метод выводит на экран полную версию доступных пользователю операций и их кратких описаний.
        /// </summary>
        private static void PrintFullInstructions()
        {
            Console.WriteLine("Ниже указаны доступные операции!\n");
            
            // Печать на экран полной версии команд.
            for (var i = 0; i < Operations.Commands.GetLength(0); i++)
            {
                Console.WriteLine($"---- {Operations.Commands[i, 1]} ----");
                Console.WriteLine($"{Operations.Commands[i, 0]}\n");
            }
            
            ContinueProgramExecution();
        }

        /// <summary>
        /// Данный метод выводит на экран краткую версию доступных пользователю операций и их кратких описаний.
        /// </summary>
        private static void PrintShortInstructions()
        {
            Console.WriteLine();
            
            // Печать на экран кратких команд.
            for (var i = 0; i < Operations.Commands.GetLength(0); i++)
                Console.WriteLine(Operations.Commands[i, 1]);

            Console.WriteLine();
        }

        /// <summary>
        /// Данный метод просит пользователя выбрать операцию, после выбора операция выполняется.
        /// </summary>
        private static void ChooseAndExecuteOperation(out string disk)
        {
            // Просим пользователя выбрать операцию для выполнения и запоминаем результат в переменную operation.
            string operation;
            do
            {
                Console.Write("Выберите операцию (чтобы получить их краткий список, напечатайте \"help\"): ");
                operation = Console.ReadLine();

                if (operation == "help")
                {
                    PrintShortInstructions();
                    continue;
                }

                if (!("pdl csd sfl pfd cpf mvf rmf ctf mtf".Split(" ").Contains(operation) |
                           "1 2 3 4 5 6 7 8 9".Split(" ").Contains(operation)))
                {
                    Console.WriteLine("\nВы ввели неверную операцию, повторите попытку!\n"); 
                    continue;
                }
                break;
            } while (true);
            
            // Создаем новый объект класса операций, передаем ему выбранную операцию и выполняем ее.
            new Operations(operation, out disk).DoOperation();
        }

        /// <summary>
        /// Данный метод отвечает за реализацию повторения решения или за окончание работы программы.
        /// </summary>
        /// <param name="flag">Флаг, отвечающий за повторение всей программы при неверно нажатой кнопке.</param>
        /// <param name="keyToExit">Кнопка, отвечающая за продолжение, прерывание программы и печать информации
        /// об операциях.</param>
        private static void ContinueOrEndProgram(ref bool flag, out ConsoleKeyInfo keyToExit)
        {
            Console.WriteLine("Чтобы продолжить без напоминания об операциях, нажмите \"ENTER\"\n" +
                              "Чтобы продолжить с напоминанием об операциях, нажмите \"H\" или \"h\"\n" +
                              "Чтобы выйти и завершить работу консольного приложения, нажмите \"BACKSPACE\"");
            
            keyToExit = Console.ReadKey();
            Console.WriteLine();
            
            if (keyToExit.Key == ConsoleKey.H)
            {
                flag = true;
                PrintFullInstructions();
            }
            else if (keyToExit.Key == ConsoleKey.Enter)
            {
                flag = true;
            }
            else if (keyToExit.Key != ConsoleKey.Backspace)
            {
                Console.WriteLine("Вы нажали неверную кнопку, повторите действие!\n");
                flag = false;
            }
        }

        /// <summary>
        /// Данный метод прощается с пользователем.
        /// </summary>
        private static void FarewellUser()
        {
            Console.WriteLine("Дорогой пользователь, благодарю за использование данного консольного приложения " +
                              "\"Файловый менеджер\"!\nЖелаю удачи и до скорых встреч!");
        }
        
        public static void Main()
        {
            // Приветствие с пользователем.
            GreetUser();
            
            // Вывод пользователю доступных операций.
            PrintFullInstructions();
            
            // Объвление переменной keyToExit, отвечающей за работоспособность программы, и переменной flag. 
            ConsoleKeyInfo keyToExit; 
            var flag = true;
            
            // Цикл выполняющий программу или завершающий ее.
            do
            {
                if (flag)
                {
                    // Выбор операции и ее выполнение.
                    ChooseAndExecuteOperation(out string disk);
                }
                
                // Продолжение выполнения программы вновь или ее завершение.
                ContinueOrEndProgram(ref flag, out keyToExit);
                
            } while (keyToExit.Key != ConsoleKey.Backspace);
            
            // Прощание с пользователем.
            FarewellUser();
        }
    }
}