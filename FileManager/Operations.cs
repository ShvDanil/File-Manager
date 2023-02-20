using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManager
{
    /*
        Дополнительный класс Operations.
        Содержит все методы, которые используются при выполнении операций консольного приложения "Файловый менеджер". 
    */
    internal class Operations
    {
        /// <summary>
        /// Данный конструктор получает операцию, выбранную пользователем, и присваивает ее значение приватному полю,
        /// а также меняет значение диска для работы.
        /// </summary>
        /// <param name="operation">Операция, выбранная пользователем.</param>
        /// <param name="disk">Диск, выбранный пользователем, либо заданный по умолчанию по текущей директории.</param>
        public Operations(string operation, out string disk)
        {
            _operation = operation;
            disk = s_path;
        }
        
        // Приватное поле операции пользователя.
        private readonly string _operation;
        
        // Приватное поле выбранного диска для работы.
        private static string s_path = Directory.GetCurrentDirectory();
        
        // Приватное поле - двумерный массив с данными об операциях и их кратком текстовом представлении. 
        private static readonly string[,] s_commandsArray = 
        {
            {"просмотр списка дисков компьютера и выбор диска", "1 / pdl -> print disks list"},
            {"переход в другую директорию (выбор папки)", "2 / csd -> choose directory"},
            {"просмотр списка файлов в директории", "3 / sfl -> show files list"},
            {"вывод содержимого текстового файла в консоль в выбранной пользователем кодировке " +
             "(UTF-8, ASCII, UTF-16, UTF-32, Latin1 (ISO-8859-1))", "4 / pfd -> print text file data"},
            {"копирование файла", "5 / cpf -> copy file"},
            {"перемещение файла в выбранную пользователем директорию", "6 / mvf -> move file"},
            {"удаление файла", "7 / rmf -> remove file"},
            {"создание текстового файла в выбранной пользователем кодировке " +
             "(UTF-8, ASCII, UTF-16, UTF-32, Latin1 (ISO-8859-1))", "8 / ctf -> create text file"},
            {"конкатенация содержимого двух или более текстовых файлов и вывод результата в " +
             "консоль в кодировке UTF-8", "9 / mtf -> merge text files"}
        };

        /// <summary>
        /// Данное свойство возвращает массив с данными об операциях.
        /// </summary>
        public static string[,] Commands => s_commandsArray;

        /// <summary>
        /// Данный метод получает от пользователя путь к исходному файлу.
        /// </summary>
        private static void GetSourceFilePath()
        {
            do
            {
                Console.Write("\nУкажите путь к исходному файлу: ");
                s_path = @$"{Console.ReadLine()}";

                if (string.IsNullOrEmpty(s_path))
                    Console.WriteLine("\nВы ничего не ввели! Повторите ввод!");
                else if (!(new FileInfo(@$"{s_path}")).Exists)
                    Console.WriteLine("\nВы указали неправильный путь! Повторите ввод!");

            } while (string.IsNullOrEmpty(s_path) || !(new FileInfo(@$"{s_path}")).Exists);
        }

        /// <summary>
        /// Данный метод получает от пользователя путь директории для нового файла.
        /// </summary>
        /// <returns>Возвращает строку, содержащую новый путь директории (без названия файла).</returns>
        private static string GetDestDirectoryPath()
        {
            string destPath;
            do
            {
                Console.Write("\nУкажите путь к новой директории: ");
                destPath = @$"{Console.ReadLine()}";
                
                if (string.IsNullOrEmpty(destPath))
                    Console.WriteLine("\nВы ничего не ввели! Повторите ввод!");
                else if (!Directory.Exists(destPath))
                    Console.WriteLine("\nДиректории по указанному пути не существует! Повторите ввод!");
                else if (File.Exists(Path.Combine(destPath, new FileInfo(s_path).Name)))
                    Console.WriteLine("\nФайл по указанному адресу уже существует! Повторите ввод!");
                
            } while (string.IsNullOrEmpty(destPath) || !Directory.Exists(destPath) || 
                     File.Exists(Path.Combine(destPath, new FileInfo(s_path).Name)));
            
            return destPath;
        }

        /// <summary>
        /// Данный метод получает от пользователя путь к директории с названием нового файла в поле _path.
        /// </summary>
        /// <returns>Возвращает булево значение об успешном / неуспешном получении данных о пути.</returns>
        private static bool GetNewFilePath()
        {
            Console.Write("\nУкажите путь к директории вместе с названием нового файла: ");
            s_path = @$"{Console.ReadLine()}";
            
            if (string.IsNullOrEmpty(s_path))
            {
                Console.WriteLine("\nВы ничего не ввели! Повторите ввод!");
                return false;
            }
            if (!Directory.Exists(new FileInfo(s_path).DirectoryName))
            {
                Console.WriteLine("\nДиректории по указанному пути не существует! Повторите ввод!");
                return false;
            }
            if (new FileInfo(@$"{s_path}").Exists)
            {
                Console.WriteLine("\nФайл по указанному пути уже существует! Повторите ввод!");
                return false;
            }
            if (new FileInfo(s_path).Extension is not ".txt")
            {
                Console.WriteLine("\nВы указали неверный путь, или вы пытаетесь создать не текстовый файл! " +
                                  "Повторите ввод!");
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Данный метод выбирает диск, который указал пользователь, для дальнейшей работы с ним.
        /// </summary>
        /// <param name="diskDrives">Массив с дисками типа DriveInfo[].</param>
        /// <param name="diskDrivesStrArray">Массив с дисками, названия которых - строки.</param>
        /// <returns>Возвращает булево значение об успешном / неуспешном выборе диска.</returns>
        private static bool ChooseDisk(DriveInfo[] diskDrives, string[] diskDrivesStrArray)
        {
            try
            {
                // Выбор диска для работы.
                Console.Write("\nВыберите диск: ");
                var disk = Console.ReadLine();
                if (!diskDrivesStrArray.Contains(@$"{disk}"))
                {
                    Console.WriteLine("\nДанный диск не существует. Повторите выбор!");
                    return false;
                }
                if (!diskDrives[Array.IndexOf(diskDrivesStrArray, @$"{disk}")].IsReady)
                {
                    Console.WriteLine("\nДиск не готов к использованию. Повторите выбор!");
                    return false;
                }
                Console.WriteLine($"\nВы выбрали диск: {disk}. Он готов к использованию!\n");
                
                // Изменение текущего диска на диск, выбранный пользователем.
                s_path = @$"{disk}";
            }
            catch (Exception exception)
            {
                Console.WriteLine($"\nВозникла ошибка: {exception.Message}\nПовторите ввод!");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Данный метод выбирает директорию, список файлов которой выведется на экран.
        /// </summary>
        private static void ChooseDirectoryToPrintData()
        {
            // Объявление кнопки для выбора места, в котором выведется список файлов (в текущем или по пути).
            ConsoleKeyInfo keyToContinue;
            do
            {
                Console.WriteLine("\nЧтобы узнать список файлов в текущей директории, нажмите \"ENTER\"\nЧтобы " + 
                                  "ввести путь к директории вручную и узнать список ее файлов, нажмите \"BACKSPACE\"");
                keyToContinue = Console.ReadKey();

                if (keyToContinue.Key is not (ConsoleKey.Enter or ConsoleKey.Backspace))
                    Console.WriteLine("\nВы нажали неверную кнопку, повторите действие!");
                else if (keyToContinue.Key == ConsoleKey.Backspace)
                {
                    do
                    {
                        Console.Write("\nУкажите путь к директории, чтобы получить список ее файлов: ");
                        s_path = @$"{Console.ReadLine()}";
                        
                        if (string.IsNullOrEmpty(s_path))
                            Console.WriteLine("\nВы ничего не ввели! Повторите ввод!");
                        else if (!Directory.Exists(@$"{s_path}"))
                            Console.WriteLine("\nВы ввели неправильный путь! Повторите ввод!");
                        
                    } while (string.IsNullOrEmpty(s_path) || !Directory.Exists(@$"{s_path}"));
                }
            } while (keyToContinue.Key is not (ConsoleKey.Enter or ConsoleKey.Backspace));
        }
        
        /// <summary>
        /// Данный метод получает кодировку, с которой пользователь хочет работать. 
        /// </summary>
        /// <returns>Возвращает кодировку файла.</returns>
        private static Encoding GetDestEncoding()
        {
            Encoding destEncoding;
            do
            {
                Console.Write("\nДоступные кодировки:\n1 / UTF-8\n2 / ASCII\n3 / UTF-16\n4 / UTF-32\n" +
                              "5 / Latin1\n\nВаш выбор: ");
                switch (Console.ReadLine())
                {
                    case "1" or "UTF-8":
                        destEncoding = Encoding.UTF8;
                        break;
                    case "2" or "ASCII":
                        destEncoding = Encoding.ASCII;
                        break;
                    case "3" or "UTF-16":
                        destEncoding = Encoding.Unicode;
                        break;
                    case "4" or "UTF-32":
                        destEncoding = Encoding.UTF32;
                        break;
                    case "5" or "Latin1":
                        destEncoding = Encoding.Latin1;
                        break;
                    default:
                        Console.WriteLine("\nВы указали неверную кодировку! Повторите ввод!");
                        continue;
                }
                break;
            } while (true);
            return destEncoding;
        }

        /// <summary>
        /// Данный метод преобразует текст файла в указанную кодировку. 
        /// </summary>
        /// <param name="currentEncoding">Текущая кодировка текста файла.</param>
        /// <param name="destEncoding">Кодировка, в которую необходимо конвертировать текст файла для последующего
        /// вывода.</param>
        /// <returns>Возвращает лист с конвертированными в указанную кодировку строками.</returns>
        private static List<string> ConvertFileTextEncoding(Encoding currentEncoding, Encoding destEncoding)
        {
            // Объявляем переменную, в которую положим конвертированные в нужной кодировке строчки.
            var linesDestList = new List<string>();
            // Читаем строки в потоке, преобразуем их в нужную кодировку и добавляем в лист.
            using (var sr = new StreamReader(s_path, currentEncoding))
            {
                string lineSource;
                while ((lineSource = sr.ReadLine()) != null)
                {
                    var sourceByteArray = currentEncoding.GetBytes(lineSource);
                    var destByteArray = Encoding.Convert(currentEncoding, destEncoding, sourceByteArray);
                    linesDestList.Add(destEncoding.GetString(destByteArray));
                }
            }
            return linesDestList;
        }

        /// <summary>
        /// Данный метод получает от пользователя количество файлов, которые он хочет конкатенировать, и пути к ним. 
        /// </summary>
        /// <returns>Возвращает массив строк - путей ко всем файлам для конкатенации.</returns>
        private static string[] GetFilesPathToMerge()
        {
            int amountFiles;
            do
            {
                Console.Write("\nВведите количество файлов, которые вы хотите конкатенировать (от 1 до 10): ");
                var amountFilesString = Console.ReadLine();
                if (!int.TryParse(amountFilesString, out amountFiles) || amountFiles is (< 1 or > 10) ||
                    amountFilesString.StartsWith(" ") || amountFilesString.StartsWith("0"))
                {
                    Console.WriteLine("\nВы ввели неверное количество файлов! Повторите ввод!");
                    continue;
                }
                break;
            } while (true);
            var filesPathArray = new string[amountFiles];
            for (var i = 0; i < filesPathArray.Length; i++)
            {
                do
                {
                    GetSourceFilePath();
                    if (new FileInfo(s_path).Extension is not ".txt")
                    {
                        Console.WriteLine("\nЭто не текстовый файл! Повторите ввод!");
                    }
                } while (new FileInfo(s_path).Extension is not ".txt");
                filesPathArray[i] = s_path;
            }
            return filesPathArray;
        }

        /// <summary>
        /// Данный метод создает лист со строками каждого файла.
        /// </summary>
        /// <returns>Возвращает кортеж с полученным списком и максимальной длиной файла.</returns>
        private static (List<string[]>, int maxLength) CreateFilesDataList()
        {
            List<string[]> dataList = new();
            var maxLength = 0;
            
            foreach (var filePath in GetFilesPathToMerge())
            {
                var tmpList = File.ReadAllLines(filePath);
                if (tmpList.Length > maxLength)
                    maxLength = tmpList.Length;
                dataList.Add(tmpList);
            }

            return (dataList, maxLength);
        }

        /// <summary>
        /// Данный метод печатает на экран список дисков компьютера и печатает выбранный диск на экран.
        /// </summary>
        private static void PrintDisksList()
        {
            do
            {
                Console.WriteLine("\nСписок доступных дисков:");
                // Извлечение списка дисков на компьютере.
                DriveInfo[] diskDrives = DriveInfo.GetDrives();
                var diskDrivesStrArray = new string[diskDrives.Length];
                for (var i = 0; i < diskDrives.Length; i++)
                {
                    diskDrivesStrArray[i] = diskDrives[i].Name;
                    Console.WriteLine(diskDrivesStrArray[i]);
                }
                // Выбор диска для работы.
                if (!ChooseDisk(diskDrives, diskDrivesStrArray))
                    continue;
                break;
            } while (true);
        }

        /// <summary>
        /// Данный метод выбирает директорию, в которую пользователь хочет попасть.
        /// </summary>
        private static void ChooseDirectory()
        {
            Console.WriteLine($"\nДиректория, в которой вы сейчас находитесь: {s_path}");

            s_path = GetDestDirectoryPath();
            
            Console.WriteLine($"\nТеперь вы находитесь в директории: {s_path}\n");
        }

        /// <summary>
        /// Данный метод выводит на экран список всех файлов в директории.
        /// </summary>
        private static void ShowFilesList()
        {
            // Вывод списка файлов в директории по пути _path.
            do
            {
                Console.WriteLine($"\nДиректория, в которой вы сейчас находитесь: {s_path}");
                // Выбор директории, список файлов которой выведется на экран.
                ChooseDirectoryToPrintData();
                try
                {
                    // Печать списка файлов директории.
                    var tmpCount = 0;
                    foreach (var fileName in Directory.GetFiles(@$"{s_path}"))
                    {
                        if (tmpCount == 0)
                            Console.WriteLine($"\nСписок файлов в директории {s_path} следующий:\n");
                        Console.WriteLine(new FileInfo(fileName).Name);
                        tmpCount++;
                    }
                    if (tmpCount == 0)
                        Console.WriteLine("\nВ данной директории файлов не обнаружено!");
                    Console.WriteLine();
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nВозникла ошибка: {exception.Message}\nПовторите ввод!");
                }
            } while (true);
        }

        /// <summary>
        /// Данный метод выводит на экран текст файла в кодировке: UTF-8 / ASCII / UTF-16 / UTF-32 / Latin1 (ISO-8859-1).
        /// </summary>
        private static void PrintFileData()
        {
            do
            {
                // Получаем исходный путь в поле _path.
                GetSourceFilePath();
                if (new FileInfo(s_path).Extension is not ".txt")
                {
                    Console.WriteLine("\nЭто не текстовый файл! Повторите ввод!");
                    continue;
                }
                Console.WriteLine();
                // Получаем текущую кодировку файла.
                Encoding currentEncoding;
                using (var sr = new StreamReader(s_path))
                    currentEncoding = sr.CurrentEncoding;
                try
                {
                    // Преобразовываем данные из текущей кодировки в указанную кодировку и выводим на экран.
                    Console.Write("Выберите кодировку, в которой вы хотите вывести содержимое текстового файла!\n");
                    Encoding destEncoding = GetDestEncoding();
                    List<string> convertedDataList = ConvertFileTextEncoding(currentEncoding, destEncoding);
                    Console.WriteLine($"\nТекст файла {new FileInfo(s_path).Name} в кодировке " +
                                      $"{destEncoding.WebName} следующий:\n");
                    foreach (string str in convertedDataList)
                        Console.WriteLine(str, Console.OutputEncoding = destEncoding);
                    Console.OutputEncoding = Encoding.Default;
                    Console.WriteLine();
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Файл не удалось прочитать: {exception.Message}\nПовторите попытку!");
                }
            } while (true);
        }
        
        /// <summary>
        /// Данный метод копирует файл из директории по исходному пути в директорию по указанному пути.
        /// </summary>
        private static void CopyFile()
        {
            // Получаем исходный путь в поле _path.
            GetSourceFilePath();
            // Получаем новый путь и копируем файл.
            do
            {
                try
                {
                    string destPath = Path.Combine(GetDestDirectoryPath(), new FileInfo(s_path).Name);
                    File.Copy(s_path, destPath);
                    Console.WriteLine("\n" + @$"Файл {new FileInfo(destPath).Name} успешно скопирован в " +
                                      @$"директорию {new FileInfo(destPath).DirectoryName}" + "\n");
                    break;
                }
                catch (IOException)
                {
                    Console.WriteLine("\nДанная директория доступна только для чтения!\nПовторите ввод!");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nВозникла ошибка: {exception.Message}\nПовторите ввод!");
                }
            } while (true);
        }
        
        /// <summary>
        ///  Данный метод перемещает файл по указанному пути в указанную директорию.
        /// </summary>
        private static void MoveFile()
        {
            // Получаем исходный путь в поле _path.
            GetSourceFilePath();
            // Получаем новый путь и перемещаем файл.
            do
            {
                try
                {
                    string destPath = Path.Combine(GetDestDirectoryPath(), new FileInfo(s_path).Name);
                    File.Move(s_path, destPath);
                    Console.WriteLine("\n" + @$"Файл {new FileInfo(destPath).Name} успешно перемещен в " +
                                      @$"директорию {new FileInfo(destPath).DirectoryName}" + "\n");
                    break;
                }
                catch (IOException)
                {
                    Console.WriteLine("\nДанная директория доступна только для чтения!\nПовторите ввод!");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nВозникла ошибка: {exception.Message}\nПовторите ввод!");
                }
            } while (true);
        }

        /// <summary>
        /// Данный метод удаляет файл по указанному пути.
        /// </summary>
        private static void RemoveFile()
        {
            // Получаем исходный путь в поле _path.
            GetSourceFilePath();
            // Удаляем файл.
            do
            {
                try
                {
                    File.Delete(s_path);
                    Console.WriteLine($"\nФайл {new FileInfo(s_path).Name} успешно удален!\n");
                    break;
                }
                catch (IOException)
                {
                    Console.WriteLine($"Файл {new FileInfo(s_path).Name} используется системой! Закройте его и " +
                                      "повторите попытку!");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nВозникла ошибка: {exception.Message}\nПовторите ввод!");
                }
            } while (true);
        }
        
        /// <summary>
        /// Данный метод создает новый текстовый файл в кодировке: UTF-8 / ASCII / UTF-16 / UTF-32 / Latin1 (ISO-8859-1).
        /// </summary>
        private static void CreateTextFile()
        {
            do
            {
                try
                {
                    // Получаем исходный путь в поле _path.
                    if (!GetNewFilePath())
                        continue;
                    // Получаем кодировку для нового файла.
                    Console.Write("\nВыберите кодировку, в которой вы хотите создать новый текстовый " +
                                   $"файл {new FileInfo(s_path).Name}!\n");
                    Encoding destEncoding = GetDestEncoding();
                    // Создаем новый файл в кодировке, указанной пользователем.
                    using (var sw = new StreamWriter(s_path, false, destEncoding))
                        sw.WriteLine($"This is example text. File has {destEncoding.WebName} encoding!");
                    Console.WriteLine($"\nФайл {new FileInfo(s_path).Name} успешно создан в " +
                                      $"кодировке {destEncoding.WebName}!\n");
                    break;
                }
                catch (IOException)
                {
                    Console.WriteLine("\nДиректория, в которой вы хотите создать файл, доступна только для чтения!\n" +
                                      "Повторите ввод!");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nВозникла ошибка: {exception.Message}\nПовторите ввод!");
                }
            } while (true);
        }
        
        /// <summary>
        /// Данный метод делает конкатенацию содержимого двух или более (до 10) текстовых файлов и выводит на экран
        /// результат в кодировке UTF-8.
        /// </summary>
        private static void MergeTextFiles()
        {
            do
            {
                try
                {
                    // Создаем список с массивами строк каждого файла.
                    (List<string[]> dataList, int maxLength) = CreateFilesDataList();
                    // Вывод данных на экран.
                    Console.WriteLine("\nДанные, полученные после конкатенации:\n");
                    Console.OutputEncoding = Encoding.UTF8;
                    for (var line = 0; line < maxLength; line++)
                    {
                        foreach (var array in dataList)
                        {
                            if (line >= array.Length)
                                Console.Write("" + " ");
                            else
                                Console.Write(array[line] + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    Console.OutputEncoding = Encoding.Default;
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nВозникла ошибка: {exception.Message}\nПовторите ввод!");
                }
            } while (true);
        }
        
        /// <summary>
        /// Данный метод выполняет выбранную пользователем операцию.
        /// </summary>
        public void DoOperation()
        {
            if (_operation is "1" or "pdl")
                PrintDisksList();
            else if (_operation is "2" or "csd")
                ChooseDirectory();
            else if (_operation is "3" or "sfl")
                ShowFilesList();
            else if (_operation is "4" or "pfd")
                PrintFileData();
            else if (_operation is "5" or "cpf")
                CopyFile();
            else if (_operation is "6" or "mvf")
                MoveFile();
            else if (_operation is "7" or "rmf")
                RemoveFile();
            else if (_operation is "8" or "ctf")
                CreateTextFile();
            else if (_operation is "9" or "mtf")
                MergeTextFiles();
        }
    }
}