# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Языки и кодировка

Основной язык — VB.NET, производительный слой — C# (unsafe). Комментарии в коде на русском языке.

## Сборка и тестирование

```bash
dotnet build Bwl.Imaging.sln
dotnet test Bwl.Imaging.UnitTests/Bwl.Imaging.UnitTests.vbproj
dotnet build Bwl.Imaging.sln -c Release   # генерирует .nupkg в release/dll/
```

Выходные артефакты: `debug/dll/` (Debug), `release/dll/` (Release).

Общие свойства (Version, Authors, License, OutputPath, SupportedOSPlatform) — в `Directory.Build.props`.

## Архитектура

**Bwl.Imaging** — библиотека компьютерного зрения. Платформа: **Windows** (`System.Drawing.Common`, помечено `SupportedOSPlatform("windows")`). Изображения представляются как многоканальные целочисленные или вещественные матрицы (`Integer[]` / `Double[]`), а не только через GDI+ Bitmap.

### Проекты в решении (Bwl.Imaging.sln)

| Проект | Язык | Target | Назначение |
|--------|------|--------|------------|
| bwl.Imaging | VB.NET | net10.0;netstandard2.0 | Ядро: матрицы, обработка, геометрия, цвет |
| Bwl.Imaging.Unsafe | C# | net10.0;netstandard2.0 | Unsafe-операции: crop, patch, sharpen, memcpy |
| Bwl.Imaging.WinForms | VB.NET | net10.0-windows | Утилиты отображения (PictureBox и пр.) |
| Bwl.Imaging.UnitTests | VB.NET | net10.0-windows | MSTest-тесты |
| bwl.Imaging.Test | VB.NET | net10.0-windows | Тестовое WinForms-приложение |

Отдельное решение: `Bwl.Imaging.OCR.sln` — распознавание символов.

### Ключевой конвейер обработки

```
Bitmap → BitmapOperations.LoadBitmap() → byte[]
       → GetGrayMatrix() / GetRGBMatrix()
       → Integer[x + y*Width] (прямой доступ к пикселям)
       → Обработка (фильтры, resize, детекция движения)
       → BitmapConverter.MatrixToBitmap()
```

### Иерархия матриц

- `CommonMatrix` (базовый, `List(Of Integer[])`) → `GrayMatrix`, `RGBMatrix`
- `CommonFloatMatrix` (`List(Of Double[])`) → `GrayFloatMatrix`, `RGBFloatMatrix`
- Доступ к пикселям: `MatrixPixel(channel, x, y)`, `FitX()`/`FitY()` для ограничения границ

### Unsafe-слой (C#, Bwl.Imaging.Unsafe)

- `UnsafeFunctions.cs` — CropGray/CropRgb, PatchGray/PatchRgb, Sharpen5Gray/Sharpen5Rgb, BitmapClone. Использует `Bitmap.LockBits()` + pointer arithmetic + `Parallel.For`.
- `RawFrameFunctions.cs` — конвертация HDR-кадров (12-bit+) в Bitmap с gain/power law маппингом.
- P/Invoke к `msvcrt.dll` для `memcpy`.

### Потокобезопасность

- `BitmapInfo` — потокобезопасная обёртка вокруг Bitmap с `Semaphore(1,1)`, JPEG-кэшем и глобальными счётчиками через `Interlocked`.
- Прямой доступ к матрицам **не** потокобезопасен — использовать `BitmapInfo` для синхронизации.

### Разделяемые исходники (refs-src/)

`JpegCodec.vb` и `Serializer.vb` подключаются через `<Compile Include="..." Link="..."/>` в нескольких проектах.

## NuGet-пакеты

Три пакета генерируются при Release-сборке (`GeneratePackageOnBuild=true`):
- `Bwl.Imaging` — ядро
- `Bwl.Imaging.Unsafe` — производительный слой
- `Bwl.Imaging.WinForms` — WinForms-контролы

Публикация на NuGet.org: push тега `v*` → GitHub Actions workflow `publish.yml` (требует секрет `NUGET_API_KEY`).

## Зависимости

- `System.Drawing.Common` 6.0.0 (в Bwl.Imaging.Unsafe и Bwl.Imaging.WinForms)

## Особенности при разработке

- Формула яркости (luma): CCIR-601 (`Y = 0.299R + 0.587G + 0.114B`), зашита в код.
- `Format8bppIndexed` требует установки палитры через `GetGrayScalePalette()`.
- Для не-4-байт-выровненных bitmap необходим учёт stride.
- Конвертация Float→Integer — усечение (clamping) к диапазону 0–255.
- Предупреждения VB.NET, трактуемые как ошибки: 41999, 42016, 42020, 42021, 42022.
- WinForms-проект подавляет WFO1000 (DesignerSerializationVisibility для legacy-контролов).
