# دليل التثبيت المفصل - Electro Frontend

## 📋 المتطلبات الأساسية

قبل البدء، تأكد من توفر التالي:

- **Node.js** الإصدار 18.0.0 أو أحدث
- **npm** أو **yarn** أو **pnpm**
- **Git** (اختياري)

## 🔧 خطوات التثبيت

### 1. التحقق من Node.js

```bash
node --version
npm --version
```

إذا لم يكن مثبتاً، قم بتحميله من: https://nodejs.org/

### 2. الانتقال إلى مجلد المشروع

```bash
cd electro-frontend
```

### 3. تثبيت الحزم

اختر إحدى الطرق التالية:

**باستخدام npm:**
```bash
npm install
```

**باستخدام yarn:**
```bash
yarn install
```

**باستخدام pnpm:**
```bash
pnpm install
```

### 4. إعداد متغيرات البيئة

أنشئ ملف `.env.local` في المجلد الرئيسي:

```bash
# ويندوز
copy .env.example .env.local

# ماك/لينكس
cp .env.example .env.local
```

ثم قم بتعديل الملف:

```env
# API URL - عنوان الـ Backend API
NEXT_PUBLIC_API_URL=http://localhost:5000/api

# يمكن إضافة متغيرات أخرى حسب الحاجة
# NEXT_PUBLIC_FIREBASE_API_KEY=your_key_here
```

### 5. تشغيل المشروع

**للتطوير (Development):**
```bash
npm run dev
```

المشروع سيعمل على: http://localhost:3000

**للإنتاج (Production):**
```bash
npm run build
npm start
```

## 🔍 التحقق من التثبيت

بعد تشغيل المشروع، افتح المتصفح على:
```
http://localhost:3000
```

يجب أن ترى الصفحة الرئيسية للمتجر.

## ⚠️ حل المشاكل الشائعة

### مشكلة: Port مستخدم بالفعل

```bash
Error: listen EADDRINUSE: address already in use :::3000
```

**الحل:**
```bash
# ويندوز - إيقاف العملية على Port 3000
netstat -ano | findstr :3000
taskkill /PID <PID_NUMBER> /F

# ماك/لينكس
lsof -ti:3000 | xargs kill -9

# أو تشغيل على port آخر
PORT=3001 npm run dev
```

### مشكلة: فشل تثبيت الحزم

```bash
npm ERR! code ERESOLVE
```

**الحل:**
```bash
# حذف node_modules و package-lock.json
rm -rf node_modules package-lock.json

# إعادة التثبيت
npm install --legacy-peer-deps
```

### مشكلة: خطأ في الاتصال بالـ API

```
Error: Network Error
```

**الحل:**
- تأكد من أن Backend API يعمل على: http://localhost:5000
- تحقق من `.env.local` أن `NEXT_PUBLIC_API_URL` صحيح
- تأكد من تفعيل CORS في Backend

## 🔄 تحديث المشروع

```bash
# سحب آخر التحديثات (إذا كان من Git)
git pull origin main

# تحديث الحزم
npm install

# إعادة البناء
npm run build
```

## 🧪 اختبار المشروع

```bash
# Lint check
npm run lint

# Type check
npx tsc --noEmit

# Build test
npm run build
```

## 📦 البناء للنشر

### Vercel

```bash
npm install -g vercel
vercel login
vercel
```

### Docker

```bash
docker build -t electro-frontend .
docker run -p 3000:3000 electro-frontend
```

### Static Export (اختياري)

إذا كنت تريد ملفات HTML ثابتة:

```bash
# عدّل next.config.js
# أضف: output: 'export'

npm run build
# الملفات في: out/
```

## 🌐 متطلبات Backend

تأكد من:
1. Backend API يعمل على Port 5000
2. CORS مفعل للسماح لـ localhost:3000
3. قاعدة البيانات متصلة وتعمل

### إعداد CORS في Backend (.NET)

في `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

app.UseCors("AllowFrontend");
```

## 📱 التوافق

المشروع متوافق مع:
- ✅ Chrome (آخر إصدارين)
- ✅ Firefox (آخر إصدارين)
- ✅ Safari (آخر إصدارين)
- ✅ Edge (آخر إصدارين)
- ✅ Mobile browsers

## 🎯 الخطوات التالية

بعد التثبيت الناجح:

1. **إنشاء حساب:** اذهب لـ `/register`
2. **تسجيل الدخول:** اذهب لـ `/login`
3. **تصفح المنتجات:** اذهب لـ `/products`
4. **إضافة للسلة:** اختر منتجاً وأضفه للسلة
5. **إتمام الطلب:** اذهب للسلة ثم Checkout

## 📞 الدعم

للمساعدة والدعم:
- فتح Issue على GitHub
- التواصل عبر: info@electro.com

---

حظاً موفقاً! 🚀

