# Проба

## Вне вкладок

```mermaid
classDiagram
    class Money {
        +decimal Amount
    }
    note for Money "Неизменяемый.\nСложение только в одной валюте"
```

## Во вкладках

=== "Диаграмма"

    ```mermaid
    classDiagram
        class Order {
            +Guid Id
        }
        note for Order "Заметка внутри вкладки"
    ```

=== "Исходник"

    ```
    classDiagram
    ```

## В details

??? note "Исходник"

    ```
    classDiagram
    ```
