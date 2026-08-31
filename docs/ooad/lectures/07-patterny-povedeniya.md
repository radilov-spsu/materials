# ООАП. Лекция 7 — Паттерны поведения

> Конспект лектора. Источник — «Приёмы объектно-ориентированного проектирования» (GoF):
> назначение, применимость, участники — по книге. Код — **C#**, диаграммы — **Mermaid**.
> Паттернов десять, поэтому темп плотный: на каждый — задача, структура, код, плата.

---

## Блок 1. Что общего у паттернов поведения

Паттерны поведения связаны с **алгоритмами и распределением обязанностей** между объектами.
Они описывают не столько структуру классов, сколько **схемы связей** и потоки запросов.

Как и раньше, есть уровень класса (наследование: шаблонный метод, интерпретатор) и уровень
объекта (композиция и делегирование: все остальные).

Что варьирует каждый — по таблице переменных аспектов из книги:

| Паттерн | Что можно менять, не трогая клиента |
|---|---|
| Стратегия | алгоритм |
| Шаблонный метод | шаги алгоритма |
| Наблюдатель | множество зависящих объектов и способ их обновления |
| Команда | время и способ выполнения запроса |
| Состояние | состояние объекта |
| Цепочка обязанностей | объект, выполняющий запрос |
| Итератор | способ перебора элементов агрегата |
| Посредник | взаимодействующие объекты и механизм их совместной работы |
| Хранитель | закрытая информация вне объекта и время её сохранения |
| Посетитель | операции над объектами без изменения их классов |

Общая нить: **инкапсулировать то, что меняется**, и заменить жёсткие связи косвенностью.

---

## Блок 2. Strategy и Template Method

Разбираем парой: они решают одну задачу — «варьировать алгоритм» — но один делает это
композицией, другой наследованием.

### Strategy — стратегия

