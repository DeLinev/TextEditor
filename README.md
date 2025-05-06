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
Програма більше залежить від абстракцій ніж від конкретних класів. Клас [MainWindow](./TextEditor/MainWindow.xaml.cs) залежить від інтерфейсу [IUserControlFactory](./TextEditor/UserControls/UserControlFactory.cs#L7-L10), що дозволяє замінити реалізацію фабрики (UserControlFactory) на іншу без змін у MainWindow.
### Fail Fast
Написаний код перевіряє вхідні дані та стан об'єктів на ранніх етапах і відповідно реагує, якщо щось пішло не так. Наприклад, при збереженні поточного файлу програма [перевіряє чи користувач обрав шлях для збереження файлу](./TextEditor/UserControls/EditUserControl.xaml.cs#L156-L157) та чи [файл було успішно збережено](./TextEditor/UserControls/EditUserControl.xaml.cs#L160-L170).  
Це дозволяє уникнути накопичення помилок і забезпечує передбачувану поведінку програми.
## Design Patterns
### Command
Цей патерн перетворює запит на окремий об'єкт. Таке перетворення дозволяє передавати запити як аргументи методу, затримувати виконання запиту або ставити його в чергу, а також підтримувати undo-операції. 

У програмі цей патерн реалізовано через абстрактний клас [Command](./TextEditor/Models/Commands/Command.cs), від якого наслідуються конкретні команди:
- [BoldTextCommand](./TextEditor/Models/Commands/BoldTextCommand.cs)
- [ItalicTextCommand](./TextEditor/Models/Commands/ItalicTextCommand.cs)
- [InsertOnStartCommand](./TextEditor/Models/Commands/InsertOnStartCommand.cs)
- [EditTextCommand](./TextEditor/Models/Commands/EditTextCommand.cs)

Причини використання:
- Інкапсуляція змін у документі. Команди (BoldTextCommand, ItalicTextCommand тощо) відповідають за окремі дії, які виконуються над текстом. Весь код для виконання і скасування зосереджений у класі-команді.

- Можливість скасування дії. Завдяки стеку [UndoStack](./TextEditor/UserControls/EditUserControl.xaml.cs#L34) виконані команди зберігаються в історії. За потреби, програма викликає метод Undo() останньої команди, дозволяючи повернути стан тексту назад.

- Гнучкість та масштабованість. Додавання нової команди (наприклад, для вставки зображення чи посилання) потребує лише створення нового класу-нащадка Command.
### Strategy
Цей патерн дозволяє визначити сімейство алгоритмів, помістити кожен з них в окремий клас і зробити їх об'єкти взаємозамінними.

Де використано:
- Інтерфейс [ISaveStrategy](./TextEditor/Models/FileManager/ISaveStrategy.cs) описує метод Save().
- Конкретні реалізації:
  - [TextSaveStrategy](./TextEditor/Models/FileManager/TextSaveStrategy.cs) — для збереження докунмента як .txt чи .md.
  - [PdfSaveStrategy](./TextEditor/Models/FileManager/PdfSaveStrategy.cs) — для збереження документа у форматі PDF.
- Клас FileManager — контекст, який містить посилання на об’єкт ISaveStrategy і делегує йому операцію збереження.

Причини використання:
- Розділення логіки збереження. Замість того, щоб у FileManager реалізовувати всі способи збереження, кожен формат винесено в окрему стратегію.
- Можливість легкого розширення. Якщо потрібно додати збереження у новому форматі, як-от .docx або .html, достатньо реалізувати нову стратегію, не змінюючи існуючий код FileManager.
- Дотримання принципів SOLID:
  - Open/Closed Principle: нові способи збереження додаються без змін старого коду.
  - Single Responsibility Principle: FileManager відповідає лише за делегування, а за сам процес збереження відповідають стратегії за стратегії.

### Simple Factory
Це патерн, який інкапсулює логіку створення об'єктів в одному місці, надаючи клієнту лише готові об'єкти без необхідності знати їх конкретні типи чи логіку створення.

Де використано:
- Інтерфейс [IUserControlFactory](./TextEditor/UserControls/UserControlFactory.cs#L7-L10) визначає метод CreateUserControl().
- Клас [UserControlFactory](./TextEditor/UserControls/UserControlFactory.cs#L12-L33) реалізує цю фабрику:
  - Створює EditUserControl або PreviewUserControl на основі enum [UserControlTypes](./TextEditor/UserControls/UserControlTypes.cs).
- У MainWindow фабрика викликається для динамічного [створення UI-компонентів](./TextEditor/MainWindow.xaml.cs#L69-L78) без прив’язки до їх конкретного типу.

Причини використання:

- Ізоляція логіки створення об’єктів. MainWindow не створює UserControl напряму — це робить UserControlFactory.
- Підтримка принципів SOLID:
  - Single Responsibility: створення UserControl зосереджено в окремому класі.
  - Open/Closed: додати новий тип UserControl можна, не змінюючи логіку в MainWindow.

## Refactoring Techniques
### Extract Method
Щоразу як в коді з'являвся фрагмент, який можна згрупувати та використовувати багато разів, він виносився в окремий новий метод. Старий код замінявся викликом методу.
Наприклад, клас [MarkdownParser](./TextEditor/Models/Parser/MarkdownParser.cs) складається з багатьох допоміжних методів, що опираються один на одного, як-от [ParseInlineMarkdown](./TextEditor/Models/Parser/MarkdownParser.cs#L149-L183), що викликає метод TryParseInlineElement декілька разів, щоб знайти елементи, що підпадають під регулярний вираз.
### Decompose Conditional
Коли в коді з'являлася складна умова в if-else, вона виокремлювалася в окремий метод. Наприклад клас [InsertOnStartCommand](./TextEditor/Models/Commands/InsertOnStartCommand.cs) мав складну умову в методі Execute, яка перевіряла чи слід вставляти на початок рядка ```symbol.ToString() + " "``` чи лише сам символ. Ця умова була винесена в окремий метод [InsertOneOrTwoSymbols](./TextEditor/Models/Commands/InsertOnStartCommand.cs#L43-L48), що збільшує читабельність коду
### Extract Class
Коли один клас виконує роботу двох, його слід розділити та створити новий клас і розмістити в ньому поля і методи, що відповідають за певну функціональність. Коли я створював клас [PdfSaveStrategy](./TextEditor/Models/FileManager/PdfSaveStrategy.cs), що відповідає за збереження файлу у .pdf форматі, я зрозумів, що він виконує зайву роботу, тому створив клас [FlowDocToPdfParser](./TextEditor/Models/FileManager/Helpers/FlowDocToPdfParser.cs), що парсить об'єкт FlowDocument у MigraDoc.DocumentObjectModel.Document.
### Extract Superclass
Коли є два чи більше класів зі спільними полями та методами слід створити для них спільний суперклас і перенесіть до нього всі однакові поля та методи. Базовий клас Command має поля [_editTextBox та _document](./TextEditor/Models/Commands/Command.cs#L7-L8), котрі є спільними для всіх команд (BoldTextCommand, ItalicTextCommand тощо)
### Replace Magic Literals with Symbolic Constant
Якщо у коді використовується число або рядок, які мають певне значення, слід замінити це число на константу, яка має зрозумілу назву, що пояснює значення літерала. Клас [PdfSaveStrategy](./TextEditor/Models/FileManager/PdfSaveStrategy.cs#L12-L13) та [MarkdownParser](./TextEditor/Models/Parser/MarkdownParser.cs#L9-L14) мають константи, що використовуються у методах класу. Це підвищує читабельність коду та дає змогу швидко змінити значення константи.
