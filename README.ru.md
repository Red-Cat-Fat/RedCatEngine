[English](README.md) | **Русский**

# RedCatEngine

RedCatEngine — набор пакетов Unity, закрывающих типовые части игры: внедрение зависимостей, запуск приложения, машины состояний, окна интерфейса, пулы, конфиги, управляемые данными значения, условия, награды и квесты, сервисы и инструменты.

Пакеты независимы. Клонировать этот репозиторий в свою игру не нужно — добавляйте только нужные пакеты через Unity Package Manager.

## Пакеты

| Пакет | Для чего | Версия | Документация |
|---|---|---|---|
| Dependency Injection | DI-контейнер: регистрации, конструкторы с `[Inject]`, дочерние контейнеры, внедрение в `MonoBehaviour` | 1.2.1 | [RU](RedCatEngineUnityProject/Packages/DependencyInjection/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/DependencyInjection/README.md) |
| State Machine | Типизированная машина состояний с данными при входе и очередью состояний | 2.0.1 | [RU](RedCatEngineUnityProject/Packages/StateMachine/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/StateMachine/README.md) |
| Application runner | Запуск игры: состояния, создаваемые через DI, локализация, загрузка и сохранение модели игрока | 1.1.1 | [RU](RedCatEngineUnityProject/Packages/ApplicationRunner/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/ApplicationRunner/README.md) |
| Configs | Конфиги на `ScriptableObject` со стабильными идентификаторами и типизированными ссылками по ним | 1.1.1 | [RU](RedCatEngineUnityProject/Packages/Configs/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Configs/README.md) |
| Pools | Пулы объектов для префабов, настраиваемые в инспекторе или из кода | 1.1.0 | [RU](RedCatEngineUnityProject/Packages/Pools/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Pools/README.md) |
| Common services | Логи, время, цикл обновления, отложенные вызовы, случайные числа, сохранения, теги, звук, хранилища | 2.0.1 | [RU](RedCatEngineUnityProject/Packages/UniversalServices/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/UniversalServices/README.md) |
| Windows | Система окон на MVP со слоями и родительскими окнами | 2.0.2 | [RU](RedCatEngineUnityProject/Packages/Windows/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Windows/README.md) |
| Values | Числа и флаги, собираемые в инспекторе из констант, переменных, кривых и операций | 1.1.1 | [RU](RedCatEngineUnityProject/Packages/Values/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Values/README.md) |
| Conditions | Проверки, собираемые в инспекторе из логических блоков и собственных проверок сервисов | 1.1.1 | [RU](RedCatEngineUnityProject/Packages/Conditions/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Conditions/README.md) |
| Rewards | Награды, описанные в конфигах и выдаваемые одним сервисом | 1.0.2 | [RU](RedCatEngineUnityProject/Packages/Rewards/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Rewards/README.md) |
| Quests | Конфиги квестов, системы сюжетных и ежедневных квестов и достижений, сохранение | 1.2.0 | [RU](RedCatEngineUnityProject/Packages/Quests/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Quests/README.md) |
| Resources | Обёртка над Addressables: загрузка с кешем, создание экземпляров, очистка | 1.0.1 | [RU](RedCatEngineUnityProject/Packages/ResourceSystem/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/ResourceSystem/README.md) |
| Game Settings | Графические настройки как данные с сохранением изменений игрока | 2.0.1 | [RU](RedCatEngineUnityProject/Packages/GameSettings/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/GameSettings/README.md) |
| Benchmark | Замер FPS и памяти игрового сценария с записью отчёта в файл | 1.1.0 | [RU](RedCatEngineUnityProject/Packages/Benchmark/README.ru.md) · [EN](RedCatEngineUnityProject/Packages/Benchmark/README.md) |

На странице каждого пакета есть его описание, зависимости со ссылками и примеры использования.

## Установка

1. Откройте **Window > Package Manager**.
2. Нажмите **+** > **Add package from git URL**.
3. Введите адрес пакета:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/<PackageFolder>
```

`<PackageFolder>` — имя папки: `ApplicationRunner`, `Benchmark`, `Conditions`, `Configs`, `DependencyInjection`, `GameSettings`, `Pools`, `Quests`, `ResourceSystem`, `Rewards`, `StateMachine`, `UniversalServices`, `Values`, `Windows`.

Зависимости пакета устанавливаются так же; начинайте с пакетов, у которых зависимостей нет. То же самое можно сделать, отредактировав `Packages/manifest.json` — например, для Windows:

```json
{
  "dependencies": {
    "com.boronnikov.games.red-cat-engine.configs": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Configs",
    "com.boronnikov.games.red-cat-engine.di": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/DependencyInjection",
    "com.boronnikov.games.red-cat-engine.pools": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Pools",
    "com.boronnikov.games.red-cat-engine.services": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/UniversalServices",
    "com.boronnikov.games.red-cat-engine.windows": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Windows"
  }
}
```

## Зависимости между пакетами

Пакеты объявляют зависимости друг от друга в `package.json`. Все нужные пакеты устанавливаются по git URL; версии берутся из `package.json` каждого пакета.

| Пакет | Требует |
|---|---|
| DependencyInjection | — |
| StateMachine | — |
| Configs | — |
| Pools | — |
| Benchmark | — |
| ApplicationRunner | DependencyInjection, StateMachine |
| Conditions | Configs, DependencyInjection, Values |
| GameSettings | Configs |
| Quests | Conditions, Configs, DependencyInjection, Rewards, Values |
| ResourceSystem | `com.unity.addressables` |
| Rewards | Conditions, Configs, DependencyInjection, Values |
| UniversalServices | Configs, DependencyInjection, Pools |
| Values | Configs, DependencyInjection, UniversalServices |
| Windows | Configs, DependencyInjection, UniversalServices |

Зависимости транзитивны: например, пакету Quests нужно и всё то, что требуют Conditions, Rewards и Values.

Git-пакеты нельзя объявить зависимостями UPM, поэтому добавляйте их в проект вручную:

- [`com.elmortem.serializereferenceeditor`](https://github.com/elmortem/serializereferenceeditor) — Conditions, Values, Quests, Rewards, GameSettings
- [`com.elmortem.localization`](https://github.com/elmortem/localization) — ApplicationRunner

`DoTweenExtensions` из UniversalServices компилируется только при заданном символе компиляции `DOTWEEN` (установлен DOTween).

## Требования

Unity 2021.3 или новее; для Game Settings — Unity 6000.0 или новее.

## Репозиторий

`RedCatEngineUnityProject/` — проект Unity, в котором пакеты разрабатываются и тестируются; сами пакеты лежат в `RedCatEngineUnityProject/Packages/`. У каждого пакета есть EditMode-тесты, которые запускаются в GitHub Actions при каждом пуше.