**Назначение.** Определяет семейство алгоритмов, инкапсулирует каждый из них и делает их
взаимозаменяемыми. Позволяет изменять алгоритм независимо от клиентов, которые им
пользуются.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction LR
        class Composition {
            -ICompositor compositor
            +Repair() void
        }
        class ICompositor {
            <<interface>>
            +Compose(Coord[] natural, int lineWidth) int
        }
        class SimpleCompositor
        class TeXCompositor
        class ArrayCompositor
        Composition o-- ICompositor : стратегия
        ICompositor <|.. SimpleCompositor
        ICompositor <|.. TeXCompositor
        ICompositor <|.. ArrayCompositor
    ```

=== "Исходник"

    ```
    classDiagram
        direction LR
        class Composition {
            -ICompositor compositor
            +Repair() void
        }
        class ICompositor {
            <<interface>>
            +Compose(Coord[] natural, int lineWidth) int
        }
        class SimpleCompositor
        class TeXCompositor
        class ArrayCompositor
        Composition o-- ICompositor : стратегия
        ICompositor <|.. SimpleCompositor
        ICompositor <|.. TeXCompositor
        ICompositor <|.. ArrayCompositor
    ```


=== "C#"

    ```csharp
    public interface IShippingCost { Money Calculate(Order order); }

    public sealed class FlatRate : IShippingCost
    {
        public Money Calculate(Order order) => Money.Of(5, "EUR");
    }

    public sealed class ByWeight : IShippingCost
    {
        public Money Calculate(Order order) => Money.Of(order.Weight * 0.8m, "EUR");
    }

    public sealed class Checkout                            // контекст
    {
        private readonly IShippingCost _shipping;
        public Checkout(IShippingCost shipping) => _shipping = shipping;   // алгоритм извне

        public Money Total(Order order) => order.Total() + _shipping.Calculate(order);
    }
    ```

=== "Python"

    ```python
    from typing import Callable

    ShippingCost = Callable[[Order], Money]        # стратегия — просто вызываемое

    def flat_rate(order: Order) -> Money:  return Money(5, "EUR")
    def by_weight(order: Order) -> Money:  return Money(order.weight * 0.8, "EUR")

    class Checkout:
        def __init__(self, shipping: ShippingCost) -> None:
            self._shipping = shipping              # функция вместо объекта-стратегии

        def total(self, order: Order) -> Money:
            return order.total() + self._shipping(order)
    ```

**Применимость.** Много родственных классов отличаются только поведением; нужны разные
варианты алгоритма; алгоритм использует данные, о которых клиенту знать не нужно; в классе
много условных операторов, выбирающих поведение — их место в отдельных стратегиях.

**Результаты.** Семейство алгоритмов, альтернатива наследованию, отказ от условных
операторов. ⚠️ Клиент должен знать о различиях стратегий, чтобы выбрать нужную; растёт
число объектов; накладные расходы на обмен данными между контекстом и стратегией.

> **В C#.** Часто стратегия схлопывается в делегат: `Func<Order, Money>` вместо интерфейса
> с одним методом. Это законно и короче — но теряется имя типа, а вместе с ним и удобство
> регистрации в DI. Правило: одна операция без состояния — делегат; несколько связанных
> операций или нужна конфигурация — интерфейс.

### Template Method — шаблонный метод

**Назначение.** Определяет скелет алгоритма, перекладывая ответственность за некоторые его
шаги на подклассы. Позволяет переопределять шаги, не меняя структуру алгоритма.

=== "Диаграмма"

    ```mermaid
    classDiagram
        class ReportGenerator {
            <<abstract>>
            +Generate() Report
            #LoadData()* DataSet
            #Format(DataSet d)* string
            #OnFinished() void
        }
        class SalesReport
        class StockReport
        ReportGenerator <|-- SalesReport
        ReportGenerator <|-- StockReport
        note for ReportGenerator "Generate() — шаблонный метод:<br/>порядок шагов фиксирован"
    ```

=== "Исходник"

    ```
    classDiagram
        class ReportGenerator {
            <<abstract>>
            +Generate() Report
            #LoadData()* DataSet
            #Format(DataSet d)* string
            #OnFinished() void
        }
        class SalesReport
        class StockReport
        ReportGenerator <|-- SalesReport
        ReportGenerator <|-- StockReport
        note for ReportGenerator "Generate() — шаблонный метод:<br/>порядок шагов фиксирован"
    ```


=== "C#"

    ```csharp
    public abstract class ReportGenerator
    {
        public Report Generate()                            // шаблонный метод: не виртуальный
        {
            var data = LoadData();                          // обязательный шаг подкласса
            var body = Format(data);
            OnFinished();                                   // hook: по умолчанию ничего
            return new Report(body);
        }

        protected abstract DataSet LoadData();
        protected abstract string Format(DataSet data);
        protected virtual void OnFinished() { }             // операция-зацепка
    }
    ```

=== "Python"

    ```python
    from abc import ABC, abstractmethod

    class ReportGenerator(ABC):
        def generate(self) -> Report:              # шаблонный метод
            data = self.load_data()
            body = self.format(data)
            self.on_finished()
            return Report(body)

        @abstractmethod
        def load_data(self) -> DataSet: ...
        @abstractmethod
        def format(self, data: DataSet) -> str: ...
        def on_finished(self) -> None: ...         # зацепка с пустой реализацией
    ```

**Ключевая идея — «голливудский принцип»**: не звоните нам, мы позвоним вам. Базовый класс
зовёт методы подкласса, а не наоборот. Именно так устроены каркасы (лекция 1).

**Стратегия против шаблонного метода.** Один вопрос: **когда** фиксируется выбор.
Наследование — на этапе компиляции, один вариант на класс; композиция — в рантайме,
можно менять и комбинировать. Плюс шаблонный метод позволяет переиспользовать **структуру**
алгоритма, а стратегия — заменить его целиком.

---

## Блок 3. Observer — наблюдатель

**Назначение.** Определяет зависимость типа «один-ко-многим» между объектами так, что при
изменении состояния одного объекта все зависящие от него получают уведомление
и автоматически обновляются.

**Задача.** Пример книги — таблица, столбчатая и круговая диаграммы поверх одних данных:
изменили данные — перерисовались все представления, при этом данные о представлениях
ничего не знают.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction LR
        class ISubject {
            <<interface>>
            +Attach(IObserver o) void
            +Detach(IObserver o) void
            +Notify() void
        }
        class IObserver {
            <<interface>>
            +Update(ISubject subject) void
        }
        class SalesData {
            -List~IObserver~ observers
            +Add(Sale s) void
            +GetState() Report
        }
        class BarChart
        class PieChart
        ISubject <|.. SalesData
        IObserver <|.. BarChart
        IObserver <|.. PieChart
        SalesData o-- IObserver : наблюдатели
        BarChart --> SalesData : запрашивает состояние
    ```

