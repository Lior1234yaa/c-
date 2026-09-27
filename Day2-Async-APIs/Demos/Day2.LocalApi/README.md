<div dir="rtl">

# Day2.LocalApi — REST API מקומי למעבדות

API קטן שרץ בזיכרון (ללא מסד נתונים) ומדמה חנות: מוצרים (Products) והזמנות (Orders).
המטרה: שכל המעבדות של יום 2 יעבדו גם בלי חיבור לאינטרנט.

## הרצה

<div dir="ltr">

```bash
cd Day2-Async-APIs/Demos/Day2.LocalApi
dotnet run
# Day2.LocalApi listening on http://localhost:5080
```

</div>

השאירו את החלון פתוח במשך היום. עצירה: `Ctrl+C`. הנתונים מתאפסים בכל הפעלה מחדש (או ב-`POST /api/reset`).

## נקודות קצה (Endpoints)

| Method | URL | תיאור | תשובה |
|--------|-----|-------|-------|
| GET | `/` | רשימת נקודות הקצה | 200 |
| GET | `/api/health` | בדיקת חיים | 200 `{status, time}` |
| GET | `/api/products?search=` | כל המוצרים (סינון אופציונלי לפי שם) | 200 `Product[]` |
| GET | `/api/products/{id}` | מוצר לפי מזהה | 200 / 404 |
| POST | `/api/products` | יצירת מוצר (`ProductInput`) | 201 + Location / 400 |
| PUT | `/api/products/{id}` | עדכון מוצר מלא | 200 / 400 / 404 |
| DELETE | `/api/products/{id}` | מחיקה | 204 / 404 |
| GET | `/api/orders?status=` | הזמנות (סינון: Pending/Paid/Shipped/Cancelled) | 200 `Order[]` |
| GET | `/api/orders/{id}` | הזמנה לפי מזהה | 200 / 404 |
| POST | `/api/orders` | יצירת הזמנה (`OrderInput`) | 201 / 400 |
| PUT | `/api/orders/{id}` | שינוי סטטוס (`{"status":"Paid"}`) | 200 / 404 |
| DELETE | `/api/orders/{id}` | מחיקה | 204 / 404 |
| GET | `/api/slow?ms=3000` | עונה אחרי `ms` מילישניות (ברירת מחדל 3000, מקסימום 60000) | 200 |
| GET | `/api/flaky?failRate=0.5` | נכשל אקראית בהסתברות `failRate` | 200 / 503 |
| GET | `/api/stats` | סטטיסטיקה מצטברת | 200 |
| POST | `/api/reset` | איפוס הנתונים לברירת המחדל | 200 |

## מבנה ה-JSON

<div dir="ltr">

```json
// Product
{ "id": 1, "name": "Laptop", "price": 4500, "category": "Computers", "stock": 12 }

// ProductInput (POST/PUT)
{ "name": "Webcam", "price": 199, "category": "Video", "stock": 10 }

// Order
{
  "id": 2, "customer": "Yossi", "createdAt": "2026-01-01T10:00:00Z", "status": "Shipped",
  "items": [ { "productId": 4, "productName": "Monitor 27\"", "quantity": 2, "unitPrice": 1290 } ],
  "total": 2580
}

// OrderInput (POST)
{ "customer": "Dana", "items": [ { "productId": 1, "quantity": 2 } ] }
```

</div>

שימו לב: שמות השדות ב-camelCase, ו-enum (`status`) מוחזר כמחרוזת.

## דוגמאות curl

<div dir="ltr">

```bash
curl http://localhost:5080/api/products
curl http://localhost:5080/api/products/1
curl -X POST http://localhost:5080/api/products -H "Content-Type: application/json" -d '{"name":"Webcam","price":199,"category":"Video","stock":10}'
curl -X PUT  http://localhost:5080/api/orders/1 -H "Content-Type: application/json" -d '{"status":"Paid"}'
curl -X DELETE http://localhost:5080/api/products/7 -i
curl "http://localhost:5080/api/slow?ms=2000"
curl -i "http://localhost:5080/api/flaky?failRate=0.7"
```

</div>

## הערות למרצה

- הפורט קבוע (5080) בקוד: `builder.WebHost.UseUrls("http://localhost:5080")`. אם הוא תפוס — שנו כאן ובלקוחות.
- הקוד עצמו הוא דוגמה טובה ל-Minimal API ול-`ConcurrentDictionary` + `Interlocked` (המחסן משותף בין בקשות מקבילות).
- `/api/slow` מכבד `CancellationToken` של הבקשה — אם הלקוח מתנתק, ה-`Task.Delay` מבוטל.

</div>
