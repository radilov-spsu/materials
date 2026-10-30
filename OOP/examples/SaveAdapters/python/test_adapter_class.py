import pytest

from adapter_class import ClassAdapter, LegacySaveSystem, ObjectAdapter


def test_object_adapter_saves_and_loads():
    storage = ObjectAdapter(LegacySaveSystem())

    storage.save("quicksave", {"player": "Аня", "level": "7"})

    assert storage.load("quicksave") == {"player": "Аня", "level": "7"}


def test_object_adapter_translates_error_code():
    legacy = LegacySaveSystem()
    legacy.lock_slot(abs(hash("autosave")) % 8)
    storage = ObjectAdapter(legacy)

    with pytest.raises(RuntimeError):
        storage.save("autosave", {"player": "Аня"})


# TODO этап 3: такой же тест для ClassAdapter
# TODO этап 3: тест, показывающий, что ClassAdapter МОЖЕТ переопределить
#              поведение унаследованного write_slot
# TODO этап 3: тест или рассуждение в отчёте — почему ClassAdapter
#              не работает с подклассами LegacySaveSystem
