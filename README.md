# Текстовий редактор із підтримкою Markdown
**Ліневич Денис ІПЗ-23-1**

## Запуск проекту
В папці з проектом виконайте команду ```dotnet run```

## Функціональність
- Головне вікно редактора
  - Дві вкладки: [Edit](./TextEditor/UserControls/EditUserControl.xaml) та [Preview](./TextEditor/UserControls/PreviewUserControl.xaml)
  - Бокова панель
  - Історія відкритих файлів
- Основні функції
  - Створення нового файлу
  - Відкриття наявного файлу
  - Створення жирного та курсивного тексту
  - Шість рівнів заголовків
  - Створення списків
  - Операція Undo
  - Збереження файлу як .txt, .md та .pdf
  - Пошук тексту у Preview
  - Зміна шрифту у Preview
  - Зміна типів перегляду у Preview
- Додатково
  - Гарячі клавіші:
    - ```Ctrl + B``` - зробити виділений текст жирним
    - ```Ctrl + I``` - зробити виділений текст курсивним
    - ```Ctrl + Z``` - відмінити попередню дію
  - Кнопки для виконання наступних дій:
    - Зробити жирним
    - Зробити курсвиним
    - Зробити заголовком
    - Зробити елементом списку
    - Відмінити попередню дію
  - Історія:
    - Перегляд попередньо відкритих файлів
    - Завантаження контенту цих файлів при кліку
  - Приховування бокової панелі
## Programming Principles
### DRY
Щоб уникати повторень коду, я інкапсулював логіку, що використовується багато разів, в окремі методи та класи.

Наприклад: 
- TryParseInlineElement класу [MarkdownParser](./TextEditor/Models/Parser/MarkdownParser.cs) допомагає створювати різні inline елементи.
- За допомогою патерну Command, логіка редагування тексту була розділена на окремі команди, як-от [BoldTextCommand](./TextEditor/Models/Commands/BoldTextCommand.cs) чи [ItalicTextCommand](./TextEditor/Models/Commands/ItalicTextCommand.cs).
### KISS
Написаний код є доволі простим і легким для сприйняття.
Наприклад:
- Класс [Command](./TextEditor/Models/Commands/Command.cs) та його нащадки([BoldTextCommand](./TextEditor/Models/Commands/BoldTextCommand.cs), [InsertOnStartCommand](./TextEditor/Models/Commands/InsertOnStartCommand.cs), [EditTextCommand](./TextEditor/Models/Commands/EditTextCommand.cs) тощо) допомагає чітко роподіляти відповідальності.
Самі команди мають простий алгоритм, що редагує текст.
- Призначення полів, методів і класів зрозумілі з їхніх назв ([FileManager](./TextEditor/Models/FileManager/FileManager.cs), [MarkdowParser](./TextEditor/Models/Parser/MarkdownParser.cs), [FlowDocToPdfParser](./TextEditor/Models/FileManager/Helpers/FlowDocToPdfParser.cs) і тд.)
### SOLID
#### Single Responsibility Principle 
Кожен клас має єдину мету:
- [MarkdownParser](./TextEditor/Models/Parser/MarkdownParser.cs): Відповідає лише за парсинг розмітки у об'єкт FlowDocument.
- [TextSaveStrategy](./TextEditor/Models/FileManager/TextSaveStrategy.cs) та [PdfSaveStrategy](./TextEditor/Models/FileManager/PdfSaveStrategy.cs): Кожен клас відповідає за збереження файлу у відповідному форматі (текстовому або PDF).
- BoldTextCommand, ItalicTextCommand, InsertOnStartCommand: Кожна команда виконує одну конкретну дію (додавання жирного, курсиву або символу на початку).
#### Open/Closed Principle 
Структура проекту дозволяє легко розширювати функціональність без змін існуючого коду.
Наприклад:
- Якщо з'явиться потреба в зберіганні файлу в іншому форматі, потрібно створити новий клас, що наслідується від [ISaveStrategy](./TextEditor/Models/FileManager/ISaveStrategy.cs), та передати цю стратегію об'єкту FileManager.
#### Liskov Substitution Principle 
Об'єкти підкласів можуть замінювати об'єкти базового класу без порушення роботи програми.

Нариклад:
-	Command та його похідні класи (BoldTextCommand, ItalicTextCommand) можуть використовуватися взаємозамінно в [EditUserControl](./TextEditor/UserControls/EditUserControl.xaml.cs).
#### Interface Segregation Principle 
Малі неперенавантажені класи є забезпеченням того, що класи реалізовують лише релевантну поведінку. 
Наприклад:
-	Інтерфейс ISaveStrategy містить лише метод Save, який потрібен для реалізації стратегій збереження.
#### Dependency Inversion Principle 
-	[FileManager](./TextEditor/Models/FileManager/FileManager.cs) залежить від абстракції [ISaveStrategy](./TextEditor/Models/FileManager/ISaveStrategy.cs), а не від конкретних реалізацій ([TextSaveStrategy](./TextEditor/Models/FileManager/TextSaveStrategy.cs), [PdfSaveStrategy](./TextEditor/Models/FileManager/PdfSaveStrategy.cs)).
### Composition Over Inheritance
У своєму коді я намагався компонувати об'єкти, спираючись на менші, повторно використовувані частини, замість створення глибоких дерев успадкування. Наприклад, FileManager використовує композицію через інтерфейс ISaveStrategy, що дозволяє динамічно змінювати стратегію збереження (TextSaveStrategy або PdfSaveStrategy).
### Program to Interfaces not Implementations
