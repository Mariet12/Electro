# 📜 Scripts المتاحة - Electro Frontend

هذا الدليل يشرح جميع الأوامر المتاحة في المشروع.

---

## 🎯 الأوامر الأساسية

### تشغيل التطوير
```bash
npm run dev
```
- يشغل المشروع في وضع التطوير
- يعمل على: http://localhost:3000
- Hot Reload مفعل (التغييرات تظهر مباشرة)
- مناسب للتطوير والاختبار

### البناء للإنتاج
```bash
npm run build
```
- يبني المشروع للإنتاج
- يُنشئ ملفات محسّنة في `.next/`
- يقوم بـ optimization للصور والكود
- يُنشئ static pages حيثما أمكن

### تشغيل الإنتاج
```bash
npm start
```
- يشغل المشروع المبني (بعد `npm run build`)
- يعمل على: http://localhost:3000
- للاستخدام في الإنتاج

### Linting
```bash
npm run lint
```
- يفحص الكود للأخطاء والمشاكل
- يتبع قواعد ESLint
- يظهر warnings و errors

---

## 🔧 أوامر إضافية مفيدة

### Type Checking
```bash
npx tsc --noEmit
```
- يفحص أخطاء TypeScript
- لا يُنشئ ملفات JavaScript
- مفيد للتأكد من صحة الأنواع

### تنظيف Cache
```bash
# ويندوز
rmdir /s /q .next
rmdir /s /q node_modules

# ماك/لينكس
rm -rf .next node_modules
```
- يحذف الملفات المؤقتة
- مفيد عند حدوث مشاكل غريبة

### إعادة التثبيت الكاملة
```bash
# ويندوز
rmdir /s /q node_modules
del package-lock.json
npm install

# ماك/لينكس
rm -rf node_modules package-lock.json
npm install
```

### تحديث الحزم
```bash
# عرض الحزم القديمة
npm outdated

# تحديث الحزم
npm update

# تحديث Next.js
npm install next@latest react@latest react-dom@latest
```

---

## 🐳 Docker Commands

### بناء Docker Image
```bash
docker build -t electro-frontend .
```

### تشغيل Container
```bash
docker run -p 3000:3000 electro-frontend
```

### تشغيل مع Environment Variables
```bash
docker run -p 3000:3000 -e NEXT_PUBLIC_API_URL=http://api.example.com electro-frontend
```

### Docker Compose (إذا كان متوفر)
```bash
docker-compose up -d
```

---

## 🚀 النشر

### Vercel
```bash
# تثبيت Vercel CLI
npm install -g vercel

# تسجيل الدخول
vercel login

# النشر
vercel

# النشر للإنتاج
vercel --prod
```

### Netlify
```bash
# تثبيت Netlify CLI
npm install -g netlify-cli

# تسجيل الدخول
netlify login

# البناء والنشر
npm run build
netlify deploy --prod
```

---

## 🧪 اختبار وتطوير

### تشغيل على Port مختلف
```bash
PORT=3001 npm run dev
```

### وضع Production محلياً
```bash
npm run build
npm start
```

### تحليل حجم Bundle
أضف في `package.json`:
```json
{
  "scripts": {
    "analyze": "ANALYZE=true npm run build"
  }
}
```

ثم:
```bash
npm install @next/bundle-analyzer
npm run analyze
```

---

## 📊 أوامر مفيدة للمطورين

### عرض معلومات المشروع
```bash
npm list --depth=0
```

### البحث عن ثغرات أمنية
```bash
npm audit

# إصلاح الثغرات
npm audit fix
```

### تنظيف الكود
```bash
# إصلاح مشاكل Lint تلقائياً
npm run lint -- --fix
```

### Format الكود (إذا كان Prettier مثبت)
```bash
npx prettier --write .
```

---

## 🔍 Debugging

### تشغيل مع Debugging
```bash
NODE_OPTIONS='--inspect' npm run dev
```

### عرض Environment Variables
```bash
# ويندوز
set

# ماك/لينكس
env | grep NEXT_PUBLIC
```

---

## 📦 إدارة الحزم

### إضافة حزمة جديدة
```bash
npm install package-name
```

### إضافة حزمة للتطوير فقط
```bash
npm install --save-dev package-name
```

### حذف حزمة
```bash
npm uninstall package-name
```

### تثبيت نسخة معينة
```bash
npm install package-name@1.2.3
```

---

## 🔄 Git Commands

### تجهيز للنشر
```bash
# تأكد من نظافة الكود
npm run lint
npm run build

# Commit
git add .
git commit -m "Ready for deployment"
git push origin main
```

---

## 🎨 Tailwind CSS

### إعادة بناء CSS
```bash
npx tailwindcss -i ./app/globals.css -o ./dist/output.css --watch
```

### مسح CSS غير المستخدم
```bash
npm run build
# Tailwind يقوم بذلك تلقائياً في Production
```

---

## 📝 الملاحظات

1. **دائماً قم بـ `npm run build` قبل النشر** للتأكد من عدم وجود أخطاء
2. **استخدم `npm run lint`** قبل كل commit
3. **اختبر على Production mode محلياً** قبل النشر الفعلي
4. **احفظ نسخة احتياطية** قبل تحديث الحزم الرئيسية

---

## 🆘 حل المشاكل

### مشكلة: الحزم لا تعمل
```bash
rm -rf node_modules package-lock.json
npm install
```

### مشكلة: .next لا يتحدث
```bash
rm -rf .next
npm run dev
```

### مشكلة: Port مستخدم
```bash
# إيقاف العملية على Port 3000
# ويندوز
netstat -ano | findstr :3000
taskkill /PID <PID> /F

# ماك/لينكس
lsof -ti:3000 | xargs kill -9
```

### مشكلة: TypeScript Errors
```bash
npx tsc --noEmit
# اقرأ الأخطاء وصححها
```

---

## 📚 موارد إضافية

- [Next.js Docs](https://nextjs.org/docs)
- [TypeScript Docs](https://www.typescriptlang.org/docs)
- [Tailwind CSS Docs](https://tailwindcss.com/docs)
- [React Docs](https://react.dev)

---

**نصيحة:** احفظ هذا الملف كمرجع سريع! 📌