=== "Исходник"

    ```
    classDiagram
        direction LR
        class ISubject {
            <<interface>>
            +Attach(IObserver o) void
            +Detach(IObserver o) void
            +Notify() void
        }
        class IObserver {
            <<interface>>
            +Update(ISubject subject) void
        }
        class SalesData {
            -List~IObserver~ observers
            +Add(Sale s) void
            +GetState() Report
        }
        class BarChart
        class PieChart
        ISubject <|.. SalesData
        IObserver <|.. BarChart
        IObserver <|.. PieChart
        SalesData o-- IObserver : наблюдатели
        BarChart --> SalesData : запрашивает состояние
    ```


=== "C#"

    ```csharp
    public sealed class SalesData                            // субъект
    {
        private readonly List<IObserver> _observers = new();
        private readonly List<Sale> _sales = new();

        public void Attach(IObserver o) => _observers.Add(o);
        public void Detach(IObserver o) => _observers.Remove(o);

        public void Add(Sale sale)
        {
            _sales.Add(sale);
            Notify();                                        // изменение состояния → уведомление
        }

        private void Notify()
        {
            foreach (var o in _observers.ToArray()) o.Update(this);   // копия: список могут менять в Update
        }
    }
    ```

=== "Python"

    ```python
    class SalesData:
        def __init__(self) -> None:
            self._observers: list[Callable[["SalesData"], None]] = []
            self._sales: list[Sale] = []

        def attach(self, observer) -> None: self._observers.append(observer)
        def detach(self, observer) -> None: self._observers.remove(observer)

        def add(self, sale: Sale) -> None:
            self._sales.append(sale)
            for observer in list(self._observers):   # копия: подписчики могут отписаться
                observer(self)                       # наблюдатель — любое вызываемое
    ```

**Две модели передачи данных (по книге).**

- **Проталкивание (push)** — субъект шлёт подробности изменения в `Update`. Быстрее, но
  субъект делает предположения о том, что нужно наблюдателям.
- **Вытягивание (pull)** — субъект шлёт только «я изменился», наблюдатель сам запрашивает
  состояние. Гибче, но может потребовать нескольких обращений.

**Результаты.** Абстрактная связанность субъекта и наблюдателя, широковещательные
уведомления. ⚠️ Опасность: **неожиданные каскадные обновления**. Наблюдатели не знают друг
о друге, поэтому одно изменение может вызвать лавину обновлений, а цепочку трудно
проследить. Плюс классическая утечка: забыли `Detach` — объект живёт вечно.

> **В .NET.** События (`event`), `IObservable<T>`/`IObserver<T>` и Rx, `INotifyPropertyChanged`
> из MVVM (лекция 2) — всё это наблюдатель. Слабые события (`WeakEventManager`) существуют
> ровно из-за проблемы с `Detach`.

---

## Блок 4. Command — команда

**Назначение.** Инкапсулирует запрос в виде объекта, позволяя параметризовать клиентов
типом запроса, ставить запросы в очередь, протоколировать их и поддерживать отмену операций.

