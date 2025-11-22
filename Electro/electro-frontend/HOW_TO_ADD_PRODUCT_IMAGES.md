# 📸 دليل إضافة صور المنتجات

## 🎯 الخطوات السريعة

### 1️⃣ احفظي الصور في المجلد الصحيح

```
Electro.Api/wwwroot/uploads/products/
```

### 2️⃣ سمي الصور بأسماء واضحة:

- `stethoscope-1.jpg` → السماعة الطبية
- `stethoscope-2.jpg` → السماعة من زاوية أخرى
- `anesthesia-machine.jpg` → جهاز التخدير
- `operation-table.jpg` → طاولة العمليات
- `mri-scanner.jpg` → جهاز MRI
- `blood-pressure.jpg` → جهاز قياس الضغط

---

## 📋 طريقة الإضافة الكاملة

### الطريقة 1: من خلال SQL (الأسهل)

1. افتحي SQL Server Management Studio
2. اتصلي بقاعدة البيانات
3. افتحي الملف: `SAMPLE_MEDICAL_PRODUCTS.sql`
4. شغلي السكريبت
5. انسخي الصور للمجلد

### الطريقة 2: من خلال Postman/Swagger

#### A. أضيفي الفئات الأول:

```http
POST http://localhost:5000/api/category
Content-Type: multipart/form-data

{
  "name": "أجهزة طبية احترافية",
  "description": "أجهزة طبية متقدمة للمستشفيات",
  "image": [ملف الصورة]
}
```

كرري للفئات:
- أجهزة قياس طبية
- معدات غرف العمليات
- أجهزة الأشعة والتصوير
- أدوات طبية

#### B. أضيفي المنتجات:

**مثال: السماعة الطبية**

```http
POST http://localhost:5000/api/products
Content-Type: multipart/form-data

{
  "name": "سماعة طبية Littmann Classic III",
  "description": "سماعة طبية احترافية من 3M Littmann...",
  "price": 2800,
  "discountedPrice": 2500,
  "stock": 25,
  "categoryId": 5,
  "brand": "Littmann 3M",
  "countryOfOrigin": "الولايات المتحدة",
  "images": [صورة السماعة]
}
```

**مثال: جهاز التخدير**

```http
POST http://localhost:5000/api/products
Content-Type: multipart/form-data

{
  "name": "جهاز التخدير الطبي SUPERSTAR",
  "description": "جهاز تخدير طبي متطور...",
  "price": 280000,
  "discountedPrice": 265000,
  "stock": 3,
  "categoryId": 1,
  "brand": "SUPERSTAR Medical",
  "countryOfOrigin": "الصين",
  "images": [صورة جهاز التخدير]
}
```

**مثال: طاولة العمليات**

```http
POST http://localhost:5000/api/products
Content-Type: multipart/form-data

{
  "name": "طاولة عمليات جراحية كهربائية",
  "description": "طاولة عمليات مع إضاءة LED...",
  "price": 450000,
  "discountedPrice": 425000,
  "stock": 2,
  "categoryId": 3,
  "brand": "Medical Systems",
  "countryOfOrigin": "ألمانيا",
  "images": [صورة الطاولة]
}
```

**مثال: جهاز MRI**

```http
POST http://localhost:5000/api/products
Content-Type: multipart/form-data

{
  "name": "جهاز التصوير بالرنين المغناطيسي MRI",
  "description": "جهاز MRI بقوة 1.5 تسلا...",
  "price": 8500000,
  "stock": 1,
  "categoryId": 4,
  "brand": "Siemens Healthineers",
  "countryOfOrigin": "ألمانيا",
  "images": [صورة MRI]
}
```

**مثال: جهاز الضغط**

```http
POST http://localhost:5000/api/products
Content-Type: multipart/form-data

{
  "name": "جهاز قياس ضغط الدم الرقمي",
  "description": "جهاز أوتوماتيكي للذراع...",
  "price": 850,
  "discountedPrice": 699,
  "stock": 50,
  "categoryId": 2,
  "brand": "Omron",
  "countryOfOrigin": "اليابان",
  "images": [صورة جهاز الضغط]
}
```

---

## 🔧 باستخدام cURL (من Terminal)

```bash
# مثال: إضافة السماعة الطبية
curl -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: multipart/form-data" \
  -F "name=سماعة طبية Littmann Classic III" \
  -F "description=سماعة طبية احترافية..." \
  -F "price=2800" \
  -F "discountedPrice=2500" \
  -F "stock=25" \
  -F "categoryId=5" \
  -F "brand=Littmann 3M" \
  -F "countryOfOrigin=الولايات المتحدة" \
  -F "images=@C:\path\to\stethoscope.jpg"
```

---

## 📂 هيكل المجلدات

```
Electro.Api/
└── wwwroot/
    └── uploads/
        ├── products/
        │   ├── stethoscope-1.jpg
        │   ├── stethoscope-2.jpg
        │   ├── anesthesia-machine.jpg
        │   ├── operation-table.jpg
        │   ├── mri-scanner.jpg
        │   └── blood-pressure.jpg
        ├── categories/
        │   └── medical-devices.jpg
        └── banners/
            └── medical-banner.jpg
```

---

## 🎨 مواصفات الصور الموصى بها

- **التنسيق:** JPG أو PNG
- **الحجم:** 800×800 بكسل على الأقل
- **الحجم الملف:** أقل من 2 ميجا
- **الجودة:** عالية ووا ضحة
- **الخلفية:** بيضاء أو شفافة (PNG)

---

## ✅ التأكد من الصور

بعد رفع الصور، جربي الروابط:

```
http://localhost:5000/uploads/products/stethoscope-1.jpg
http://localhost:5000/uploads/products/anesthesia-machine.jpg
http://localhost:5000/uploads/products/operation-table.jpg
http://localhost:5000/uploads/products/mri-scanner.jpg
http://localhost:5000/uploads/products/blood-pressure.jpg
```

لازم تفتح الصور في المتصفح!

---

## 🚀 بعد إضافة المنتجات

روحي للموقع وشوفي المنتجات:

```
http://localhost:3000/products
```

المفروض تظهر كل المنتجات مع صورها! 🎉

---

## ⚠️ ملاحظات مهمة

1. **تأكدي إن Backend شغال** على port 5000
2. **المجلد uploads لازم يكون موجود**
3. **الصور لازم تكون بنفس الأسماء** في SQL
4. **لو الصور مش ظاهرة**، تأكدي من `next.config.js`:

```javascript
images: {
  remotePatterns: [
    {
      protocol: 'http',
      hostname: 'localhost',
      port: '5000',
      pathname: '/uploads/**',
    },
  ],
}
```

---

**محتاجة مساعدة؟ قوليلي!** 😊

