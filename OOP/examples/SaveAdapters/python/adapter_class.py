"""
ЭТАП 3. Адаптер классов — схема GoF, которая требует множественного наследования.
В C# её выразить нельзя, в Python можно.

Задание:
  1) дописать ClassAdapter так, чтобы он наследовал и Target, и Adaptee;
  2) показать тестом, что он может переопределить поведение Adaptee
     (в адаптере объектов так сделать нельзя — оно спрятано за ссылкой);
  3) показать, что адаптер классов привязан ровно к одному классу Adaptee
     и с его подклассами не работает.
"""


class LegacySaveSystem:                      # Adaptee: чужой код
    """Синхронная система со слотами и кодами возврата."""

    OK = 0
    ERR_LOCKED = 2

    def __init__(self) -> None:
        self._slots: dict[int, str] = {}
        self._locked: set[int] = set()

    def write_slot(self, slot: int, data: str) -> int:
        if slot in self._locked:
            return self.ERR_LOCKED
        self._slots[slot] = data
        return self.OK

    def read_slot(self, slot: int) -> str:
        return self._slots.get(slot, "")

    def lock_slot(self, slot: int) -> None:
        self._locked.add(slot)


class SaveStorage:                           # Target: контракт игры
    def save(self, name: str, payload: dict) -> None:
        raise NotImplementedError

    def load(self, name: str) -> dict | None:
        raise NotImplementedError


class ObjectAdapter(SaveStorage):            # схема 1: композиция
    def __init__(self, legacy: LegacySaveSystem) -> None:
        self._legacy = legacy

    def save(self, name: str, payload: dict) -> None:
        code = self._legacy.write_slot(self._slot(name), self._pack(payload))
        if code == LegacySaveSystem.ERR_LOCKED:
            raise RuntimeError(f"Сохранение «{name}» заблокировано")

    def load(self, name: str) -> dict | None:
        raw = self._legacy.read_slot(self._slot(name))
        return self._unpack(raw) if raw else None

    @staticmethod
    def _slot(name: str) -> int:
        return abs(hash(name)) % 8

    @staticmethod
    def _pack(payload: dict) -> str:
        return "|".join(f"{k}={v}" for k, v in payload.items())

    @staticmethod
    def _unpack(raw: str) -> dict:
        return dict(part.split("=", 1) for part in raw.split("|"))


class ClassAdapter(SaveStorage, LegacySaveSystem):   # схема 2: множественное наследование
    """
    TODO этап 3: реализуйте save/load, вызывая УНАСЛЕДОВАННЫЕ write_slot и read_slot
    (без self._legacy — здесь адаптер сам ЯВЛЯЕТСЯ чужой системой).

    Затем переопределите write_slot так, чтобы адаптер вёл себя иначе,
    чем исходная система, — и объясните в отчёте, почему в ObjectAdapter
    так сделать невозможно.
    """

    def __init__(self) -> None:
        LegacySaveSystem.__init__(self)

    def save(self, name: str, payload: dict) -> None:
        raise NotImplementedError("этап 3")

    def load(self, name: str) -> dict | None:
        raise NotImplementedError("этап 3")