**Задача.** Пример книги — меню и кнопки редактора: пункт меню не должен знать, что именно
он делает. Он знает объект-команду, у которой есть `Execute`.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction TB
        class ICommand {
            <<interface>>
            +Execute() void
            +Unexecute() void
        }
        class PasteCommand {
            -Document document
            +Execute() void
            +Unexecute() void
        }
        class OpenCommand
        class MenuItem {
            -ICommand command
            +Clicked() void
        }
        class History {
            -Stack~ICommand~ done
            +Push(ICommand c) void
            +Undo() void
        }
        ICommand <|.. PasteCommand
        ICommand <|.. OpenCommand
        MenuItem o-- ICommand
        History o-- ICommand : журнал для отмены
    ```

=== "Исходник"

    ```
    classDiagram
        direction TB
        class ICommand {
            <<interface>>
            +Execute() void
            +Unexecute() void
        }
        class PasteCommand {
            -Document document
            +Execute() void
            +Unexecute() void
        }
        class OpenCommand
        class MenuItem {
            -ICommand command
            +Clicked() void
        }
        class History {
            -Stack~ICommand~ done
            +Push(ICommand c) void
            +Undo() void
        }
        ICommand <|.. PasteCommand
        ICommand <|.. OpenCommand
        MenuItem o-- ICommand
        History o-- ICommand : журнал для отмены
    ```


=== "C#"

    ```csharp
    public interface ICommand
    {
        void Execute();
        void Unexecute();
    }

    public sealed class PasteCommand : ICommand
    {
        private readonly Document _document;
        private readonly string _text;
        private int _position;

        public PasteCommand(Document document, string text) => (_document, _text) = (document, text);

        public void Execute()
        {
            _position = _document.CaretPosition;             // запоминаем для отмены
            _document.Insert(_position, _text);
        }

        public void Unexecute() => _document.Delete(_position, _text.Length);
    }

    public sealed class History                              // журнал команд
    {
        private readonly Stack<ICommand> _done = new();
        public void Run(ICommand c) { c.Execute(); _done.Push(c); }
        public void Undo() { if (_done.Count > 0) _done.Pop().Unexecute(); }
    }
    ```

=== "Python"

    ```python
    from dataclasses import dataclass, field

    @dataclass
    class PasteCommand:                       # протокол: execute / unexecute
        document: Document
        text: str
        position: int = field(default=0, init=False)

        def execute(self) -> None:
            self.position = self.document.caret_position
            self.document.insert(self.position, self.text)

        def unexecute(self) -> None:
            self.document.delete(self.position, len(self.text))

    class History:
        def __init__(self) -> None: self._done: list = []
        def run(self, command) -> None: command.execute(); self._done.append(command)
        def undo(self) -> None:
            if self._done: self._done.pop().unexecute()
    ```

**Применимость.** Параметризовать объекты выполняемым действием; определять, ставить
в очередь и выполнять запросы в разное время; поддерживать отмену; протоколировать
изменения для восстановления после сбоя; строить систему на высокоуровневых операциях
из примитивных (транзакции).

**Результаты.** Отделяет инициатора от исполнителя, делает команды объектами первого
класса (их можно хранить, передавать, комбинировать в макрокоманды через компоновщик).
⚠️ Число классов растёт; для отмены нужно хранить состояние, а для многоуровневой отмены —
следить, чтобы команды не накапливали ошибок при повторном выполнении.

> **В .NET.** `ICommand` в MVVM — этот же паттерн, встроенный в платформу. MediatR,
> очереди сообщений, `Action`-делегаты как простейшие команды. Отмена в редакторах,
> миграции БД (`Up`/`Down`) — команда с обратной операцией.

---

## Блок 5. State — состояние

**Назначение.** Позволяет объекту изменять поведение при изменении внутреннего состояния.
Со стороны выглядит так, будто объект поменял свой класс.

**Задача.** Пример книги — TCP-соединение: `Open`, `Close`, `Acknowledge` работают
по-разному в состояниях Established, Listen, Closed. Без паттерна это гигантские `switch`
в каждом методе.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction LR
        class TcpConnection {
            -TcpState state
            +Open() void
            +Close() void
            +Acknowledge() void
        }
        class TcpState {
            <<abstract>>
            +Open(TcpConnection c) void
            +Close(TcpConnection c) void
        }
        class TcpEstablished
        class TcpListen
        class TcpClosed
        TcpConnection o-- TcpState : текущее состояние
        TcpState <|-- TcpEstablished
        TcpState <|-- TcpListen
        TcpState <|-- TcpClosed
    ```

=== "Исходник"

    ```
    classDiagram
        direction LR
        class TcpConnection {
            -TcpState state
            +Open() void
            +Close() void
            +Acknowledge() void
        }
        class TcpState {
            <<abstract>>
            +Open(TcpConnection c) void
            +Close(TcpConnection c) void
        }
        class TcpEstablished
        class TcpListen
        class TcpClosed
        TcpConnection o-- TcpState : текущее состояние
        TcpState <|-- TcpEstablished
        TcpState <|-- TcpListen
        TcpState <|-- TcpClosed
    ```


```csharp
public sealed class TcpConnection
{
    private TcpState _state = new TcpClosed();

    internal void ChangeState(TcpState next) => _state = next;

    public void Open() => _state.Open(this);            // делегирование состоянию
    public void Close() => _state.Close(this);
}

public abstract class TcpState
{
    public virtual void Open(TcpConnection c) => throw new InvalidOperationException();
    public virtual void Close(TcpConnection c) => throw new InvalidOperationException();
}

public sealed class TcpClosed : TcpState
{
    public override void Open(TcpConnection c) => c.ChangeState(new TcpEstablished());
}
```

**Результаты.** Локализует поведение состояния в одном классе; делает переходы явными
(состояние меняется присваиванием объекта, а не набором флагов); объекты состояний можно
разделять, если у них нет собственных данных. ⚠️ Растёт число классов; надо решить, **кто
определяет переходы** — контекст или сами состояния (второе гибче, но состояния начинают
знать друг о друге).

**Состояние против стратегии.** Диаграммы одинаковые, намерение разное: стратегию выбирает
**клиент** и обычно один раз, состояние меняется **само** по ходу жизни объекта. Связь
с лекцией 4 прямая: диаграмма состояний — это проект, паттерн State — одна из трёх
её реализаций.

