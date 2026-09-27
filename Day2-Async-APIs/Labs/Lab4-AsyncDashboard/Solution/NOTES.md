# מעבדה 4 — הערות לפתרון

## הרעיון המרכזי: כישלון הוא ערך, לא חריגה

`FetchSafeAsync<T>` הופך כל מקור ל-`SourceResult<T>` — הצלחה עם ערך או כישלון עם סיבה קצרה. כך `Task.WhenAll` לעולם לא זורק בגלל מקור אחד, והלוח מציג את מה שיש. חריגה אחת כן עוברת הלאה: ביטול שהמשתמש יזם (`when (!ct.IsCancellationRequested)`), כי אז רוצים לצאת, לא להציג "FAILED".

ההבחנה בין timeout לביטול: שני המקרים זורקים `OperationCanceledException`. ב-`WeatherSource` יש `CancellationTokenSource` **מקושר** (`CreateLinkedTokenSource(ct)` + `CancelAfter(4s)`), ולכן הטוקן הפנימי מבוטל גם ב-Ctrl+C וגם ב-timeout. ב-`FetchSafeAsync` בודקים את ה-`ct` **החיצוני**: אם הוא מבוטל — זה המשתמש; אחרת — timeout.

## מקבילות

ארבעת ה-`FetchSafeAsync` מופעלים בלי `await` ואז `await Task.WhenAll`. זמן הרענון ≈ המקור האיטי ביותר (בלי אינטרנט: ~4 שניות בגלל ה-timeout של מזג האוויר; עם: ~200–500 ms). `SemaphoreSlim(maxConcurrency)` מאפשר להדגים את ההשפעה של הגבלת מקביליות: `--max 1` הופך את הכל לסדרתי.

`.Result` אחרי `WhenAll` הוא בטוח — ה-Tasks כבר הושלמו ואין חסימה. זה המקום היחיד שבו `.Result` לגיטימי.

## `ChannelLogger`

- `Log` עושה `TryWrite` על Channel לא-מוגבל — תמיד מצליח, אף פעם לא חוסם, ולכן מותר לקרוא לו מכל תהליכון בלי לחשוש לביצועים.
- תהליכון רקע יחיד (`SingleReader = true`) קורא `ReadAllAsync` וכותב ל-`Console.Error`. אין ערבוב שורות ואין `lock`.
- `DisposeAsync` קורא `Complete()` ומחכה ל-pump — כך ההודעות האחרונות ("cancelled by user") לא הולכות לאיבוד. `await using` ב-`Program` מבטיח את זה גם ביציאה בגלל חריגה.
- הלוגים ב-stderr והלוח ב-stdout: `dotnet run 2> log.txt` מפריד ביניהם.

## לולאת הרענון

`PeriodicTimer` (.NET 6+) מחליף `while (true) { await Task.Delay(...) }` ולא "צובר" איחורים. `WaitForNextTickAsync(ct)` זורק `OperationCanceledException` בביטול, שנתפס פעם אחת ב-`Program`. `Console.CancelKeyPress` עם `e.Cancel = true` מונע מהתהליך למות באמצע — ה-`await using` של הלוגר וה-`using` של `HttpClient` רצים כרגיל.

## בונוס שמומש: stale values

`_lastGood` שומר את התקציר האחרון שהצליח לכל מקור. כשמקור נופל, השורה מציגה "offline (timeout) — stale from 12:30:05: 27.4°C..." — מידע שימושי יותר מ-FAILED עירום, ודפוס נפוץ בלוחות בקרה אמיתיים.

## איך לבדוק את קריטריוני הקבלה

```bash
dotnet run -- --once                 # רענון אחד
dotnet run -- --interval 2           # ואז לעצור את Day2.LocalApi באמצע -> FAILED, להפעיל -> OK
dotnet run -- --once --max 1         # סדרתי: זמן הרענון = סכום המקורות
dotnet run -- --once --quiet 2>/dev/null   # בלי לוגים
```

בלי אינטרנט מזג האוויר יציג `offline (timeout)` או `offline (connection failed)` והשאר תקינים — זו התנהגות נכונה.
