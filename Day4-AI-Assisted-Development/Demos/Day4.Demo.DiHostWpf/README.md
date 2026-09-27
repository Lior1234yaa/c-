<div dir="rtl">

# Day4.Demo.DiHostWpf

אפליקציית WPF קטנה שמדגימה `Microsoft.Extensions.Hosting` בתוך WPF:

- `App.xaml.cs` מקים `IHost` (DI + Configuration + Logging) ופותח את `MainWindow` מתוך ה-container.
- `appsettings.json` → `AppOptions` דרך Options pattern.
- `OrderService` מקבל `IOrderRepository`, `IClock`, `IOptions<AppOptions>` ו-`ILogger` בבנאי.
- `MainViewModel` מוזרק ל-`MainWindow` — אפס לוגיקה ב-code-behind.

הרצה (ב-Windows): `dotnet run`. ב-Linux/macOS הפרויקט מתקמפל (`EnableWindowsTargeting`) אך אינו רץ.

</div>