---

## Блок 6. Chain of Responsibility — цепочка обязанностей

**Назначение.** Позволяет избежать жёсткой привязки отправителя запроса к получателю,
давая шанс обработать запрос нескольким объектам. Получатели связываются в цепочку,
и запрос идёт по ней, пока кто-нибудь его не обработает.

**Задача.** Пример книги — контекстная справка: кнопка не знает справки, передаёт запрос
диалогу, тот — приложению. Обработает тот, у кого есть ответ.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction LR
        class Handler {
            <<abstract>>
            #Handler successor
            +HandleRequest(Request r) void
        }
        class Button
        class Dialog
        class Application
        Handler <|-- Button
        Handler <|-- Dialog
        Handler <|-- Application
        Handler o-- Handler : преемник
    ```

=== "Исходник"

    ```
    classDiagram
        direction LR
        class Handler {
            <<abstract>>
            #Handler successor
            +HandleRequest(Request r) void
        }
        class Button
        class Dialog
        class Application
        Handler <|-- Button
        Handler <|-- Dialog
        Handler <|-- Application
        Handler o-- Handler : преемник
    ```


```csharp
public abstract class Handler
{
    private readonly Handler? _successor;
    protected Handler(Handler? successor = null) => _successor = successor;

    public virtual void Handle(Request request) => _successor?.Handle(request);
    // если не обработал — передал дальше; в конце цепочки запрос может остаться без ответа
}

public sealed class ValidationHandler : Handler
{
    public ValidationHandler(Handler? next) : base(next) { }

    public override void Handle(Request request)
    {
        if (!request.IsValid) { request.Reject("Некорректные данные"); return; }
        base.Handle(request);
    }
}
```

**Результаты.** Ослабляет связанность: ни отправитель, ни получатели не знают друг о друге;
цепочку можно менять в рантайме. ⚠️ **Получение не гарантировано** — запрос может дойти
до конца цепочки и остаться без обработки, и это надо предусматривать явно.

> **В .NET.** Конвейер middleware в ASP.NET Core — цепочка обязанностей в чистом виде;
> `DelegatingHandler` в `HttpClient`; обработка исключений по стеку вызовов — та же идея
> на уровне языка.

---

## Блок 7. Iterator — итератор

**Назначение.** Даёт способ последовательно обойти элементы составного объекта, не раскрывая
его внутреннего представления.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction LR
        class IAggregate~T~ {
            <<interface>>
            +CreateIterator() IIterator~T~
        }
        class IIterator~T~ {
            <<interface>>
            +First() void
            +Next() void
            +IsDone() bool
            +CurrentItem() T
        }
        class ListAggregate~T~
        class ListIterator~T~
        IAggregate~T~ <|.. ListAggregate~T~
        IIterator~T~ <|.. ListIterator~T~
        ListAggregate~T~ ..> ListIterator~T~ : создаёт
    ```

=== "Исходник"

    ```
    classDiagram
        direction LR
        class IAggregate~T~ {
            <<interface>>
            +CreateIterator() IIterator~T~
        }
        class IIterator~T~ {
            <<interface>>
            +First() void
            +Next() void
            +IsDone() bool
            +CurrentItem() T
        }
        class ListAggregate~T~
        class ListIterator~T~
        IAggregate~T~ <|.. ListAggregate~T~
        IIterator~T~ <|.. ListIterator~T~
        ListAggregate~T~ ..> ListIterator~T~ : создаёт
    ```


=== "C#"

    ```csharp
    public sealed class BinaryTree<T> : IEnumerable<T>
    {
        private Node<T>? _root;

        public IEnumerator<T> GetEnumerator() => InOrder(_root).GetEnumerator();

        private static IEnumerable<T> InOrder(Node<T>? node)   // внутреннее устройство скрыто
        {
            if (node is null) yield break;
            foreach (var left in InOrder(node.Left)) yield return left;
            yield return node.Value;
            foreach (var right in InOrder(node.Right)) yield return right;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    ```

=== "Python"

    ```python
    class BinaryTree:
        def __init__(self, root: Node | None = None) -> None:
            self._root = root

        def __iter__(self):                       # протокол итерирования языка
            yield from self._in_order(self._root)

        @staticmethod
        def _in_order(node: Node | None):
            if node is None: return
            yield from BinaryTree._in_order(node.left)
            yield node.value                      # генератор = внешний итератор бесплатно
            yield from BinaryTree._in_order(node.right)
    ```

