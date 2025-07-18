# InvoiceCreator - Генератор счетов

WPF-приложение для генерации счетов на оплату с поддержкой двух организаций (Россия и Казахстан).

## Архитектура

### Структура проекта
- **Core** - Бизнес-логика и модели данных
- **Desktop** - WPF-приложение с MaterialDesign
- **Infrastructure** - Слой доступа к данным (в разработке)

### MVVM-архитектура
- **BaseViewModel** - Базовый класс с INotifyPropertyChanged
- **MainViewModel** - Главная ViewModel, координирует работу
- **InvoiceInputViewModels** - ViewModel для создания счетов
- **ProductViewModel** - ViewModel для управления товарами/услугами
- **AsyncRelayCommand** - Асинхронная команда с поддержкой CanExecute

### Dependency Injection
- Использует Microsoft.Extensions.DependencyInjection
- Автоматическая регистрация ViewModels
- Поддержка логирования через ILogger

## Функционал

### ✅ Реализовано
1. **WPF-интерфейс на MaterialDesign**
   - Современный UI с Material Design
   - Адаптивная верстка
   - Индикаторы загрузки

2. **Ввод данных счета**
   - ИНН/БИН клиента
   - Номер договора
   - Дата договора
   - Наименование компании

3. **Управление товарами/услугами**
   - Добавление/редактирование/удаление
   - Автоподсчет суммы
   - DataGrid с сортировкой

4. **Валидация и UX**
   - CanExecute для команд
   - Индикаторы загрузки
   - Обработка ошибок

5. **Логирование**
   - Структурированное логирование
   - Поддержка Console и Debug логов

### 🚧 В разработке
1. **Генерация PDF**
   - Шаблоны для разных организаций
   - Вставка печатей и подписей

2. **Интеграция с API**
   - Dadata для РФ
   - pk.adata.kz для КЗ

3. **Автонумерация**
   - Отдельные счетчики для организаций
   - Сохранение в БД/файл

4. **Акты**
   - Генерация актов на основе счетов

## Технологии

- **.NET 8.0**
- **WPF** с MaterialDesignThemes
- **Microsoft.Extensions.DependencyInjection**
- **Microsoft.Extensions.Logging**
- **MVVM-паттерн**

## Запуск

1. Установите .NET 8.0 SDK
2. Клонируйте репозиторий
3. Выполните: `dotnet run --project Desktop`

## Структура кода

### ViewModels
- Все ViewModels наследуют BaseViewModel
- Используют AsyncRelayCommand для асинхронных операций
- Поддерживают INotifyPropertyChanged
- Имеют валидацию через CanExecute

### Views
- UserControl'ы для переиспользования
- Правильная передача DataContext
- MaterialDesign стили

### Модели
- Product с автоподсчетом Total
- Подготовлены модели для счетов и организаций

## Следующие шаги

1. **PDF-генерация** - интеграция с iTextSharp/PdfSharp
2. **API-интеграция** - Dadata и pk.adata.kz
3. **Хранение данных** - SQLite или JSON
4. **Шаблоны** - создание PDF-шаблонов
5. **Тестирование** - unit-тесты для ViewModels