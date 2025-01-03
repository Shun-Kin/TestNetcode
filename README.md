# TestNetcode

Тестовый проект с использованием Netcode for GameObjects
 
Unity 6000.0.31f1

Netcode for GameObjects 2.1.1

Архив со сборкой доступен по ссылке: https://disk.yandex.ru/d/WKBe8jpI38jssg


Описание по тестовому заданию:

    Сетевое решение:

        Использование Unity Netcode for GameObjects.

        Проект собран под последнюю актуальную версию Unity.

    Сетевые роли:

        Создана возможность запуска приложения в 2 режимах: сервер и клиент (экспериментально добавлен режим хоста, не доработан).

        Выбор режима в одном и том же билде через UI.

    Игровая сцена:

        Простая сцена с плоской площадкой.

        Статичные препятствия (3D-примитивы).

        Один «важный» предмет (сфера), с которым игроки могут взаимодействовать.

    Игроки и управление:

        Каждый подключившийся клиент имеет своего собственного управляемого персонажа (капсулу).

        Управление персонажем: WASD для передвижения, Space для прыжка, Q - токнуть сферу, E - подобрать сферу.

        Движение персонажа синхронизировано по сети: сервер получает входные данные от клиента и обновляет состояние персонажа, после чего рассылает обновлённое состояние обратно всем клиентам.

    Взаимодействие с сетевым объектом:

        В сцене находится один сетевой объект (сфера), который игрок может «подобрать» или «толкнуть».

        При столкновении персонажа с этим предметом, если нажать, например, клавишу «E», персонаж «захватывает» предмет.

        Логика: на сервере проверяется условие (позиция персонажа рядом с предметом). Если условие выполнено, право владения предметом передаётся этому игроку, и предмет начинает двигаться вместе с персонажем (если другой игрок держит сферу, то при нажатии на E, он её тпустит, то есть добавлена возмождность отнять).

        Информация о том, кто сейчас владеет предметом, синхронизируется со всеми клиентами.

        Если игрок отпускает предмет (вновь нажимает «E»), предмет остаётся на месте, и владение снимается.

    Синхронизация и оптимизация:

        Использованы базовая синхронизация трансформа для Netcode for GameObjects и вызовы методов сервера для толкания сферы через добавление силы.

    UI:

        Простой UI для подключения: текстовое поле для ввода IP/порт, кнопки "Start Server" и "Connect as Client".

        Отображение статуса подключения (Server Running/Client Connected/Disconnected).

        Ввод и отображение ника игрока.