**Важное различение из книги.** Итератор бывает **внешний** (клиент управляет обходом:
`MoveNext`) и **внутренний** (агрегат сам обходит, клиент передаёт операцию — `ForEach`).
Внешний гибче: можно сравнивать две коллекции, прерывать обход.

**Результаты.** Поддерживает разные способы обхода одной коллекции, упрощает интерфейс
агрегата, позволяет вести несколько обходов одновременно. ⚠️ Хрупкость при изменении
коллекции во время обхода — отсюда `InvalidOperationException` в .NET.

> **В .NET.** `IEnumerable<T>`/`IEnumerator<T>` и `yield return` — паттерн, встроенный
> в язык: компилятор генерирует машину состояний, а вы пишете обход как обычный код.
> В Python — протокол `__iter__`/`__next__` и генераторы; в Rust — трейт `Iterator`.
> Это редкий случай, когда паттерн полностью растворился в языке.

---

## Блок 8. Mediator — посредник

**Назначение.** Определяет объект, инкапсулирующий способ взаимодействия множества объектов.
Обеспечивает слабую связанность, избавляя объекты от необходимости явно ссылаться друг
на друга, и позволяет независимо менять схему их взаимодействия.

**Задача.** Пример книги — диалоговое окно: список, кнопка и поле ввода связаны правилами
(выбрали в списке — заполнилось поле, поле пустое — кнопка неактивна). Если каждый виджет
знает про остальных, диалог невозможно переиспользовать.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction TB
        class DialogDirector {
            <<abstract>>
            +WidgetChanged(Widget w) void
        }
        class FontDialogDirector {
            -ListBox list
            -EntryField field
            +WidgetChanged(Widget w) void
        }
        class Widget {
            <<abstract>>
            #DialogDirector director
            +Changed() void
        }
        class ListBox
        class EntryField
        DialogDirector <|-- FontDialogDirector
        Widget <|-- ListBox
        Widget <|-- EntryField
        Widget o-- DialogDirector
        FontDialogDirector --> ListBox
        FontDialogDirector --> EntryField
    ```

=== "Исходник"

    ```
    classDiagram
        direction TB
        class DialogDirector {
            <<abstract>>
            +WidgetChanged(Widget w) void
        }
        class FontDialogDirector {
            -ListBox list
            -EntryField field
            +WidgetChanged(Widget w) void
        }
        class Widget {
            <<abstract>>
            #DialogDirector director
            +Changed() void
        }
        class ListBox
        class EntryField
        DialogDirector <|-- FontDialogDirector
        Widget <|-- ListBox
        Widget <|-- EntryField
        Widget o-- DialogDirector
        FontDialogDirector --> ListBox
        FontDialogDirector --> EntryField
    ```


```csharp
public sealed class FontDialogDirector
{
    private readonly ListBox _fonts;
    private readonly EntryField _field;

    public void WidgetChanged(Widget source)             // все правила связи — здесь
    {
        if (source == _fonts) _field.SetText(_fonts.Selected);
        else if (source == _field) _fonts.Select(_field.Text);
    }
}
```

**Результаты.** Связи «многие ко многим» превращаются в «один ко многим»: вместо паутины —
звезда. Взаимодействие становится **отдельной сущностью**, которую можно изучать и менять.
⚠️ Посредник легко превращается в God Object: вся сложность переезжает в него. Это
осознанный размен — сложность связей на сложность одного класса.

**Посредник против фасада.** Фасад **однонаправлен**: клиенты зовут подсистему, подсистема
о фасаде не знает. Посредник **двунаправлен**: коллеги знают о посреднике и общаются
через него.

---

## Блок 9. Memento — хранитель

**Назначение.** Не нарушая инкапсуляции, получает и сохраняет во внешней памяти внутреннее
состояние объекта, чтобы позже объект можно было восстановить в точно таком же состоянии.

**Задача.** Отмена операции требует запомнить состояние. Но выставить наружу все поля —
значит сломать инкапсуляцию, ради которой всё и затевалось.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction LR
        class Originator {
            +CreateMemento() Memento
            +SetMemento(Memento m) void
        }
        class Memento {
            -State state
            +GetState() State
        }
        class Caretaker {
            -Memento memento
        }
        Originator ..> Memento : создаёт
        Caretaker o-- Memento : хранит, но не заглядывает
    ```

=== "Исходник"

    ```
    classDiagram
        direction LR
        class Originator {
            +CreateMemento() Memento
            +SetMemento(Memento m) void
        }
        class Memento {
            -State state
            +GetState() State
        }
        class Caretaker {
            -Memento memento
        }
        Originator ..> Memento : создаёт
        Caretaker o-- Memento : хранит, но не заглядывает
    ```


**Ключевая идея — два интерфейса.** Широкий, доступный только создателю (`Originator`),
и узкий, доступный смотрителю (`Caretaker`): смотритель может хранить и передавать снимок,
но не может его прочитать или изменить.

```csharp
public sealed class Editor                               // Originator
{
    private string _text = "";
    private int _caret;

    public sealed class Snapshot                         // вложенный тип: доступ к приватным полям
    {
        private readonly string _text;
        private readonly int _caret;
        internal Snapshot(string text, int caret) => (_text, _caret) = (text, caret);
        internal void Restore(Editor editor) => (editor._text, editor._caret) = (_text, _caret);
    }

    public Snapshot Save() => new(_text, _caret);
    public void Restore(Snapshot snapshot) => snapshot.Restore(this);
}

// Caretaker хранит Snapshot и ничего не знает о его содержимом
var history = new Stack<Editor.Snapshot>();
```

**Результаты.** Сохраняет инкапсуляцию и упрощает создателя (ему не нужно вести журнал
версий). ⚠️ Хранители могут дорого обходиться по памяти; смотрителю неизвестна стоимость
хранения; в языках без модификаторов «только для друзей» узкий интерфейс приходится
обеспечивать соглашениями.

> **Комбинация.** Хранитель почти всегда идёт с командой: команда хранит снимок для
> `Unexecute`. В .NET есть и «дешёвый» вариант: неизменяемые записи — снимок получается
> `with`-выражением, а старая версия остаётся валидной сама по себе.

---

## Блок 10. Visitor — посетитель

**Назначение.** Представляет операцию, выполняемую над элементами структуры объектов.
Позволяет определить новую операцию, не изменяя классы этих элементов.

**Задача.** Пример книги — компилятор: по дереву разбора надо выполнять проверку типов,
оптимизацию, генерацию кода, красивую печать. Складывать все эти операции в классы узлов —
значит превращать их в свалку.

=== "Диаграмма"

    ```mermaid
    classDiagram
        direction TB
        class INodeVisitor {
            <<interface>>
            +VisitAssignment(AssignmentNode n) void
            +VisitVariable(VariableNode n) void
        }
        class TypeCheckingVisitor
        class CodeGenVisitor
        class Node {
            <<abstract>>
            +Accept(INodeVisitor v)* void
        }
        class AssignmentNode {
            +Accept(INodeVisitor v) void
        }
        class VariableNode {
            +Accept(INodeVisitor v) void
        }
        INodeVisitor <|.. TypeCheckingVisitor
        INodeVisitor <|.. CodeGenVisitor
        Node <|-- AssignmentNode
        Node <|-- VariableNode
        Node ..> INodeVisitor : Accept
    ```

=== "Исходник"

    ```
    classDiagram
        direction TB
        class INodeVisitor {
            <<interface>>
            +VisitAssignment(AssignmentNode n) void
            +VisitVariable(VariableNode n) void
        }
        class TypeCheckingVisitor
        class CodeGenVisitor
        class Node {
            <<abstract>>
            +Accept(INodeVisitor v)* void
        }
        class AssignmentNode {
            +Accept(INodeVisitor v) void
        }
        class VariableNode {
            +Accept(INodeVisitor v) void
        }
        INodeVisitor <|.. TypeCheckingVisitor
        INodeVisitor <|.. CodeGenVisitor
        Node <|-- AssignmentNode
        Node <|-- VariableNode
        Node ..> INodeVisitor : Accept
    ```


=== "C#"

    ```csharp
    public interface INodeVisitor
    {
        void Visit(AssignmentNode node);
        void Visit(VariableNode node);
    }

    public abstract class Node
    {
        public abstract void Accept(INodeVisitor visitor);    // двойная диспетчеризация
    }

    public sealed class VariableNode : Node
    {
        public string Name { get; init; } = "";
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public sealed class TypeCheckingVisitor : INodeVisitor    // новая операция без правки узлов
    {
        public void Visit(AssignmentNode node) { }
        public void Visit(VariableNode node) { }
    }
    ```

=== "Python"

    ```python
    from functools import singledispatch

    @singledispatch                       # диспетчеризация по типу первого аргумента
    def type_check(node: Node) -> None:
        raise NotImplementedError(type(node).__name__)

    @type_check.register
    def _(node: VariableNode) -> None: ...

    @type_check.register
    def _(node: AssignmentNode) -> None:
        type_check(node.target); type_check(node.value)

    # Accept писать не нужно: одиночная диспетчеризация по типу даётся языком.
    # Плата та же, что у switch в C#: забытый узел обнаружится только в рантайме.
    ```

**Двойная диспетчеризация** — механизм, на котором стоит паттерн: выполняемая операция
зависит **от двух типов** — типа посетителя и типа узла. `Accept` выбирает узел,
`Visit` — посетителя.

**Применимость.** В структуре есть объекты многих классов с различными интерфейсами,
и нужно выполнять над ними операции, зависящие от конкретных классов; над объектами
структуры выполняется много несвязанных операций, и не хочется засорять ими классы;
**классы структуры редко меняются, а операции добавляются часто**.

**Результаты.** Добавить операцию просто — это новый класс посетителя; родственные операции
собраны вместе, а не размазаны по узлам; посетитель может накапливать состояние при обходе.
⚠️ Главная плата, о которой книга предупреждает прямо: **добавить новый класс элемента
трудно** — придётся править интерфейс посетителя и все его реализации. Плюс посетителю
часто нужен доступ к внутренностям элементов, что ослабляет инкапсуляцию.

**Правило выбора:** много операций и стабильная иерархия — посетитель; стабильные операции
и растущая иерархия — обычные виртуальные методы. Эта дилемма известна как «проблема
выражения».

> **В C#.** Сопоставление с образцом по типу (`obj switch { VariableNode v => ..., ... }`)
> решает ту же задачу без `Accept`, но теряет проверку полноты — компилятор не скажет,
> что вы забыли узел. В F# и Rust размеченные объединения дают и то и другое: операции
> добавляются свободно, а исчерпывающность проверяется компилятором. Подробнее посетитель
> разбирается в материале для самостоятельной работы.

---

## Итоги, ошибки и домашнее задание

### Пары, которые путают

| Пара | Чем различаются |
|---|---|
| Стратегия / Состояние | стратегию выбирает клиент; состояние меняет себя само по ходу жизни объекта |
| Стратегия / Шаблонный метод | композиция и рантайм против наследования и компиляции |
| Наблюдатель / Посредник | наблюдатель — рассылка «один ко многим»; посредник — центр связей «многие ко многим» |
| Посредник / Фасад | посредник двунаправлен, коллеги о нём знают; фасад однонаправлен |
| Команда / Стратегия | команда инкапсулирует **запрос** (что сделать и когда), стратегия — **алгоритм** (как сделать) |
| Хранитель / Прототип | хранитель сохраняет состояние ради восстановления, прототип копирует ради создания |
| Посетитель / виртуальные методы | посетитель — когда операции растут, а иерархия стабильна; наоборот — обычный полиморфизм |

### Частые ошибки

1. **Наблюдатель без отписки** — утечка памяти и «мёртвые» подписчики.
2. **Каскад уведомлений** — обновление тянет обновление, и цепочку не отследить.
3. **Посредник-God Object** — сложность связей заменили сложностью одного класса и на этом
   остановились.
4. **Цепочка без гаранта** — никто не обработал запрос, а клиент считает, что всё хорошо.
5. **Состояние, реализованное флагами** — четыре булевых поля вместо явных переходов
   (см. лекцию 4).
6. **Посетитель поверх растущей иерархии** — каждый новый узел ломает всех посетителей.
7. **Команда без обратной операции**, названная командой ради названия.

### Домашнее задание

1. Взять место в своём проекте, где ветвление выбирает поведение, и реализовать его
   **двумя** способами: стратегией и шаблонным методом. В отчёте — сравнение и вывод,
   какой вариант здесь уместнее и почему.
2. Реализовать отмену операций через команду с журналом (минимум три вида команд, включая
   макрокоманду через компоновщик) и хранитель для одного из состояний.
3. Реализовать наблюдателя двумя способами — через события .NET и через явные
   `Attach`/`Detach` — и показать, где возникает утечка при отсутствии отписки.
4. Обойти древовидную структуру из ДЗ по лекции 6 двумя посетителями (например, подсчёт
   статистики и экспорт в другой формат). Затем переписать то же самое на сопоставлении
   с образцом и сравнить, что произойдёт при добавлении нового вида узла.
5. Диаграммы классов и последовательности — в Mermaid, имена участников как в книге
   (Context, Strategy, Subject, Observer, Invoker, Receiver, Originator, Caretaker,
   Element, Visitor).
